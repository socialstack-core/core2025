using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        public static void AddMethodBody(MethodInfo method, ClassMethod classMethod, Script script, string baseUrl)
        {
            var ecmaService = Services.Get<EcmaService>();
            var endpointUrl = GetEndpointUrl(method) ?? "";

            var parameters = method.GetParameters();
            var bodyParam = parameters.FirstOrDefault(p => p.GetCustomAttribute<FromBodyAttribute>() != null);
            bool hasBodyParam = bodyParam != null;

            HandleParameterTypes(parameters, script);

            endpointUrl = ReplaceRouteAndQueryParams(endpointUrl, parameters);

            var caller = ResolveCaller(method);

            classMethod.Injected = GenerateCall(caller, endpointUrl, bodyParam, parameters);

            AddMethodDocs(ecmaService, method, classMethod);
        }

        private static void HandleParameterTypes(ParameterInfo[] parameters, Script script)
        {
            var ecmaService = Services.Get<EcmaService>();

            foreach (var param in parameters)
            {
                var paramType = GetResolvedType(param.ParameterType);

                if (!TypeDefinitionExists(paramType.Name))
                {
                    if (IsEntity(paramType))
                    {
                        script.AddImport(new()
                        {
                            Symbols = [param.Name],
                            From = "./" + param.Name
                        });
                    }
                    else if (!ecmaService.TypeConversions.ContainsKey(paramType) && !ignoreParamTypes.Contains(paramType))
                    {
                        var generated = OnNonEntity(paramType, script);
                        script.AddTypeDefinition(generated);
                    }
                }
            }
        }

        private static string ReplaceRouteAndQueryParams(string endpointUrl, ParameterInfo[] parameters)
        {
            var queryParams = new List<string>();

            // Process route parameters first
            foreach (var param in parameters)
            {
                if (endpointUrl.Contains($"{{{param.Name}}}"))
                {
                    endpointUrl = endpointUrl.Replace($"{{{param.Name}}}", $"' + {param.Name} + '");
                }

                // Process query parameters separately
                if (param.GetCustomAttribute<FromQueryAttribute>() != null)
                {
                    queryParams.Add($"{param.Name}=' + {param.Name} + '");
                }
            }

            // Add query parameters after route parameters
            if (queryParams.Any())
            {
                var queryString = string.Join("&", queryParams);
                endpointUrl += (endpointUrl.Contains("?") ? "&" : "?") + queryString;
            }

            return endpointUrl.Replace("?&", "?");
        }

        private static string ResolveCaller(MethodInfo method)
        {
            var returnType = method.ReturnType;
            var genericArgs = returnType.IsGenericType ? returnType.GetGenericArguments() : Type.EmptyTypes;

            return (genericArgs.Length > 0 && IsList(genericArgs[0])) ? "getList" : "getOne";
        }

        private static List<string> GenerateCall(string caller, string endpointUrl, ParameterInfo bodyParam, ParameterInfo[] allParams)
        {
            var urlPart = string.IsNullOrEmpty(endpointUrl) ? "this.apiUrl" : $"this.apiUrl + '/{endpointUrl}'";
            var debug = true;

            if (bodyParam != null)
            {
                return [ 
                    $"return {caller}({urlPart}, {bodyParam.Name})"
                ];
            }

            var multipleParams = allParams
                .Where(p => p.GetCustomAttribute<FromBodyAttribute>() != null)
                .ToList();

            if (multipleParams.Count > 1)
            {
                var injected = new List<string> {
                    $"return getOne({urlPart}, {{" 
                };
                injected.AddRange(multipleParams.Select(p => $"{p.Name},"));
                injected.Add("})");
                return injected;
            }

            if (multipleParams.Count == 1)
            {
                return
                [
                    $"return {caller}({urlPart}, {{",
                    multipleParams[0].Name,
                    "})"
                ];
            }

            return [$"return {caller}({urlPart})"];
        }

        private static void AddMethodDocs(EcmaService ecmaService, MethodInfo method, ClassMethod classMethod)
        {
            var docs = ecmaService.GetMethodDocumentation(method);

            if (docs == null) return;

            var summary = string.IsNullOrWhiteSpace(docs.Summary) ? "No summary available" : docs.Summary.Trim();
            classMethod.AddTsDocLine(summary);

            if (docs.Parameters != null)
            {
                foreach (var paramDoc in docs.Parameters)
                {
                    classMethod.AddTsDocLine($"@param {{{paramDoc.Key}}} - {paramDoc.Value}");
                }
            }
        }
    }
}
