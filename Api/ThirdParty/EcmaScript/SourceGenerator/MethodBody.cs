

using System;
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

            var endpointUrl = GetEndpointUrl(method);

            if (endpointUrl is null) 
            {
                endpointUrl = baseUrl;
                return;
            }
            var hasBodyParam = false;

            // first iterate any params that are injected via url params
            foreach(var param in method.GetParameters())
            {

                var paramType = GetResolvedType(param.ParameterType);
                if (!TypeDefinitionExists(paramType.Name))
                {
                    if (IsEntity(paramType))
                    {
                        script.AddImport(new() {
                            Symbols = [param.Name],
                            From = "./" + param.Name
                        });
                    }
                    else
                    {
                        if (!ecmaService.TypeConversions.ContainsKey(paramType))
                        {
                            if (!ignoreParamTypes.Contains(paramType))
                            {
                                // needs to be created. 
                                var generated = OnNonEntity(paramType, script);
                                script.AddTypeDefinition(generated);    
                            }
                            
                        }
                        
                    }
                }

                if (endpointUrl.Contains($"{{{param.Name}}}"))
                {
                    endpointUrl = endpointUrl.Replace($"{{{param.Name}}}", "' + " + param.Name + " + '");
                }
                if (param.Name == "body")
                {
                    hasBodyParam = true;
                }
                if (param.GetCustomAttribute<FromQueryAttribute>() != null)
                {
                    if (!endpointUrl.Contains('?'))
                    {
                        endpointUrl += "?";
                    }

                    endpointUrl += $"&{param.Name}=' + {param.Name} + '";
                }
            }
            endpointUrl = endpointUrl.Replace("?&", "?");

            if (hasBodyParam)
            {
                classMethod.Injected = ["return getJson(this.apiUrl + '/" + endpointUrl + "', body )"];
            }
            else
            {
                var targetParams = method.GetParameters().Where(param => param.GetCustomAttribute<FromBodyAttribute>() != null);
                var targetCount = targetParams.Count();

                if (targetCount > 1)
                {
                    classMethod.Injected = [
                        "return getJson(this.apiUrl + '/" + endpointUrl + "', {" ,
                    ];
                    foreach (var param in targetParams)
                    {
                        classMethod.Injected.Add($"{param.Name},");
                    }

                    classMethod.Injected.Add("})");
                }
                else if (targetCount == 1)
                {
					classMethod.Injected = [
						"return getJson(this.apiUrl + '/" + endpointUrl + "', {  ",
                        targetParams.First().Name,
                        "})"
					];
				}
                else
                {
                    classMethod.Injected = ["return getJson(this.apiUrl + '/" + endpointUrl + "')"];
                }
            }

            // add documentation
            var methodDocumentation = ecmaService.GetMethodDocumentation(method);

            if (methodDocumentation != null)
            {
                classMethod.AddTsDocLine(string.IsNullOrEmpty(methodDocumentation?.Summary) ? "No summary available" :  methodDocumentation.Summary?.Trim());
                
                if (methodDocumentation.Parameters is not null)
                {
                    foreach(var prop in methodDocumentation.Parameters)
                    {
                        classMethod.AddTsDocLine("@param {" + prop.Key + "} - " + prop.Value);
                    }
                }
            }
        }
    }
}