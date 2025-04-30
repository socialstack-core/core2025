using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Api.AvailableEndpoints;
using Api.CanvasRenderer;
using Api.Contexts;
using Api.EcmaScript.Markdown;
using Api.EcmaScript.TypeScript;
using Api.Eventing;
using Api.Startup;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Api.EcmaScript
{
    /// <summary>
    /// Handles both JS &amp; TS generation.
    /// </summary>
    public partial class EcmaService : AutoService
    {
        /// <summary>
        /// Used for things like uint => number, int => number, 
        /// </summary>
        public readonly Dictionary<Type, string> TypeConversions = [];
        
        private readonly AvailableEndpointService endpointService;

        private Script IncludesScript = new() {
            FileName = "TypeScript/Api/Includes.tsx"
        };

        /// <summary>
        /// A list of types to ignore when converting method parameters to TS
        /// </summary>  
        public static readonly Type[] methodParamIgnoreTypes = [
            typeof(HttpContext),
            typeof(Context)
        ];


        /// <summary>
        /// Constructor
        /// </summary>
        public EcmaService(AvailableEndpointService endpointService)
        {
            this.endpointService = endpointService;

            Events.Compiler.BeforeCompile.AddEventListener((ctx, source) => {

                // Create the typescript functionality before the JS is compiled.

                // Create if needed (noop otherwise)
                Directory.CreateDirectory("TypeScript/Api");

                // Create a container to hold the API/*.tsx files.
                // It exists such that ultimately the UI bundle compiles files present here as well.
                var container = new SourceFileContainer(Path.GetFullPath("TypeScript/Api"), "Api");
            

				CreateTSSchema(container);
				BuildTypescriptAliases(source.Bundles);

                // Add the container to the UI bundle:
                var uiBundle = source.GetBundle("UI");

                if (uiBundle != null)
                {
                    uiBundle.AddContainer(container);
                }

				return ValueTask.FromResult(source);
			});

            Events.Compiler.OnMapChange.AddEventListener((ctx, sourceBuilders) =>
            {
				// Called when 1 file has changed.
				// Need to make sure the global.ts file is correct
                // (the C# api won't have changed whilst the api is running, so no other files must regenerate).
				BuildTypescriptAliases(sourceBuilders);

				return ValueTask.FromResult(sourceBuilders);
            });
		}

        

        private void InitTsScripts(SourceFileContainer sourceContainer)
        {
            var allEndpointsByModule = endpointService.ListByModule();
            var def = new ClassDefinition() {
                Name = "ApiIncludes",
                Children = [
                    new ClassProperty() {
                        PropertyName = "text",
                        PropertyType = "string",
                        DefaultValue = ""
                    },
                    new ClassMethod() {
                        Name = "constructor",
                        Arguments = [
                            new ClassMethodArgument() {
                                Name = "chain",
                                Type = "string",
                                DefaultValue = ""
                            },
                            new ClassMethodArgument() {
                                Name = "includeName", 
                                Type = "string",
                                DefaultValue = ""
                            }
                        ],
                        Injected = [
                            "if(chain && includeName){",
                            "this.text = chain + '.' + includeName;",
                            "}else{",
                            "this.text = chain || includeName;",
                            "}"
                        ]
                    },
                    new ClassMethod() {
                        Name = "toString", 
                        Injected = [
                            "return this.text"
                        ]
                    }
                ]
            };

            // now for all global virtual fields.

            foreach(var entry in ContentFields.GlobalVirtualFields)
            {
                var field = entry.Key;
                var contentFieldInfo = entry.Value;


                if (contentFieldInfo.IsVirtual)
                {
                    def.Children.Add(new ClassGetter() {
                        Name = LcFirst(field),
                        ReturnType = "ApiIncludes",
                        Source = [
                            "return new ApiIncludes(this.text, '" + LcFirst(field) + "');"
                        ]
                    });
                }
            }

            IncludesScript.AddChild(
                def
            );

            SourceGenerator.EnsureScript(IncludesScript);
            
            foreach(var module in allEndpointsByModule){

                // module.Endpoints - is what you expect, all the endpoints that are present on e.g. /v1/locale/* for example

                var controllerType = module.GetAutoControllerType();
                var controller     = module.ControllerType;
                var entityType     = module.GetContentType();

                if (entityType is not null)
                {
                    var script = new Script() {
                        FileName = "TypeScript/Api/" + entityType.Name + ".tsx" 
                    };

                    SourceGenerator.EnsureScript(script);

                    try
                    {
                        MarkdownGeneration.OnImport(entityType);
                        MarkdownGeneration.AddUsage(entityType);

                        script.AddTypeDefinition(
                            SourceGenerator.OnEntity(entityType, script, module)
                        );
                        MarkdownGeneration.OnEntity(entityType, module);
                        script.AddChild(
                            SourceGenerator.OnEntityController(entityType, controller, script)
                        );
                        MarkdownGeneration.OnController(controllerType, entityType, module);
                    
                        SourceGenerator.CreateIncludeClass(entityType, IncludesScript);

                        var importedSelf = script.Imports.Where(import => import.From == "./" + entityType.Name);

                        if (importedSelf.Any())
                        {
                            script.Imports.Remove(
                                importedSelf.First()
                            );
                        }

                        script.AddImport(new() {
                            Symbols = ["getOne", "getList", "getJson", "getText"],
                            From = "UI/Functions/WebRequest"
                        });

                        script.AddSLOC("export default new " + entityType.Name + "Api();");

                        // we also have to get the includes script and
                        // add custom includes to that.
                    }
                    catch(Exception e)
                    {
                        Console.Write(e.StackTrace);
                    }
                
                }
                else
                {
                    var script = new Script() {
                        FileName = "TypeScript/Api/" + controller.Name.Replace("Controller", "") + ".tsx"
                    };
                    
                    SourceGenerator.EnsureScript(script);

                    try{
                        
                        MarkdownGeneration.OnImport(controller);
                        
                        script.AddChild(
                            SourceGenerator.OnNonEntityController(controller, script)
                        );

                        MarkdownGeneration.OnController(controller, module);

                        
                        script.AddImport(new() {
                            Symbols = ["getOne", "getList", "getJson", "getText"],
                            From = "UI/Functions/WebRequest"
                        });
                        
                        script.AddSLOC("export default new " + controller.Name.Replace("Controller", "") + "Api();");
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.Write(e.StackTrace);
                    }
                }

            }

            SourceGenerator.Validate();

            SourceGenerator.scripts.ForEach(script => {
                var source = script.CreateSource();
                sourceContainer.Add(script.FileName, source);
                File.WriteAllText(script.FileName, source);
            });

            MarkdownGeneration.GenerateMarkdown();
            
        }


        private void CreateTSSchema(SourceFileContainer container)
        {
            ContextGenerator.SaveToFile("TypeScript/Config/Session.tsx");
			InitTypeConversions();
            try
            {
                CreateBaseApi(container);
                InitTsScripts(container);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Write(e.StackTrace);
            }
            container.Add(IncludesScript.FileName, IncludesScript.CreateSource());
        }


        private void InitTypeConversions()
        {
            AddTypeConversion(typeof(string), "string");
            AddTypeConversion(typeof(uint), "uint");
            AddTypeConversion(typeof(int), "int");
            AddTypeConversion(typeof(double), "double");
            AddTypeConversion(typeof(float), "float");
            AddTypeConversion(typeof(ulong), "ulong");
            AddTypeConversion(typeof(long), "long");
            AddTypeConversion(typeof(short), "short");
            AddTypeConversion(typeof(ushort), "ushort");
            AddTypeConversion(typeof(byte), "byte");
            AddTypeConversion(typeof(sbyte), "sbyte");
            AddTypeConversion(typeof(DateTime), "Date");
            AddTypeConversion(typeof(bool), "boolean");
            AddTypeConversion(typeof(void), "void");
            AddTypeConversion(typeof(object), "Record<string, string | number | boolean>");
            AddTypeConversion(typeof(Context), "SessionResponse");
            AddTypeConversion(typeof(JObject), "Record<string, string | number | boolean>");
            AddTypeConversion(typeof(byte[]), "int[]");
        }

       
        /// <summary>
        /// Add a type equivalent for JS for the output.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="jsEquivalent"></param>
        public EcmaService AddTypeConversion(Type t, string jsEquivalent)
        {
            TypeConversions[t] = jsEquivalent;
            return this;
        }

        /// <summary>
        /// Returns the JS equivalent for a CS type, when not known returns unknown
        /// which is an accepted TS keyword.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetTypeConversion(Type t)
        {
            if (TypeConversions.TryGetValue(t, out string jsEquivalent))
            {
                return jsEquivalent;
            }
            return t.Name;
        }
    }
}