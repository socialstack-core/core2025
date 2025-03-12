using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Api.AvailableEndpoints;
using Api.CanvasRenderer;
using Api.Database;
using Api.EcmaScript.TypeScript;
using Api.Eventing;
using Api.Startup;
using Api.Users;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        private readonly Dictionary<Type, string> TypeConversions = [];
        
        /// <summary>
        /// Kinda ironic this generic huh?
        /// </summary>
        private readonly Dictionary<Type, Script> EntityScriptMapping = [];

        private readonly AvailableEndpointService endpointService;


        /// <summary>
        /// Constructor
        /// </summary>
        public EcmaService(AvailableEndpointService endpointService)
        {
            this.endpointService = endpointService;

            Events.Compiler.BeforeCompile.AddEventListener(async (ctx, sourceBuilders) => {
                
                // Create the typescript functionality before the JS is compiled.
                CreateTSSchema();

                return sourceBuilders;
            });
            Events.Compiler.AfterCompile.AddEventListener(async (ctx, sourceBuilders) => {

                // build everything 
                BuildTypescriptAliases(sourceBuilders);
                return sourceBuilders;
            });
        }

        private void CreateTSSchema()
        {
            InitTypeConversions();
            CreateBaseApi();
            InitTsScripts();
        }

        private Script GetScriptByEntity(Type entityType)
        {
            if (EntityScriptMapping.TryGetValue(entityType, out Script target))
            {
                return target;
            }
            var sct = new Script();

            EntityScriptMapping[entityType] = sct;
            return sct;
        }

        private void CreateBaseApi()
        {
            var apiScript = GetScriptByEntity(typeof(Content<>));

            apiScript.FileName = "TypeScript/Api/ApiEndpoints.tsx";

            apiScript.AddImport(new() {
                DefaultImport = "webRequest",
                From = "UI/Functions/WebRequest",
                Symbols = ["ApiSuccess", "ApiFailure"]
            });

            // ======== CONTENT.CS =========== \\
            var content = new TypeDefinition() {
                Name = "Content",
                GenericTemplate = "ID"
            };
            AddFieldsToType(typeof(Content<>), content);
            apiScript.AddChild(content);

            // ===== VERSIONEDCONTENT.CS ===== \\
            var versionedContent = new TypeDefinition() {
                Name = "VersionedContent",
                GenericTemplate = "T",
                Inheritence = ["UserCreatedContent<T>"]
            };
            AddFieldsToType(typeof(VersionedContent<>), versionedContent);
            versionedContent.AddProperty("revisionId", "T");
            apiScript.AddChild(versionedContent);

            // ===== USERCREATEDCONTENT.CS ===== \\
            var userGenContent = new TypeDefinition() {
                Name = "UserCreatedContent",
                GenericTemplate = "T",
                Inheritence = ["Content<T>"]
            };
            AddFieldsToType(typeof(UserCreatedContent<>), userGenContent);
            apiScript.AddChild(userGenContent);

            // ===== AutoAPI ===== \\
            var baseControllerClass = new ClassDefinition { 
                Name = "AutoApi",
                GenericTemplate = "EntityType extends VersionedContent<number>"
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

            apiScript.AddChild(baseControllerClass);

            // === SAVING TS FILE === \\

            File.WriteAllText("TypeScript/Api/ApiEndpoints.tsx", apiScript.CreateSource());
        }

        /// <summary>
        /// Adds the CRUD Functionality to a class.
        /// </summary>
        /// <param name="baseControllerClass"></param>
        private void AddCrudFunctionality(ClassDefinition baseControllerClass)
        {
            var listMethod = new ClassMethod
            {
                Name = "list",
                ReturnType = "Promise<ApiSuccess<EntityType[]> | ApiFailure>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "where",
                        Type = "Partial<Record<keyof(EntityType), string | number | boolean>>",
                        DefaultValue = "{}"
                    },
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "string[]",
                        DefaultValue = "[]"
                    }
                ], 
                Injected = [
                    "return webRequest(this.apiUrl + '/list', { where }, { method: 'POST', includes })"
                ]
            };
            var oneMethod = new ClassMethod() {
                Name = "load", 
                ReturnType = "Promise<ApiSuccess<EntityType> | ApiFailure>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "id",
                        Type = "number"
                    }
                ],
                Injected = [
                    "return webRequest(this.apiUrl + '/' + id)"
                ]
            };
            var createMethod = new ClassMethod() {
                Name = "create", 
                ReturnType = "Promise<ApiSuccess<EntityType> | ApiFailure>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entity",
                        Type = "EntityType"
                    }
                ],
                Injected = [
                    "return webRequest(this.apiUrl, entity)"
                ]
            };
            var updateMethod = new ClassMethod() {
                Name = "update", 
                ReturnType = "Promise<ApiSuccess<EntityType> | ApiFailure>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entity",
                        Type = "EntityType"
                    }
                ],
                Injected = [
                    "return webRequest(this.apiUrl + '/' + entity.id, entity)"
                ]
            };

            var deleteMethod = new ClassMethod() {
                Name = "delete",
                ReturnType = "Promise<ApiSuccess<EntityType> | ApiFailure>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entityId",
                        Type = "number" 
                    }
                ], 
                Injected = [
                    "return webRequest(this.apiUrl + '/' + entityId, {} , { method: 'DELETE', includes: [] })"
                ]
            };

            var constructorMethod = new ClassMethod() {
                Name = "constructor",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "apiUrl", 
                        Type = "string"
                    }
                ],
                Injected = [
                    "this.apiUrl = apiUrl;"
                ]
            };
            
            baseControllerClass.Children.Add(constructorMethod);
            baseControllerClass.Children.Add(listMethod);
            baseControllerClass.Children.Add(oneMethod);
            baseControllerClass.Children.Add(createMethod);
            baseControllerClass.Children.Add(updateMethod);
            baseControllerClass.Children.Add(deleteMethod);

        }

        private void AddFieldsToType(Type source, TypeDefinition target)
        {
            foreach (var field in source.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // Skip compiler-generated backing fields (e.g., auto-properties)
                if (Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute)))
                {
                    continue;
                }
                if (field.IsStatic)
                {
                    continue;
                }
                var fieldName = field.Name;

                if (fieldName[0] == '_')
                {
                    fieldName = fieldName[1..];
                }

                fieldName = LcFirst(fieldName);

                var type = field.FieldType;

                var nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                {
                    type = nullableType;
                }

                // Add only actual fields (not property backers)
                target.AddProperty(fieldName, GetTypeConversion(type));
            }
        }


        private void InitTsScripts()
        {
            var allEndpointsByModule = endpointService.ListByModule();

            foreach(var module in allEndpointsByModule){

                // module.Endpoints - is what you expect, all the endpoints that are present on e.g. /v1/locale/* for example

                var controllerType = module.GetAutoControllerType();
                var controller     = module.ControllerType;
                var entityType     = module.GetContentType();

                if (controllerType is null && entityType is null)
                {
                    continue;
                }

                var fields = module.GetAutoService()?.GetContentFields().List;
                var coreImports = new List<string>() {
                    "AutoApi"
                };

                Script script = new();
                script.AddImport(new() {
                    Symbols = coreImports,
                    From = "Api/ApiEndpoints"
                });

                if (fields == null)
                {
                    Console.WriteLine(controller.Name);
                }

                // === Convert the entity to a TS type === \\
                if (entityType is not null && fields != null)
                {
                    // create entity type.

                    if (string.IsNullOrEmpty(script.FileName))
                    {
                        script.FileName = "TypeScript/Api/" + entityType.Name + ".tsx";
                    }

                    var typeDef = new TypeDefinition();
                    typeDef.SetName(entityType.Name);
                    
                    // needs to be done like this, VersionedContent is a generic class, it will always contain a `
                    var baseName = entityType.BaseType.Name[..entityType.BaseType.Name.LastIndexOf('`')];

                    coreImports.Add(
                        baseName
                    );

                    if (entityType.BaseType.GenericTypeArguments.Length != 0)
                    {
                        typeDef.AddInheritence(baseName + "<" + GetTypeConversion(entityType.BaseType.GenericTypeArguments[0]) + ">");
                    }

                    AddFieldsToType(fields, typeDef);

                    script.AddChild(typeDef);
                }

                // === End of entity conversion === \\
                
                if ( controller != null )
                {
                    
                    if (string.IsNullOrEmpty(script.FileName))
                    {
                        script.FileName = "TypeScript/Api/" + controller.Name[..controller.Name.LastIndexOf("Controller")] + ".tsx";
                    }

                    var baseUrl = controller.GetCustomAttribute<RouteAttribute>();

                    if (baseUrl is null)
                    {
                        continue;
                    }

                    var controllerDef = new ClassDefinition() {
                        Name = entityType.Name + "Api",
                        Extends = "AutoApi<" + entityType.Name + ">"
                    };

                    controllerDef.Children.Add(new ClassMethod() {
                        Name = "constructor", 
                        Injected = [
                            $"super('{baseUrl.Template}')"
                        ]
                    });

                    script.AddChild(controllerDef);
                    script.AddSLOC($"export default new {controllerDef.Name}();");

                }


                File.WriteAllText(script.FileName, script.CreateSource());

            }
            
        }

        private void AddFieldsToType(List<ContentField> fields, TypeDefinition typeDef)
        {
            foreach(var field in fields)
            {
                var targetType = field.FieldType;
                if (Nullable.GetUnderlyingType(targetType) != null)
                {
                    targetType = Nullable.GetUnderlyingType(targetType);
                }
                var jsonIgnore = field.PropertyInfo?.GetCustomAttribute<JsonIgnoreAttribute>();

                jsonIgnore ??= field.FieldInfo?.GetCustomAttribute<JsonIgnoreAttribute>();

                if (jsonIgnore is not null)
                {
                    continue;
                }
                typeDef.AddProperty(field.Name, GetTypeConversion(targetType));
            }
        }

        private void InitTypeConversions()
        {
            AddTypeConversion(typeof(string), "string");
            AddTypeConversion(typeof(uint), "number");
            AddTypeConversion(typeof(int), "number");
            AddTypeConversion(typeof(double), "number");
            AddTypeConversion(typeof(float), "number");
            AddTypeConversion(typeof(ulong), "number");
            AddTypeConversion(typeof(long), "number");
            AddTypeConversion(typeof(DateTime), "Date");
            AddTypeConversion(typeof(bool), "boolean");
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

        /// <summary>
        /// Converts entity properties to have LCFirst names.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LcFirst(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}