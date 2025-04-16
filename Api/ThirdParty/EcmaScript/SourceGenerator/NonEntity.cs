
using System;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new &amp; vastly improved source generation engine.
    /// made to be a lot more maintainable, and easy to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {

        public static TypeDefinition OnNonEntity(Type nonEntityType, Script containingScript, int currentDepth = 0)
        {

            // ensure the script passed is registered.
            EnsureScript(containingScript);

            // create a non-entity.
            var nonEntity = new TypeDefinition() {
                Name = GetResolvedTypeName(nonEntityType),
                FromType = nonEntityType
            };

            foreach(var field in nonEntityType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.DeclaringType != nonEntityType) 
                {
                    continue;
                }

                if (field.FieldType == nonEntityType)
                {
                    // its a self ref
                    nonEntity.AddProperty(LcFirst(field.Name) + '?', nonEntityType.Name);                    
                }
                else
                {
                    var isNullable = IsTypeNullable(field.FieldType);
                    nonEntity.AddProperty(LcFirst(field.Name) + (isNullable ? "?" : ""), OnField(field.FieldType, containingScript, currentDepth + 1));
                }
            }

            foreach(var field in nonEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.DeclaringType != nonEntityType) 
                {
                    continue;
                }
                if (field.PropertyType == nonEntityType)
                {
                    // its a self ref
                    nonEntity.AddProperty(LcFirst(field.Name) + '?', nonEntityType.Name);  
                }
                else
                {
                    var isNullable = IsTypeNullable(field.PropertyType);
                    nonEntity.AddProperty(LcFirst(field.Name) + (isNullable ? "?" : ""), OnField(field.PropertyType, containingScript, currentDepth + 1));
                }
            }

            return nonEntity;
        }
    }
}