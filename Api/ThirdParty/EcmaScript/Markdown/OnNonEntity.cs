using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Api.Startup;
using Api.AvailableEndpoints;

namespace Api.EcmaScript.Markdown
{
    /// <summary>
    /// Generates examples and usage for a non-entity type.
    /// </summary>
    public static partial class MarkdownGeneration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nonEntityType"></param>
        /// <param name="module"></param>
        public static void OnNonEntity(Type nonEntityType, ModuleEndpoints module)
        {
            var ecmaService = Services.Get<EcmaService>();

            var document = GetDocument(nonEntityType);
            var docs = ecmaService.GetTypeDocumentation(nonEntityType);

            // Heading for the non-entity type
            document.AddHeading(nonEntityType.Name);
            document.AddParagraph($"*Full Type:* `{nonEntityType.FullName}`");
            document.AddParagraph("");

            // Summary (if available)
            if (docs is not null && !string.IsNullOrWhiteSpace(docs.Summary))
            {
                document.AddHorizontalRule();
                document.AddParagraph("*Auto-generated from a .NET non-entity type*");
                document.AddParagraph(docs.Summary.Trim());
                document.AddHorizontalRule();
                document.AddParagraph("");
            }

            // Fields Table
            document.AddHeading("Fields");
            document.AddParagraph("The following fields are available on this non-entity type:");

            var headers = new[] { "Name", "Type", "Nullable", "Summary" };
            var rows = new List<string[]>();

            // Loop through all fields of the non-entity type
            foreach (var field in nonEntityType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                // Skip if the field is not declared by the current type
                if (field.DeclaringType != nonEntityType)
                    continue;

                var fieldType = field.FieldType;
                bool isNullable = IsTypeNullable(fieldType);
                string typeName = GetTypeString(fieldType);
                string name = field.Name;

                // Get the summary for the field (if available)
                string summary = ecmaService.GetPropertyDocumentation(field)?.Summary?.Trim() ?? "No description available";
                
                // Add a row for this field
                rows.Add(new[] { name, $"`{typeName}`", isNullable ? "Yes" : "No", summary });
            }

            // Loop through all properties of the non-entity type
            foreach (var property in nonEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                // Skip if the property is not declared by the current type
                if (property.DeclaringType != nonEntityType)
                    continue;

                var propertyType = property.PropertyType;
                bool isNullable = IsTypeNullable(propertyType);
                string typeName = GetTypeString(propertyType);
                string name = property.Name;

                // Get the summary for the property (if available)
                string summary = ecmaService.GetPropertyDocumentation(property)?.Summary?.Trim() ?? "No description available";
                
                // Add a row for this property
                rows.Add(new[] { name, $"`{typeName}`", isNullable ? "Yes" : "No", summary });
            }

            // Add the fields and properties table to the markdown document
            document.AddTable(headers, rows);
        }
    }
}
