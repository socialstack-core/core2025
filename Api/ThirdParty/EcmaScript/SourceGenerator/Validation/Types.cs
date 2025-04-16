
using System;
using System.Collections.Generic;
using System.Linq;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    /// <summary>
    /// This adds any missing types 
    /// </summary>
    public static partial class SourceGenerator
    {
        /// <summary>
        /// Ensures all defined types are valid and exist.
        /// </summary>
        /// <param name="script">The script to validate types for.</param>
        private static void ValidateTypes(Script script)
        {
            var ecmaService = Services.Get<EcmaService>();
            var types = script.Children.OfType<TypeDefinition>().ToList();

            foreach (var type in types)
            {
                var newProperties = new Dictionary<string, string>();

                foreach (var property in type.Properties)
                {
                    var originalType = property.Value;
                    var cleanType = StripGenerics(originalType);

                    if (ShouldIgnoreType(cleanType, script, ecmaService))
                    {
                        newProperties[property.Key] = originalType;
                        continue;
                    }

                    var fieldName = NormalizeFieldName(property.Key);
                    var field = type.FromType.GetField(fieldName);
                    var propertyInfo = type.FromType.GetProperty(fieldName);

                    if (field is not null)
                    {
                        newProperties[property.Key] = ProcessMemberType(field.FieldType, script, ecmaService);
                    }
                    else if (propertyInfo is not null)
                    {
                        newProperties[property.Key] = ProcessMemberType(propertyInfo.PropertyType, script, ecmaService);
                    }
                    else
                    {
                        newProperties[property.Key] = originalType;
                    }
                }

                type.Properties = newProperties;
            }
        }
        private static string StripGenerics(string typeName)
        {
            var index = typeName.IndexOf('<');
            return index > -1 ? typeName[..index] : typeName;
        }

        private static bool ShouldIgnoreType(string type, Script script, EcmaService ecmaService)
        {
            return IgnoreTypes.Contains(type) ||
                IsSymbolImported(script, type) ||
                ecmaService.TypeConversions.ContainsValue(type);
        }

        private static string NormalizeFieldName(string name)
        {
            var clean = name.Replace("?", "");
            return string.Concat(char.ToUpper(clean[0]), clean[1..]);
        }

        private static string ProcessMemberType(Type type, Script script, EcmaService ecmaService)
        {
            if (IsEntity(type))
            {
                return type.Name;
            }

            if (IsList(type))
            {
                var elementType = type.GetGenericArguments()[0];

                if (ecmaService.TypeConversions.ContainsKey(elementType))
                {
                    return $"{elementType.Name}[]";
                }

                var gennedType = OnNonEntity(elementType, script, 0);
                script.AddChild(gennedType);
                return $"{gennedType.Name}[]";
            }

            var generatedType = OnNonEntity(type, script, 0);
            script.AddChild(generatedType);
            return generatedType.Name;
        }


        private static bool IsSymbolImported(Script script, string symbol)
        {
            return (
                script.Imports.Where(
                    import => import.Symbols.Contains(symbol)
                ).Any()
            );
        }
    }
}