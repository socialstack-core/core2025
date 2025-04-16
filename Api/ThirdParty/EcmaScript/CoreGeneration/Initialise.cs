
using System;
using System.IO;
using Api.CanvasRenderer;
using Api.EcmaScript.TypeScript;
using Api.Users;

namespace Api.EcmaScript
{
    public partial class EcmaService
    {
        private void CreateBaseContentTsx(SourceFileContainer container)
        {
            var content = new Script
            {
                FileName = "TypeScript/Api/Content.tsx"
            };

            SourceGenerator.EnsureScript(content);

            content.AddTypeDefinition(
                SourceGenerator.OnGenericTypeDefinition(typeof(Api.Database.Content<uint>), content)
            );
            content.AddTypeDefinition(
                SourceGenerator.OnGenericTypeDefinition(typeof(UserCreatedContent<uint>), content)
            );
            content.AddTypeDefinition(
                SourceGenerator.OnGenericTypeDefinition(typeof(VersionedContent<uint>), content)
            );
        }

        private void CreateBaseApi(SourceFileContainer container)
        {
            CreateBaseContentTsx(container);

            var apiScript = new Script() {
                FileName = "TypeScript/Api/ApiEndpoints.tsx"
            };

            SourceGenerator.EnsureScript(apiScript);

            apiScript.FileName = "TypeScript/Api/ApiEndpoints.tsx";

            apiScript.AddImport(new() {
                Symbols = ["ApiIncludes"],
                From = "./Includes"
            });

            apiScript.AddImport(new() {
                Symbols = ["getOne", "getList"],
                From = "UI/Functions/WebRequest"
            });

            apiScript.AddChild(
                SourceGenerator.OnGenericControllerClass(typeof(AutoController<,>), apiScript)
            );
        }
        /// <summary>
        /// Converts C# names to TS names.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string LcFirst(string value)
        {
            return string.Concat(value[0].ToString().ToLower(), value.AsSpan(1, value.Length - 1)); 
        }

    }
}