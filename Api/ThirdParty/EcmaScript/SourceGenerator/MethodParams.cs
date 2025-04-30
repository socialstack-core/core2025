using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Api.Contexts;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
		/// <summary>
		/// Types to be ignored when processing method parameters
		/// </summary>
		public static readonly Type[] ignoreParamTypes = {
            typeof(HttpContext),
            typeof(Context)
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="classMethod"></param>
        /// <param name="script"></param>
        /// <param name="acceptsPartial"></param>
        public static void AddMethodParams(MethodInfo method, ClassMethod classMethod, Script script, bool acceptsPartial = false)
        {
            var ecmaService = Services.Get<EcmaService>();

            
            if (classMethod.ReturnType == "Promise<SessionResponse>")
            {
                classMethod.Arguments.Add(new ClassMethodArgument() {
                    Name = "setSession", 
                    Type = "(s: SessionResponse) => Session"
                });
            }

            // Iterate over method parameters
            foreach (var param in method.GetParameters())
            {
                var type = GetResolvedType(param.ParameterType);

                if (acceptsPartial && type == typeof(JObject))
                {
                    classMethod.Arguments.Add(new ClassMethodArgument() {
                        Name = param.Name,
                        Type = "Partial<T>"
                    });
                    continue;
                }

                // Skip ignored types like HttpContext or Context
                if (ignoreParamTypes.Contains(type))
                {
                    continue;
                }

                // Handle ContentStream<,> type
                if (type == typeof(ContentStream<,>))
                {
                    classMethod.Arguments.Add(new ClassMethodArgument
                    {
                        Name = param.Name,
                        Type = "Record<string, any>",
                        DefaultValue = "{}"
                    });
                    continue;
                }

                // Handle mapped types from TypeConversions
                if (ecmaService.TypeConversions.TryGetValue(type, out string mappedType))
                {
                    classMethod.Arguments.Add(new ClassMethodArgument
                    {
                        Name = param.Name,
                        Type = mappedType,
                        DefaultValue = param.DefaultValue?.ToString() ?? ""
                    });
                    continue;
                }

                // This is an undiscovered type, either an entity or unknown type
                if (!TypeDefinitionExists(type.Name))
                {
                    if (IsEntity(type))
                    {
                        // Add import for entities
                        script.AddImport(new Import
                        {
                            Symbols = [ type.Name ],
                            From = $"./{type.Name}"
                        });
                    }
                    else
                    {
                        // Handle unknown types by generating them
                        if (!ecmaService.TypeConversions.ContainsKey(type))
                        {
                            var newType = OnNonEntity(type, script);
                            script.AddTypeDefinition(newType);
                        }
                    }
                }
                else
                {
                    // Type is already defined, add import from the existing script
                    var containingScript = GetScriptByContainingTypeDefinition(type.Name);
                    script.AddImport(new Import
                    {
                        Symbols = [ type.Name ],
                        From = $"./{Path.GetFileName(containingScript.FileName).Replace(".tsx", "")}"
                    });
                }

                // Add parameter to the method
                classMethod.Arguments.Add(new ClassMethodArgument
                {
                    Name = param.Name,
                    Type = type.Name,
                    DefaultValue = param.DefaultValue?.ToString() ?? ""
                });
            }
        }
    }
}
