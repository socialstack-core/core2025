
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

            // ==== CONTENT.CS ==== \\
            var contentType = new TypeDefinition() {
                Name = "Content"
            };
            contentType.AddTsDocLine("* The base content type for all content.");
            contentType.AddProperty("id", "uint");
            contentType.AddProperty("type", "string | null");
            content.AddChild(contentType);

            // ===== VERSIONEDCONTENT.CS ===== \\
            var versionedContent = new TypeDefinition() {
                Name = "VersionedContent",
                Inheritence = ["UserCreatedContent"]
            };
            versionedContent.AddTsDocLine("* The base content type for all content.");
            versionedContent.AddProperty("revisionId?", "uint");
            content.AddChild(versionedContent);

            var generatedSource = content.CreateSource();
            container.Add(content.FileName, generatedSource);
			File.WriteAllText(content.FileName, generatedSource);
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
        public static string LcFirst(string value)
        {
            return string.Concat(value[0].ToString().ToLower(), value.AsSpan(1, value.Length - 1)); 
        }

    }
}