
using System;
using System.Reflection;
using Api.AvailableEndpoints;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new &amp; vastly improved source generation engine.
    /// made to be a lot more maintainable, and easy to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {

        public static TypeDefinition OnEntity(Type entityType, Script containingScript, ModuleEndpoints module)
        {
            // ensure the script passed is registered.
            EnsureScript(containingScript);

            if (!IsEntity(entityType))
            {
                return OnNonEntity(entityType, containingScript);
            }

            var ecmaService = Services.Get<EcmaService>();

            // create a non-entity.
            var entity = new TypeDefinition() {
                Name = GetResolvedTypeName(entityType),
                Inheritence = [GetCleanTypeName(entityType.BaseType)]
            };

            containingScript.AddImport(new() {
                Symbols = [GetCleanTypeName(entityType.BaseType)],
                From = "./Content"
            });

            foreach(var field in module.GetAutoService().GetContentFields().List)
            {
                if (field.FieldInfo is not null)
                {
                    if (field.FieldInfo.DeclaringType != entityType)
                    {
                        continue;
                    }

                    var isNullable = IsTypeNullable(field.FieldInfo.FieldType);

                    entity.AddProperty(LcFirst(field.FieldInfo.Name) + (isNullable ? '?' : ""), OnField(field.FieldInfo.FieldType, containingScript));
                }
                else if (field.PropertyInfo is not null)
                {
                    if (field.PropertyInfo.DeclaringType != entityType)
                    {
                        continue;
                    }
                    
                    var isNullable = IsTypeNullable(field.PropertyInfo.PropertyType);
                    
                    entity.AddProperty(LcFirst(field.PropertyInfo.Name) + (isNullable ? '?' : ""), OnField(field.PropertyInfo.PropertyType, containingScript));
                }
            }

            var docs = ecmaService.GetTypeDocumentation(entityType);

            if (docs is not null)
            {
                entity.AddTsDocLine(docs.Summary.Trim());
            }

            return entity;
        }
    }
}