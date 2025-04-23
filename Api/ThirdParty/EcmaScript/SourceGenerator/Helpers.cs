
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Api.AvailableEndpoints;
using Api.CanvasRenderer;
using Api.Database;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Api.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new &amp; vastly improved source generation engine.
    /// made to be a lot more maintainable, and easy to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {

        private readonly static Type[] EntityBaseTypes = [
            typeof(Content),
            typeof(UserCreatedContent<>),
            typeof(VersionedContent<>)
        ];

        private static TypeDefinition CreateType(Type newType, Script script = null)
        {
            if (script is null)
            {
                script = new Script() {
                    FileName = "TypeScript/Api/" + GetCleanTypeName(newType) + ".tsx"
                };
            }

            EnsureScript(script);

            if (IsEntity(newType))
            {
                var aes = Services.Get<AvailableEndpointService>();

                var module = aes.ListByModule().Find(
                    mod => mod.GetContentType() == newType
                );

                if (module is null)
                {
                    throw new Exception($"Cannot find corresponding module for entity type {newType.FullName}");
                }

                return OnEntity(newType, script, module);
            }
            
            // non entity type, can be placed in a script, 
            // or if no script is provided, it creates a new one.

            return OnNonEntity(newType, script);
        }

        private static string GetResolvedTypeName(Type t)
        {
            // If it's a nullable type (e.g., int?), resolve the underlying type
            var underlyingNullable = Nullable.GetUnderlyingType(t);
            if (underlyingNullable != null)
            {
                return GetResolvedTypeName(underlyingNullable);
            }

            // If it's Task<T> or ValueTask<T>, resolve the inner type
            if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();
                if (genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>))
                {
                    return GetResolvedTypeName(t.GetGenericArguments()[0]);
                }
            }
            return t.Name;
        }

        private static Type GetResolvedType(Type t)
        {
            // If it's a nullable type (e.g., int?), resolve the underlying type
            var underlyingNullable = Nullable.GetUnderlyingType(t);
            if (underlyingNullable != null)
            {
                return GetResolvedType(underlyingNullable);
            }

            // If it's Task<T> or ValueTask<T>, resolve the inner type
            if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();
                if (genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>))
                {
                    return GetResolvedType(t.GetGenericArguments()[0]);
                }
            }

            if (t == typeof(Task) || t == typeof(ValueTask))
            {
                return typeof(void);
            }

            return t;
        }

        private static bool IsList(Type t)
        {
            return GetListOfType(t) != null;
        }

        private static Type GetListOfType(Type t)
        {
            // If it's a nullable type (e.g., int?), resolve the underlying type
            var underlyingNullable = Nullable.GetUnderlyingType(t);
            if (underlyingNullable != null)
            {
                return GetListOfType(underlyingNullable);
            }

            if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();
                var argZero = t.GetGenericArguments()[0];

				// If it's Task<T> or ValueTask<T>, resolve the inner type
				if (genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>))
                {
                    return GetListOfType(argZero);
                }

                if (genericDef == typeof(IEnumerable<>))
                {
                    return argZero;
                }
                if (genericDef == typeof(List<>))
                {
                    return argZero;
                }
                if (genericDef == typeof(ContentStream<,>))
                {
                    return argZero;
                }
            }

            if (t.IsArray)
            {
                return t.GetElementType();
            }

            return null;
        }


        private static string GetCleanTypeName(Type t)
        {
            return t.Name.Contains('`') ? t.Name[..t.Name.IndexOf('`')] : t.Name;
        }

        private static bool IsTypeNullable(Type t)
        {
            return Nullable.GetUnderlyingType(t) != null || !t.IsValueType;
        }

        /// <summary>
        /// Check if the given type is an entity.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsEntity(Type type)
        {
            foreach (var baseType in EntityBaseTypes)
            {
                var current = type;
                while (current != null && current != typeof(object))
                {
                    if (baseType.IsGenericTypeDefinition && current.IsGenericType &&
                        current.GetGenericTypeDefinition() == baseType)
                    {
                        return true;
                    }
                    if (!baseType.IsGenericTypeDefinition && baseType.IsAssignableFrom(current))
                    {
                        return true;
                    }
                    current = current.BaseType;
                }
            }
            return false;
        }


        private static string LcFirst(string value)
        {
            return string.Concat(value[0].ToString().ToLower(), value.AsSpan(1, value.Length - 1)); 
        }

        private static bool TypeDefinitionExists(string name)
        {
            foreach(var script in scripts)
            {
                foreach(var child in script.Children)
                {
                    if (child.GetType() == typeof(TypeDefinition) && (child as TypeDefinition).Name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Script GetScriptByContainingTypeDefinition(string name)
        {
            foreach(var script in scripts)
            {
                foreach(var child in script.Children)
                {
                    if (child.GetType() == typeof(TypeDefinition) && (child as TypeDefinition).Name == name)
                    {
                        return script;
                    }
                }
            }
            return null;
        }


        private static bool IsEndpoint(MethodInfo method)
        {
            // Check if any of the MVC HTTP attributes exist on the method
            var httpAttributes = method.GetCustomAttributes(true)
                                    .OfType<Attribute>()
                                    .Any(attr => attr is HttpGetAttribute || 
                                                    attr is HttpPostAttribute || 
                                                    attr is HttpPutAttribute || 
                                                    attr is HttpDeleteAttribute || 
                                                    attr is HttpPatchAttribute ||
                                                    attr is RouteAttribute);

            return httpAttributes;
        }

        private static string GetEndpointUrl(MethodInfo method)
        {
            // Check if the method has any of the relevant HTTP attributes
            var httpAttributes = method.GetCustomAttributes(true)
                                    .OfType<Attribute>()
                                    .Where(attr => attr is RouteAttribute || 
                                                    attr is HttpGetAttribute || 
                                                    attr is HttpPostAttribute || 
                                                    attr is HttpPutAttribute || 
                                                    attr is HttpDeleteAttribute || 
                                                    attr is HttpPatchAttribute)
                                    .ToList();

            // If no relevant attribute is found, return null or an appropriate default value
            if (!httpAttributes.Any())
            {
                return null;
            }

            // Extract URL from RouteAttribute or the HTTP method attributes
            foreach (var attr in httpAttributes)
            {
                // RouteAttribute has a property called 'Template' which contains the URL pattern
                if (attr is RouteAttribute routeAttribute)
                {
                    return routeAttribute.Template;
                }

                // For HttpGet, HttpPost, etc., the URL is usually passed in the constructor
                // If the attribute has a 'Template' property, we extract that
                if (attr is HttpMethodAttribute httpMethodAttribute && !string.IsNullOrEmpty(httpMethodAttribute.Template))
                {
                    return httpMethodAttribute.Template;
                }
            }

            // Return null or a default string if no URL is found
            return null;
        }

        private static bool IsContentType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Content<>))
            {
                return true;
            }
    
            var baseType = type.BaseType;
    
            if (baseType == null)
            {
                return false;
            }
    
            return IsContentType(baseType);
        }

        private static string GetHttpMethodFromAttribute(MethodInfo method)
        {
            var attrs = method.GetCustomAttributes();

            foreach (var attr in attrs)
            {
                var typeName = attr.GetType().Name;

                if (typeName == nameof(HttpPostAttribute)) return "POST";
                if (typeName == nameof(HttpPutAttribute)) return "PUT";
                if (typeName == nameof(HttpPatchAttribute)) return "PATCH";
                if (typeName == nameof(HttpDeleteAttribute)) return "DELETE";
                if (typeName == nameof(HttpGetAttribute)) return "GET";
            }

            // Default to GET if nothing is specified
            return "GET";
        }

    }
}