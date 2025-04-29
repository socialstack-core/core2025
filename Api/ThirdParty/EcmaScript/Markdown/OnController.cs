using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Api.Startup;
using Api.AvailableEndpoints;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using System.IO;

namespace Api.EcmaScript.Markdown
{
    public static partial class MarkdownGeneration
    {
        public static void OnController(Type controllerType, Type entityType, ModuleEndpoints module)
        {
            var ecmaService = Services.Get<EcmaService>();
            var document = GetDocument(entityType);

            string entityName = entityType.Name;
            string controllerName = controllerType.Name;

            var routeAttr = controllerType.GetCustomAttribute<RouteAttribute>();
            string baseRoute = NormalizeRoute(routeAttr?.Template ?? entityName.ToLower());

            document.AddHeading($"{entityName} API");
            document.AddParagraph($"This controller provides API methods for the `{entityName}` entity.");
            document.AddParagraph($"Base URL: `/api/{baseRoute}`");
            document.AddHorizontalRule();

            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                var httpAttr = method.GetCustomAttributes()
                    .FirstOrDefault(attr => attr is HttpMethodAttribute) as HttpMethodAttribute;

                if (httpAttr == null)
                    continue;

                var httpMethod = httpAttr.HttpMethods.FirstOrDefault() ?? "GET";
                var path = NormalizeRoute(httpAttr.Template ?? method.Name.ToLower());
                var fullUrl = $"/api/{baseRoute}/{path}".TrimEnd('/');

                var summary = ecmaService.GetMethodDocumentation(method)?.Summary?.Trim() ?? "No description available.";
                var example = GenerateExampleUsage(method, fullUrl, httpMethod, entityType);

                document.AddHeading($"`{httpMethod.ToUpper()} {fullUrl}`", 3);
                document.AddParagraph(summary);
                document.AddCodeBlock(example, "ts");

                // Add a link to view the method in markdown format
                string markdownFile = Path.GetFileNameWithoutExtension(document.FileName) + ".md"; // Replace .tsx with .md
                document.AddParagraph($"[View method]({markdownFile})");
            }
        }

        public static void OnController(Type controllerType, ModuleEndpoints module)
        {
            var ecmaService = Services.Get<EcmaService>();
            var document = GetDocument(controllerType);

            string entityName = controllerType.Name;
            string controllerName = controllerType.Name;

            var routeAttr = controllerType.GetCustomAttribute<RouteAttribute>();
            string baseRoute = NormalizeRoute(routeAttr?.Template ?? entityName.ToLower());

            document.AddHeading($"{entityName} API");
            document.AddParagraph($"This controller provides API methods for the `{entityName}` entity.");
            document.AddParagraph($"Base URL: `/api/{baseRoute}`");
            document.AddHorizontalRule();

            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                var httpAttr = method.GetCustomAttributes()
                    .FirstOrDefault(attr => attr is HttpMethodAttribute) as HttpMethodAttribute;

                if (httpAttr == null)
                    continue;

                var httpMethod = httpAttr.HttpMethods.FirstOrDefault() ?? "GET";
                var path = NormalizeRoute(httpAttr.Template ?? method.Name.ToLower());
                var fullUrl = $"/api/{baseRoute}/{path}".TrimEnd('/');

                var summary = ecmaService.GetMethodDocumentation(method)?.Summary?.Trim() ?? "No description available.";
                var example = GenerateExampleUsage(method, fullUrl, httpMethod, controllerType);

                document.AddHeading($"`{httpMethod.ToUpper()} {fullUrl}`", 3);
                document.AddParagraph(summary);
                document.AddCodeBlock(example, "ts");

                // Add a link to view the method in markdown format
                string markdownFile = Path.GetFileNameWithoutExtension(document.FileName) + ".md"; // Replace .tsx with .md
                document.AddParagraph($"[View method]({markdownFile})");
            }
        }

        private static string NormalizeRoute(string route)
        {
            route = route?.ToLower() ?? "";
            return route.StartsWith("v1/") ? route[3..] : route;
        }

        private static string GenerateExampleUsage(MethodInfo method, string url, string httpMethod, Type entityType)
        {
            // Extract method name and the class name (API instance name)
            var methodName = method.Name;
            var entityName = method.DeclaringType?.Name?.Replace("Api", "") ?? "Entity"; // API class name, e.g., FrontendCodeApi => FrontendCode
            var apiInstance = entityType.Name + "Api"; // Create dynamic apiInstance name

            // Get the parameters for the method
            var parameters = method.GetParameters()
                .Where(p => !SourceGenerator.ignoreParamTypes.Contains(p.ParameterType))
                .ToList();

            // Build parameter string
            string args = parameters.Count > 0
                ? "{ " + string.Join(", ", parameters.Select(p => $"{p.Name}: {InferTypeScriptType(p.ParameterType)}")) + " }"
                : "";

            // Handle special case for localeId with default value
            string idArg = parameters.FirstOrDefault(p => p.Name?.ToLower() == "localeid")?.Name ?? "localeId";
            string defaultLocaleId = idArg == "localeId" ? "= 1" : "";

            // Convert method name to lowercase-first (lcfirst) style
            string lcMethodName = methodName.Substring(0, 1).ToLower() + methodName.Substring(1);

            // Generate the TypeScript example based on HTTP method
            return httpMethod.ToUpper() switch
            {
                "GET" => $"await {apiInstance}.{lcMethodName}();",  // For methods like reload(), getStaticFileList()
                "POST" => $"await {apiInstance}.{lcMethodName}({args});",  // For POST with parameters
                "PUT" => $"await {apiInstance}.{lcMethodName}({args});",  // For PUT with parameters
                "DELETE" => $"await {apiInstance}.{lcMethodName}();",  // For DELETE with no params
                _ => $"await {apiInstance}.request('{httpMethod.ToLower()}', '{url}', {args});"  // For other HTTP methods
            };
        }

        private static string InferTypeScriptType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(uint)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(JObject)) return "Partial<T>";
            if (type == typeof(object)) return "any";

            return type.Name;
        }
    }
}
