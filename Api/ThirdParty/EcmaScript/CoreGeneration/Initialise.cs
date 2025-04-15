
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
                From = "UI/Functions/WebRequest",
                Symbols = ["getOne", "getList", "ApiList"]
            });
            apiScript.AddImport(new() {
                From = "Api/Content",
                Symbols = ["Content", "VersionedContent", "UserCreatedContent"]
            });

            // ===== AutoAPI ===== \\
            var baseControllerClass = new ClassDefinition { 
                Name = "AutoApi",
                GenericTemplate = "EntityType extends Content, IncludeSet extends ApiIncludes"
            };
            var apiUrl = new ClassProperty
            {
                Visibility = "protected",
                PropertyName = "apiUrl",
                PropertyType = "string"
            };
            baseControllerClass.Children.Add(apiUrl);

            // add CRUD methods to controller.

            AddCrudFunctionality(baseControllerClass);
            AddBaseIncludeFunctionality();

            apiScript.AddChild(baseControllerClass);

            // === SAVING TS FILE === \\

            var filePath = "TypeScript/Api/ApiEndpoints.tsx";
			var generatedSource = apiScript.CreateSource();
            container.Add(filePath, generatedSource);
            File.WriteAllText(filePath, generatedSource);
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