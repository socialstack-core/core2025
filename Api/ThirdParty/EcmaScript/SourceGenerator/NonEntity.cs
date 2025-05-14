using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    /// <summary>
    /// The improved source generation engine.
    /// Designed for maintainability and easy error identification.
    /// </summary>
    public static partial class SourceGenerator
    {

        /// <summary>
        /// 
        /// </summary>
        public static readonly Dictionary<Type, Dictionary<string, string>> FieldOverwrites = [];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nonEntityType"></param>
        /// <param name="containingScript"></param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        public static TypeDefinition OnNonEntity(Type nonEntityType, Script containingScript, int currentDepth = 0)
        {
            // Ensure the script is registered.
            EnsureScript(containingScript);

            // Create a non-entity definition.
            var nonEntity = new TypeDefinition
            {
                Name = GetResolvedTypeName(nonEntityType),
                FromType = nonEntityType
            };

            // Process fields and properties of the non-entity type.
            ProcessFields(nonEntityType, nonEntity, containingScript, currentDepth);
            ProcessProperties(nonEntityType, nonEntity, containingScript, currentDepth);

            return nonEntity;
        }

        /// <summary>
        /// Process fields for a given non-entity type.
        /// </summary>
        private static void ProcessFields(Type nonEntityType, TypeDefinition nonEntity, Script containingScript, int currentDepth)
        {
            foreach (var field in nonEntityType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                // Skip if the field is not declared by the current type.
                if (field.DeclaringType != nonEntityType)
                    continue;

                // Handle self-reference or regular field.
                if (field.FieldType == nonEntityType)
                {
                    nonEntity.AddProperty(LcFirst(field.Name) + "?", nonEntityType.Name);
                }
                else
                {
                    var isNullable = IsTypeNullable(field.FieldType) || field.FieldType.IsPrimitive;

                    if (FieldOverwrites.TryGetValue(nonEntityType, out Dictionary<string, string> fieldOverwrite))
                    {
                        // this class has associated field overwrites

                        if (fieldOverwrite.TryGetValue(field.Name, out string overridingType))
                        {
                            nonEntity.AddProperty(LcFirst(field.Name) + (isNullable ? "?" : ""), overridingType);
                            continue;
                        }
                    }
                    
                    var fieldType = OnField(field.FieldType, containingScript, currentDepth + 1);
                    nonEntity.AddProperty(LcFirst(field.Name) + (isNullable ? "?" : ""), fieldType);
                }
            }
        }

        /// <summary>
        /// Process properties for a given non-entity type.
        /// </summary>
        private static void ProcessProperties(Type nonEntityType, TypeDefinition nonEntity, Script containingScript, int currentDepth)
        {
            foreach (var property in nonEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                // Skip if the property is not declared by the current type.
                if (property.DeclaringType != nonEntityType)
                    continue;

                // Handle self-reference or regular property.
                if (property.PropertyType == nonEntityType)
                {
                    nonEntity.AddProperty(LcFirst(property.Name) + "?", nonEntityType.Name);
                }
                else
                {
                    var isNullable = IsTypeNullable(property.PropertyType);

                    if (FieldOverwrites.TryGetValue(nonEntityType, out Dictionary<string, string> fieldOverwrite))
                    {
                        // this class has associated field overwrites

                        if (fieldOverwrite.TryGetValue(property.Name, out string overridingType))
                        {
                            nonEntity.AddProperty(LcFirst(property.Name) + (isNullable ? "?" : ""), overridingType);
                            continue;
                        }
                    }
                    var propertyType = OnField(property.PropertyType, containingScript, currentDepth + 1);
                    nonEntity.AddProperty(LcFirst(property.Name) + (isNullable ? "?" : ""), propertyType);
                }
            }
        }
    }
}
