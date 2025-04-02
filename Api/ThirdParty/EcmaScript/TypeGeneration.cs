

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Api.Uploader;

namespace Api.EcmaScript
{
    public partial class EcmaService
    {

        private readonly List<TypeDefinition> typeDefinitionCache = []; 

        private void AddFieldsToType(Type source, TypeDefinition target, Script script)
        {
            if (typeDefinitionCache.Contains(target))
            {
                return;
            }
            typeDefinitionCache.Add(target);
            foreach (var field in source.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // Skip compiler-generated backing fields (e.g., auto-properties)
                if (Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute)))
                {
                    continue;
                }

                var fieldName = field.Name;

                // Remove leading underscore and lowercase first letter
                if (fieldName[0] == '_')
                {
                    fieldName = fieldName[1..];
                }

                fieldName = LcFirst(fieldName);

                var type = field.FieldType;

                // Handle nullable types
                var nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                {
                    type = nullableType;
                }

                if (IsNullableType(type))
                {
                    fieldName += "?"; // optional
                }

                if (type == typeof(string) && fieldName.EndsWith("Ref")){
                    type = typeof(FileRef);
                }

                

                // Skip types already converted (to prevent duplicates or circular references)
                if (TypeConversions.TryGetValue(type, out string value))
                {
                    if (IsCollection(field)) 
                    {
                        target.AddProperty(fieldName, value + "[]");
                    }
                    else
                    {
                        target.AddProperty(fieldName, value);
                    }
                    continue;
                }

                var targetCached = typeDefinitionCache.Where(cached => cached.Name == type.Name);

                // If it's a collection (array or generic collection), handle its item type
                if (IsCollection(field))
                {
                    var objectType = GetCollectionItemType(field);

                    if (targetCached.Any())
                    {
                        target.AddProperty(fieldName, targetCached.First().Name);
                        continue;
                    }

                    if (TypeConversions.TryGetValue(objectType, out string collValue))
                    {
                        target.AddProperty(
                            fieldName,
                            collValue + "[]"
                        );
                    }
                    else
                    {
                        // If the object type is not yet in TypeConversions, generate it
                        if (script != null)
                        {
                            // Avoid processing duplicate or circular references
                            if (script.Children.Any(obj => obj.GetType() == typeof(TypeDefinition) && (obj as TypeDefinition).Name == objectType.Name))
                            {
                                continue;
                            }

                            if (source.Name == type.Name)
                            {
                                target.AddProperty(fieldName, type.Name + "[]");
                            }
                            else
                            {

                                // Generate the type definition recursively
                                var generatedType = new TypeDefinition() { Name = objectType.Name };
                                AddFieldsToType(objectType, generatedType, script);

                                TypeConversions[objectType] = generatedType.Name + "[]";
                                script.AddChild(generatedType);
                                target.AddProperty(fieldName, generatedType.Name + "[]");
                            }
                        }
                    }
                }
                else
                {
                    if (targetCached.Any())
                    {
                        target.AddProperty(fieldName, targetCached.First().Name);
                        continue;
                    }
                    // If the field is not a collection, check if it's a known type
                    if (TypeConversions.TryGetValue(type, out string nonCollType))
                    {
                        target.AddProperty(fieldName, nonCollType);
                    }
                    else
                    {
                        // If the type is not yet processed, generate it
                        if (script != null)
                        {
                            // Avoid circular references by checking the script's existing types
                            if (script.Children.Any(obj => obj.GetType() == typeof(TypeDefinition) && (obj as TypeDefinition).Name == type.Name))
                            {
                                continue;
                            }           
                        }
                        if (source.Name == type.Name)
                        {
                            target.AddProperty(fieldName, type.Name);
                        }
                        else
                        {
                            var generatedType = new TypeDefinition() { Name = type.Name };
                            AddFieldsToType(type, generatedType, script);
                            TypeConversions[type] = generatedType.Name + "[]";
                            script.AddChild(generatedType);
                            target.AddProperty(fieldName, generatedType.Name);
                        }
                    }
                }
            }
        }
        private void AddFieldsToType(List<ContentField> fields, TypeDefinition typeDef, Type source)
        {
            if (typeDefinitionCache.Contains(typeDef))
            {
                return;
            }
            typeDefinitionCache.Add(typeDef);

            foreach(var field in fields)
            {
                Type targetType;
                
                if (field.FieldInfo is not null)
                {
                    targetType = field.FieldInfo.FieldType;
                }
                else
                {
                    targetType = field.PropertyInfo.PropertyType;
                }
                if (Nullable.GetUnderlyingType(targetType) != null)
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }

                if (!field.IsVirtual)
                {
                    if (field.FieldInfo is not null)
                    {
                        if (field.FieldInfo.DeclaringType != source)
                        {
                            continue;
                        }
                    }
                    else if (field.PropertyInfo is not null)
                    {
                        if (field.PropertyInfo.DeclaringType != source)
                        {
                            continue;
                        }
                    }
                }

                var jsonIgnore = field.PropertyInfo?.GetCustomAttribute<JsonIgnoreAttribute>();

                jsonIgnore ??= field.FieldInfo?.GetCustomAttribute<JsonIgnoreAttribute>();

                if (jsonIgnore is not null)
                {
                    continue;
                }
                var fieldName = field.Name;

                if (fieldName.EndsWith("Ref"))
                {
                    targetType = typeof(FileRef);
                }
                

                if (IsNullableType(field.FieldType))
                {
                    fieldName += "?";
                }
                typeDef.AddProperty(fieldName, GetTypeConversion(targetType));
            }
        }
    }
}