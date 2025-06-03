using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Api.Translate;
using Api.TypeScript.Objects;

namespace Api.TypeScript
{
    public partial class TypeScriptService : AutoService
    {
        /// <summary>
        /// Checks whether the provided type is a nullable type (e.g., int?, DateTime?).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is nullable; otherwise, false.</returns>
        public static bool IsNullable(Type type)
        {
            // Reference types are considered nullable
            if (!type.IsValueType)
            {
                return true;
            }

            // Nullable value types are generic types of Nullable<T>
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsContentfulValueTask(Type type, out Type contentType)
        {
            while (true)
            {
                // Check if the type is generic and is a ValueTask<>
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
                {
                    type = type.GetGenericArguments()[0];
                    continue;
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Localized<>))
                {
                    contentType = type.GetGenericArguments()[0];
                    return true;
                }

                contentType = null;
                return false;
                break;
            }
        }

        public static Type UnwrapTypeNesting(Type type)
        {
            if (IsContentfulValueTask(type, out var resolved))
            {
                type = resolved;
            }

            if (IsNullable(type))
            {
                type = Nullable.GetUnderlyingType(type) ?? type;
            }

            if (IsEnumerable(type) && type.GetGenericArguments().Length != 0)
            {
                type = type.GetGenericArguments()[0];
            }

            return type;
        }
        
        public static bool IsNestedCollection(Type type)
        {
            if (IsContentfulValueTask(type, out var resolved))
            {
                type = resolved;
            }

            if (IsNullable(type))
            {
                type = Nullable.GetUnderlyingType(type) ?? type;
            }

            if (IsEnumerable(type))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Converts the first character of the string to lowercase.
        /// If the string is null, empty, or the first character is already lowercase, the original string is returned.
        /// </summary>
        /// <param name="input">The string to transform.</param>
        /// <returns>The string with its first character lowercased.</returns>
        public static string LcFirst(string input)
        {
            if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            {
                return input;
            }

            return char.ToLower(input[0]) + input[1..];
        }

        public static void EnsureParameterTypes(List<ParameterInfo> types, ESModule module)
        {
            foreach (var param in types)
            {
                var type = param.ParameterType;
                
                if (type.IsGenericParameter)
                {
                    continue;
                }
                if (type.IsPrimitive)
                {
                    continue;
                }
                
                module.AddType(type);
            }
        }

        public static void EnsureApis(ControllerMethod method, ESModule module, Type selfType)
        {
            var isArrayType = method.IsApiList;

            if (isArrayType)
            {
                module.RequireWebApi(method.ReturnType == selfType
                    ? WebApis.GetList
                    : WebApis.GetJson);
            }
            else
            {
                module.RequireWebApi(IsEntityType(method.ReturnType)
                    ? WebApis.GetOne
                    : WebApis.GetJson);
            }
        }
        
    }
}