

using System;
using System.Collections.Generic;
using System.IO;

namespace Api.EcmaScript.Markdown
{
    /// <summary>
    /// Generates examples and usage for a specific endpoint/ entity.
    /// </summary>
    public static partial class MarkdownGeneration
    {
        private readonly static Dictionary<Type, Document> ApiMarkdown = [];

        /// <summary>
        /// Gets a document based off a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Document GetDocument(Type type)
        {
            if (ApiMarkdown.TryGetValue(type, out Document target))
            {
                return target;
            }

            ApiMarkdown.Add(type, new Document() {
                FileName = "TypeScript/Api/" + type.Name + ".md"
            });

            return ApiMarkdown[type];
        }

        /// <summary>
        /// Creates the markdown files.
        /// </summary>
        public static void GenerateMarkdown()
        {
            foreach(var entry in ApiMarkdown)
            {
                // save the document.
                entry.Value.SaveToFile();
            }
        }

        
        private static bool IsTypeNullable(Type t)
        {
            if (!t.IsValueType)
                return true;

            return Nullable.GetUnderlyingType(t) != null;
        }

        private static string GetTypeString(Type t)
        {
            if (Nullable.GetUnderlyingType(t) is Type underlying)
                return $"{underlying.Name}?";
            return t.Name;
        }
    } 
}