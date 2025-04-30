using System;
using System.Reflection;
using Api.AvailableEndpoints;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    /// <summary>
    /// The new and vastly improved source generation engine.
    /// Made to be a lot more maintainable and easier to identify errors.
    /// </summary>
    public static partial class SourceGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="containingScript"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public static TypeDefinition OnEntity(Type entityType, Script containingScript, ModuleEndpoints module)
        {
            EnsureScript(containingScript);

            if (!IsEntity(entityType))
            {
                return OnNonEntity(entityType, containingScript);
            }

            var ecmaService = Services.Get<EcmaService>();
            var baseTypeName = GetCleanTypeName(entityType.BaseType);

            var typeDefinition = new TypeDefinition
            {
                Name = GetResolvedTypeName(entityType),
                FromType = entityType,
                Inheritence = [baseTypeName + "<uint>"]
            };

            containingScript.AddImport(new()
            {
                Symbols = [baseTypeName],
                From = "./Content"
            });

            foreach (var field in module.GetAutoService().GetContentFields().List)
            {
                AddEntityFieldOrProperty(field, entityType, containingScript, typeDefinition);
            }

            var docs = ecmaService.GetTypeDocumentation(entityType);
            if (docs is not null)
            {
                typeDefinition.AddTsDocLine(docs.Summary.Trim());
            }

            return typeDefinition;
        }

        private static void AddEntityFieldOrProperty(ContentField field, Type entityType, Script script, TypeDefinition typeDefinition)
        {
            MemberInfo member = field.FieldInfo ?? (MemberInfo)field.PropertyInfo;
            if (member == null || member.DeclaringType != entityType)
            {
                return;
            }

            Type memberType = member switch
            {
                FieldInfo fi => fi.FieldType,
                PropertyInfo pi => pi.PropertyType,
                _ => null
            };

            if (memberType == null) return;

            bool isNullable = IsTypeNullable(memberType);
            string memberName = LcFirst(member.Name) + (isNullable ? '?' : "");
            string typeString = OnField(memberType, script);

            typeDefinition.AddProperty(memberName, typeString);
        }
    }
}
