

using System;

namespace Api.EcmaScript.Markdown
{
    public static partial class MarkdownGeneration
    {
        public static void OnImport(Type t)
        {
            var document = GetDocument(t);

            document.AddHeading("Using this API", 3);
            document.AddCodeBlock("import " + t.Name + "Api, { " + t.Name + " } from 'Api/" + t.Name + "';", "typescript");
        }
    }
}