using System;
using System.Reflection;
using System.Collections.Generic;
using Api.Startup;
using Api.AvailableEndpoints;

namespace Api.EcmaScript.Markdown
{
    /// <summary>
    /// Generates examples and usage for a specific endpoint/entity.
    /// </summary>
    public static partial class MarkdownGeneration
    {
        public static void OnEntity(Type entity, ModuleEndpoints module)
        {
            var ecmaService = Services.Get<EcmaService>();

            var document = GetDocument(entity);
            var docs = ecmaService.GetTypeDocumentation(entity);

            document.AddHeading(entity.Name);
            document.AddParagraph($"*Full Type:* `{entity.FullName}`");
            document.AddParagraph("");

            // Summary
            if (docs is not null && !string.IsNullOrWhiteSpace(docs.Summary))
            {
                document.AddHorizontalRule();
                document.AddParagraph("*Auto-generated from a .NET entity inside the Api/ directory*");
                document.AddParagraph(docs.Summary.Trim());
                document.AddHorizontalRule();
                document.AddParagraph("");
            }

            // Fields Table
            document.AddHeading("Fields");
            document.AddParagraph("The following fields are available on this entity:");

            var headers = new[] { "Name", "Type", "Nullable", "Summary" };
            var rows = new List<string[]>();

            foreach (var field in module.GetAutoService().GetContentFields().List)
            {
                var member = field.FieldInfo ?? (MemberInfo)field.PropertyInfo;

                if (member == null || member.DeclaringType != entity)
                    continue;

                var memberType = field.FieldInfo != null ? field.FieldInfo.FieldType : field.PropertyInfo.PropertyType;
                bool isNullable = IsTypeNullable(memberType);
                string typeName = GetTypeString(memberType);
                string name = member.Name;
                string summary;
                if (field.FieldInfo is not null)
                {
                    summary = ecmaService.GetPropertyDocumentation(field.FieldInfo)?.Summary?.Trim() ?? "No description available";
                }
                else
                {
                    summary = ecmaService.GetPropertyDocumentation(field.PropertyInfo)?.Summary?.Trim() ?? "No description available";
                }
                rows.Add([ name, $"`{typeName}`", isNullable ? "Yes" : "No", summary ]);
            }

            document.AddTable(headers, rows);
        }

    }
}
