

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Api.Contexts;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Microsoft.AspNetCore.Http;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {

        public static readonly Type[] ignoreParamTypes = [
            typeof(HttpContext),
            typeof(Context)
        ];

        public static void AddMethodParams(MethodInfo method, ClassMethod classMethod,  Script script)
        {
            var ecmaService = Services.Get<EcmaService>();
            

            foreach(var param in method.GetParameters())
            {
                var type = GetResolvedType(param.ParameterType);

                if (ignoreParamTypes.Contains(type))
                {
                    continue;
                }

                if (type == typeof(ContentStream<,>))
                {
                    classMethod.Arguments.Add(new() {
                        Name = param.Name,
                        Type = "Record<string, any>",
                        DefaultValue = "{}"
                    });
                    continue;
                }

                if (ecmaService.TypeConversions.TryGetValue(type, out string mappedType))
                {
                    classMethod.Arguments.Add(new() {
                        Name = param.Name,
                        Type = mappedType,
                        DefaultValue = param.DefaultValue.ToString()
                    });
                    continue;
                }

                // this is not a mapped type, so it will be either an entity, or a undiscovered type.

                if (!TypeDefinitionExists(type.Name))
                {
                    if (IsEntity(type))
                    {
                        // add the import
                        script.AddImport(new() {
                            Symbols = [type.Name],
                            From = "./" + type.Name
                        });
                    }
                    else
                    {
                        if (!ecmaService.TypeConversions.ContainsKey(type))
                        {
                            // needs to be created. 
                            var newType = OnNonEntity(type, script);
                            script.AddTypeDefinition(newType);
                        }
                    }
                }
                else
                {
                    var containingScript = GetScriptByContainingTypeDefinition(type.Name);

                    script.AddImport(new() {
                        Symbols = [type.Name],
                        From = "./" + Path.GetFileName(containingScript.FileName).Replace(".tsx", "")
                    });
                }

                classMethod.Arguments.Add(new() {
                    Name = param.Name,
                    Type = type.Name,
                    DefaultValue = param.DefaultValue.ToString()
                });
            }

        }

    }
}   