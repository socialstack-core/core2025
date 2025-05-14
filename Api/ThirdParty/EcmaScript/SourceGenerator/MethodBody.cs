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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="classMethod"></param>
        /// <param name="script"></param>
        /// <param name="baseUrl"></param>
        /// <param name="resolvedReturnType"></param>
        public static void AddMethodBody(MethodInfo method, ClassMethod classMethod, Script script, string baseUrl, Type resolvedReturnType, bool returnsList)
        {
            var ecmaService = Services.Get<EcmaService>();
            var endpointUrl = GetEndpointUrl(method) ?? "";

            var parameters = method.GetParameters();
            var bodyParam = parameters.FirstOrDefault(p => p.GetCustomAttribute<FromBodyAttribute>() != null);
            bool hasBodyParam = bodyParam != null;

            HandleParameterTypes(parameters, script);

            endpointUrl = ReplaceRouteAndQueryParams(endpointUrl, parameters);
            endpointUrl += !endpointUrl.Contains('?') ? '?' : "";

            if (classMethod.Arguments.Any(arg => arg.Name == "includes?"))
            {
                if (IsContentType(resolvedReturnType))
                {
                    endpointUrl += "&includes=' + includes + '";
                }
                else if (IsEntity(resolvedReturnType))
                {
                    endpointUrl += "&includes=' + includes + '";
                }
            }

            endpointUrl = endpointUrl.Replace("?&", "?");

            if (endpointUrl.EndsWith('?'))
            {
                endpointUrl = endpointUrl[0..^1];
            }

            var caller = ResolveCaller(resolvedReturnType, returnsList);

            classMethod.Injected = GenerateCall(caller, endpointUrl, bodyParam, parameters, GetHttpMethodFromAttribute(method), classMethod);

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

		private static string ResolveCaller(Type resolvedReturnType, bool returnsList = false)
		{
			var ecmaService = Services.Get<EcmaService>();
			var listOfType = GetListOfType(resolvedReturnType);

			if (listOfType != null)
			{
				if (IsContentType(listOfType))
				{
					return "getList<" + ecmaService.GetTypeConversion(resolvedReturnType) + ">";
				}
				return "getJson<" + ecmaService.GetTypeConversion(resolvedReturnType) + "[]>";
			}
			else if (IsContentType(resolvedReturnType))
			{
				return "getOne<" + resolvedReturnType.Name + ">";
			}
			else if (resolvedReturnType == typeof(void))
			{
				return "getText";
			}
            else if (returnsList)
            {
                return "getList<" + ecmaService.GetTypeConversion(resolvedReturnType) + ">";
            }

			return "getJson<" + ecmaService.GetTypeConversion(resolvedReturnType) + ">";
		}

		private static List<string> GenerateCall(string caller, string endpointUrl, ParameterInfo bodyParam, ParameterInfo[] allParams, string requestMethod, ClassMethod classMethod)
        {
            var urlPart = string.IsNullOrEmpty(endpointUrl) ? "this.apiUrl" : $"this.apiUrl + '/{endpointUrl}'";

            if (classMethod.Arguments.Any(arg => arg.Type == "ApiIncludes[]"))
            {
                urlPart += "+ (includes.length != 0 ? '?includes=' + includes.map(include => include.toString()).join(', ') : '')";
            }
            List<string> inject = [];

            if (bodyParam != null)
            {
                inject.Add( 
                    $"return {caller}({urlPart}, {bodyParam.Name})"
                );

                if (classMethod.ReturnType == "Promise<SessionResponse>")
                {
                    inject.Add(".then((s: SessionResponse) => {");
                    inject.Add("\tsetSession(s);");
                    inject.Add("\treturn s;");
                    inject.Add("})");
                }
                return inject;
            }

            var multipleParams = allParams
                .Where(p => p.GetCustomAttribute<FromBodyAttribute>() != null)
                .ToList();

            if (multipleParams.Count > 1)
            {
                throw new Exception("Multiple [FromBody] attributes in a method is not permitted. The method was: " + classMethod.Name);
            }
            
            if (multipleParams.Count == 1)
            {
                inject.Add($"return {caller}({urlPart}, ");
                inject.Add(multipleParams[0].Name);
                inject.Add(", { method: '" + requestMethod + "' })");

                if (classMethod.ReturnType == "Promise<SessionResponse>")
                {
                    inject.Add(".then((s: SessionResponse) => {");
                    inject.Add("\tsetSession(s);");
                    inject.Add("\treturn s;");
                    inject.Add("})");
                }
                return inject;
            }
            
            inject.Add($"return {caller}({urlPart})");

            if (classMethod.ReturnType == "Promise<SessionResponse>")
            {
                inject.Add(".then((s: SessionResponse) => {");
                inject.Add("\tsetSession(s);");
                inject.Add("\treturn s;");
                inject.Add("})");
            }

            return inject;
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
