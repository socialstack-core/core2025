using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Api.AutoForms;
using Api.AvailableEndpoints;
using Api.CanvasRenderer;
using Api.Contexts;
using Api.Database;
using Api.EcmaScript.TypeScript;
using Api.Eventing;
using Api.Startup;
using Api.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
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
        private readonly Dictionary<Type, string> TypeConversions = [];
        
        /// <summary>
        /// Kinda ironic this generic huh?
        /// </summary>
        private readonly Dictionary<Type, Script> EntityScriptMapping = [];

        private readonly AvailableEndpointService endpointService;

        private Script IncludesScript;


        /// <summary>
        /// Constructor
        /// </summary>
        public EcmaService(AvailableEndpointService endpointService)
        {
            this.endpointService = endpointService;

            Events.Compiler.BeforeCompile.AddEventListener((ctx, source) => {

                // Create the typescript functionality before the JS is compiled.

                // Create a container to hold the API/*.tsx files.
                // It exists such that ultimately the UI bundle compiles files present here as well.
                var container = new SourceFileContainer(Path.GetFullPath("TypeScript/Api"), "Api");
                
                IncludesScript = new Script() {
                    FileName = "TypeScript/Api/Includes.tsx"
                };

                IncludesScript.AddImport(new() {
                    Symbols = ["ApiIncludes"],
                    From = "./ApiEndpoints"                    
                });

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

        private void CreateTSSchema(SourceFileContainer container)
        {
            ContextGenerator.SaveToFile("TypeScript/Config/Session.tsx");
			InitTypeConversions();
            CreateBaseApi(container);
            InitTsScripts(container);

            container.Add(IncludesScript.FileName, IncludesScript.CreateSource());
            CreateAutoControllerApi(container);
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

        private void CreateAutoControllerApi(SourceFileContainer container)
        {
            var controller = typeof(AutoFormController);
            var script = GetScriptByEntity(controller);

            script.FileName = "TypeScript/Api/AutoForm.tsx";

            script.AddImport(new() {
                From = "UI/Functions/WebRequest",
                Symbols = ["getJson"]
            });

            FromController(controller, script, null);
            container.Add(script.FileName, script.CreateSource());
            File.WriteAllText(script.FileName, script.CreateSource());
        }

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
            contentType.AddProperty("id", "number");
            contentType.AddProperty("type", "string");
            content.AddChild(contentType);

            // ===== VERSIONEDCONTENT.CS ===== \\
            var versionedContent = new TypeDefinition() {
                Name = "VersionedContent",
                Inheritence = ["UserCreatedContent"]
            };
            versionedContent.AddTsDocLine("* The base content type for all content.");
            AddFieldsToType(typeof(VersionedContent<>), versionedContent, content);
            versionedContent.AddProperty("revisionId", "number");
            content.AddChild(versionedContent);

            // ===== USERCREATEDCONTENT.CS ===== \\
            var userGenContent = new TypeDefinition() {
                Name = "UserCreatedContent",
                Inheritence = ["Content"]
            };
            userGenContent.AddTsDocLine("* The base content type for all entities users can create");
            AddFieldsToType(typeof(UserCreatedContent<>), userGenContent, content);
            content.AddChild(userGenContent);

            var generatedSource = content.CreateSource();
            container.Add(content.FileName, generatedSource);
			File.WriteAllText(content.FileName, generatedSource);
        }

        private void CreateBaseApi(SourceFileContainer container)
        {
            CreateBaseContentTsx(container);

            var apiScript = GetScriptByEntity(typeof(Content<>));

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
                GenericTemplate = "EntityType extends VersionedContent, IncludeSet extends ApiIncludes"
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
            AddBaseIncludeFunctionality(apiScript);

            apiScript.AddChild(baseControllerClass);

            // === SAVING TS FILE === \\

            var filePath = "TypeScript/Api/ApiEndpoints.tsx";
			var generatedSource = apiScript.CreateSource();
            container.Add(filePath, generatedSource);
            File.WriteAllText(filePath, generatedSource);
        }

        private void AddBaseIncludeFunctionality(Script script)
        {
            var classDef = new ClassDefinition() {
                Name = "ApiIncludes",
            };

            var property = new ClassProperty() {
                PropertyName = "text",
                PropertyType = "string",
                Visibility = "protected" // musn't be accessible outside of the class, but must be accessible to children classes
            };

            var constructor = new ClassMethod() {
                Name = "constructor",
                Arguments = [
                    new() {
                        Name = "prev?",
                        Type = "string"
                    },
                    new() {
                        Name = "extra?",
                        Type = "string"
                    }
                ],
                Injected = [
                    "this.text = (prev ? prev + '.' : '') + (extra || '');"
                ]
            };
            var toString = new ClassMethod() {
                Name = "toString",
                ReturnType = "string",
                Injected = [
                    "return this.text;"
                ]
            };
            
            classDef.Children.Add(constructor);
            classDef.Children.Add(property);
            classDef.Children.Add(toString);

            script.AddChild(classDef);
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
                ReturnType = "Promise<ApiList<EntityType>>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "where",
                        Type = "Partial<Record<keyof(EntityType), string | number | boolean>>",
                        DefaultValue = "{}"
                    },
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "IncludeSet[]",
                        DefaultValue = "[]"
                    }
                ], 
                Injected = [
                    "return getList(this.apiUrl + '/list', { where }, { method: 'POST', includes: includes.map(include => include.toString()) })"
                ]
            };
            var oneMethod = new ClassMethod() {
                Name = "load", 
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "id",
                        Type = "number"
                    },
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "IncludeSet[]",
                        DefaultValue = "[]"
                    }
                ],
                Injected = [
                    "return getOne(this.apiUrl + '/' + id, { includes: includes.map(include => include.toString()) })"
                ]
            };
            var createMethod = new ClassMethod() {
                Name = "create", 
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entity",
                        Type = "EntityType"
                    }
                ],
                Injected = [
                    "return getOne(this.apiUrl, entity)"
                ]
            };
            var updateMethod = new ClassMethod() {
                Name = "update", 
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entity",
                        Type = "EntityType"
                    }
                ],
                Injected = [
                    "return getOne(this.apiUrl + '/' + entity.id, entity)"
                ]
            };

            var deleteMethod = new ClassMethod() {
                Name = "delete",
                ReturnType = "Promise<EntityType>",
                Arguments = [
                    new ClassMethodArgument() {
                        Name = "entityId",
                        Type = "number" 
                    },                    
                    new ClassMethodArgument() {
                        Name = "includes",
                        Type = "IncludeSet[]",
                        DefaultValue = "[]"
                    }
                ], 
                Injected = [
                    "return getOne(this.apiUrl + '/' + entityId, {} , { method: 'DELETE', includes: includes.map(include => include.toString()) })"
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

            baseControllerClass.Children.Add(new ClassProperty() {
                PropertyName = "includes",
                PropertyType = "IncludeSet | null",
                DefaultValue = "null"
            });
            
            baseControllerClass.Children.Add(constructorMethod);
            baseControllerClass.Children.Add(listMethod);
            baseControllerClass.Children.Add(oneMethod);
            baseControllerClass.Children.Add(createMethod);
            baseControllerClass.Children.Add(updateMethod);
            baseControllerClass.Children.Add(deleteMethod);

        }

        private void AddFieldsToType(Type source, TypeDefinition target, Script script)
        {
            foreach (var field in source.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
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

                // Remove leading underscore and lowercase first letter
                if (fieldName[0] == '_')
                {
                    fieldName = fieldName[1..];
                }

                fieldName = LcFirst(fieldName);

                var type = field.FieldType;

                // Handle nullable types
                var nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                {
                    type = nullableType;
                }

                // Skip types already converted (to prevent duplicates or circular references)
                if (TypeConversions.TryGetValue(type, out string value))
                {
                    if (IsCollection(field)) 
                    {
                        target.AddProperty(fieldName, value + "[]");
                    }
                    else
                    {
                        target.AddProperty(fieldName, value);
                    }
                    continue;
                }

                // If it's a collection (array or generic collection), handle its item type
                if (IsCollection(field))
                {
                    var objectType = GetCollectionItemType(field);

                    if (TypeConversions.TryGetValue(objectType, out string collValue))
                    {
                        target.AddProperty(
                            fieldName,
                            collValue + "[]"
                        );
                    }
                    else
                    {
                        // If the object type is not yet in TypeConversions, generate it
                        if (script != null)
                        {
                            // Avoid processing duplicate or circular references
                            if (script.Children.Any(obj => obj.GetType() == typeof(TypeDefinition) && (obj as TypeDefinition).Name == objectType.Name))
                            {
                                continue;
                            }

                            // Generate the type definition recursively
                            var generatedType = new TypeDefinition() { Name = objectType.Name };
                            AddFieldsToType(objectType, generatedType, script);

                            TypeConversions[objectType] = generatedType.Name + "[]";
                            script.AddChild(generatedType);
                            target.AddProperty(fieldName, generatedType.Name + "[]");
                        }
                    }
                }
                else
                {
                    // If the field is not a collection, check if it's a known type
                    if (TypeConversions.TryGetValue(type, out string nonCollType))
                    {
                        target.AddProperty(fieldName, nonCollType);
                    }
                    else
                    {
                        // If the type is not yet processed, generate it
                        if (script != null)
                        {
                            // Avoid circular references by checking the script's existing types
                            if (script.Children.Any(obj => obj.GetType() == typeof(TypeDefinition) && (obj as TypeDefinition).Name == type.Name))
                            {
                                continue;
                            }

                            var generatedType = new TypeDefinition() { Name = type.Name };
                            AddFieldsToType(type, generatedType, script);

                            TypeConversions[type] = generatedType.Name + "[]";
                            script.AddChild(generatedType);
                            target.AddProperty(fieldName, generatedType.Name);
                        }
                    }
                }
            }
        }


        private static Type GetCollectionItemType(FieldInfo field)
        {
            // Check for arrays (e.g., int[] or string[])
            if (field.FieldType.IsArray)
            {
                return field.FieldType.GetElementType();
            }

            // Check for generic collections like List<T>, Dictionary<TKey, TValue>, etc.
            if (field.FieldType.IsGenericType)
            {
                var genericType = field.FieldType.GetGenericTypeDefinition();
                if (genericType == typeof(List<>) || genericType == typeof(IEnumerable<>))
                {
                    return field.FieldType.GetGenericArguments()[0];
                }
                if (genericType == typeof(Dictionary<,>))
                {
                    var valueType = field.FieldType.GetGenericArguments()[1];
                    return valueType;
                }
            }

            return null;  // In case we can't determine the type
        }

        private static bool IsCollection(FieldInfo field)
        {
            // Check if the field type is a collection (implements IEnumerable)
            return typeof(IEnumerable).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string);
        }

        private void InitTsScripts(SourceFileContainer sourceContainer)
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
                    "AutoApi",
                    "ApiIncludes"
                };

                Script script = new();
                script.AddImport(new() {
                    Symbols = coreImports,
                    From = "Api/ApiEndpoints"
                });

                // === Convert the entity to a TS type === \\
                if (entityType is not null && fields != null)
                {
                    // create entity type.

                    if (string.IsNullOrEmpty(script.FileName))
                    {
                        script.FileName = "TypeScript/Api/" + entityType.Name + ".tsx";
                    }

                    script.AddImport(new () {
                        Symbols = ["VersionedContent", "UserCreatedContent", "Content"],
                        From = "Api/Content"
                    });

                    script.AddImport(new() {
                        Symbols = [entityType.Name + "Includes"],
                        From = "./Includes"
                    });

                    var typeDef = new TypeDefinition();
                    typeDef.SetName(entityType.Name);
                    
                    var entityDocumentation = GetTypeDocumentation(entityType);

                    if (entityDocumentation != null)
                    {
                        typeDef.AddTsDocLine(entityDocumentation.Summary.Trim());
                    }
                    
                    // needs to be done like this, VersionedContent is a generic class, it will always contain a `
                    var baseName = entityType.BaseType.Name[..entityType.BaseType.Name.LastIndexOf('`')];

                    if (entityType.BaseType.GenericTypeArguments.Length != 0)
                    {
                        if (baseName == "VersionedContent" || baseName == "Content" || baseName == "UserCreatedContent")
                        {
                            // changed to align with WebRequest.tsx
                            typeDef.AddInheritence(baseName);
                        }
                        else
                        {
                            typeDef.AddInheritence(baseName + "<" + GetTypeConversion(entityType.BaseType.GenericTypeArguments[0]) + ">");
                        }
                    }

                    AddFieldsToType(fields, typeDef);

                    var virtualFields = entityType.GetCustomAttributes<HasVirtualFieldAttribute>();

                    ClassDefinition includeClass;

                    includeClass = new ClassDefinition() {
                        Name = entityType.Name + "Includes",
                        Extends = "ApiIncludes"
                    };


                    includeClass.AddTsDocLine("Allows custom chained includes inside the list & load methods.");

                    if (virtualFields.Any())
                    {
                        foreach(var virtualField in virtualFields)
                        {
                            includeClass.Children.Add(
                                new ClassGetter() {
                                    Name = LcFirst(virtualField.FieldName), 
                                    ReturnType = virtualField.Type.Name + "Includes",
                                    Source = [
                                        $"return new {virtualField.Type.Name}Includes(this.text, '{virtualField.FieldName}')"
                                    ]
                                }
                            );

                            if (virtualField.Type != entityType)
                            {
                                script.AddImport(new() {
                                    Symbols = [virtualField.Type.Name],
                                    From = "Api/" + virtualField.Type.Name
                                });
                                script.AddImport(new () {
                                    Symbols = [virtualField.Type.Name + "Includes"],
                                    From = "./Includes"
                                });
                            }
                            typeDef.AddProperty(virtualField.FieldName, GetTypeConversion(virtualField.Type));
                        }

                    }
                    
                    IncludesScript.AddChild(includeClass);
                    script.AddChild(typeDef);
                }

                // === End of entity conversion === \\
                
                if ( controller != null )
                {
                    

                    script.AddImport(new() {
                        Symbols = ["getJson", "ApiList"],
                        From = "UI/Functions/WebRequest"
                    });

                    var controllerDef = FromController(controller, script, entityType);
                    script.AddSLOC($"export default new {controllerDef.Name}();");

                }

                var generatedSource = script.CreateSource();
				sourceContainer.Add(script.FileName, generatedSource);
				File.WriteAllText(script.FileName, generatedSource);
                File.WriteAllText(IncludesScript.FileName, IncludesScript.CreateSource());
			}
            
        }

        private ClassDefinition FromController(Type controller, Script script, Type entityType)
        {
            if (entityType is null)
            {
                entityType = controller;
            }
            if (string.IsNullOrEmpty(script.FileName))
            {
                script.FileName = "TypeScript/Api/" + controller.Name[..controller.Name.LastIndexOf("Controller")] + ".tsx";
            }

            var baseUrl = controller.GetCustomAttribute<RouteAttribute>();

            if (baseUrl is null)
            {
                return null;
            }
            
            var crudOperations = new string[]{"List", "Load", "Create", "Update", "Delete"};
            
            var controllerDef = new ClassDefinition() {
                Name = entityType.Name + "Api",
                Extends = 
                    entityType != controller ? 
                    "AutoApi<" + entityType.Name + ", " + entityType.Name + "Includes>" :
                    null
            };

            controllerDef.AddTsDocLine("Auto generated API for " + entityType.Name);

            var controllerDocumentation = GetTypeDocumentation(controller);

            if (controllerDocumentation is not null)
            {
                controllerDef.AddTsDocLine(controllerDocumentation.Summary.Trim());
            }

            var url = baseUrl.Template.StartsWith("v1/") ? baseUrl.Template[3..] : baseUrl.Template;

            if (entityType != controller)
            {
                controllerDef.Children.Add(new ClassMethod() {
                    Name = "constructor", 
                    Injected = [
                        $"super('{url}')",
                        "this.includes = new " + entityType.Name + "Includes();"
                    ],
                    Documentation = ["This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller"]
                });
            }
            else
            {
                controllerDef.Children.Add(new ClassProperty() {
                    PropertyName = "apiUrl",
                    PropertyType = "string",
                    Visibility = "protected",
                    DefaultValue = $"{url}"
                });
            }

            foreach(var method in controller.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                if (crudOperations.Contains(method.Name))
                {
                    continue;
                }
                if (!IsEndpoint(method))
                {
                    continue;
                }
                if (
                    controllerDef.Children.Find(
                        child => child.GetType() == typeof(ClassMethod) && 
                                (child as ClassMethod).Name == LcFirst(method.Name))
                    != null
                )
                {
                    continue;
                }

                // maybe a chance that some of the params have structures that need creating (i.e UserLogin)

                var returnType = GetTrueMethodReturnType(method);

                if (
                    returnType == typeof(Context) ||
                    returnType == typeof(object) || 
                    returnType == entityType || 
                    returnType == typeof(void)
                )
                {
                    // noop
                }
                else if (
                    returnType.BaseType == typeof(Content<>) || 
                    returnType.BaseType == typeof(VersionedContent<>) || 
                    returnType.BaseType == typeof(UserCreatedContent<>)
                )
                {

                    if (!script.Imports.Where(import => import.From == "Api/" + returnType.Name).Any())
                    {
                        // references another entity. 
                        script.AddImport(new() {
                            From = "Api/" + returnType.Name,
                            Symbols = [returnType.Name]
                        });
                    }
                }
                else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(List<>))
                {  
                    // This will trigger for any List<T>, like List<int>, List<string>, etc.
                    var listEntityType = returnType.GetGenericArguments()[0];

                    if (
                        script.Imports.Find(
                            import => import.From == "Api/" + listEntityType.Name
                        ) == null
                    )
                    {
                        if (listEntityType.BaseType == typeof(Content<>) || 
                            listEntityType.BaseType == typeof(VersionedContent<>) || 
                            listEntityType.BaseType == typeof(UserCreatedContent<>))
                        {
                            script.AddImport(new() {
                                From = "Api/" + listEntityType.Name,
                                Symbols = [listEntityType.Name]
                            });
                        } else {
                            // generate a type for the type

                            if (!script.Children.Where(obj => obj.GetType() == typeof(TypeDefinition) && (obj as TypeDefinition).Name == listEntityType.Name).Any())
                            {
                                script.AddChild(CreateNonEntityType(listEntityType));
                            }
                        }
                    }
                }
                else
                {
                    if (!TypeConversions.ContainsKey(returnType))
                    {
                        // create entity type.
                        var def = new TypeDefinition() {
                            Name = returnType.Name
                        };
                        
                        var defDocs = GetTypeDocumentation(returnType);

                        if (defDocs is not null)
                        {
                            def.AddTsDocLine(defDocs.Summary.Trim());
                        }

                        AddFieldsToType(returnType, def, script);
                        script.AddChild(def);
                    }
                }

                foreach(var param in method.GetParameters())
                {
                    // create missing types.
                    var paramType = param.ParameterType;

                    if (!paramType.Namespace.StartsWith("Api."))
                    {
                        continue;
                    }

                    if (paramType.BaseType == typeof(Content<>) || 
                        paramType.BaseType == typeof(VersionedContent<>) || 
                        paramType.BaseType == typeof(UserCreatedContent<>))
                    {
                        script.AddImport(new() {
                            From = "Api/" + paramType.Name,
                            Symbols = [paramType.Name]
                        });
                    } else {
                        // generate a type for the type

                        if (!script.Children.Where(obj => obj.GetType() == typeof(TypeDefinition) && (obj as TypeDefinition).Name == paramType.Name).Any())
                        {
                            script.AddChild(CreateNonEntityType(paramType));
                        }
                    }

                }

                controllerDef.Children.Add(
                    ConvertToTsMethod(
                        method, 
                        returnType
                    )
                );
            }

            script.AddChild(controllerDef);
            return controllerDef;
        }


        private TypeDefinition CreateNonEntityType(Type listEntityType)
        {
            var type = new TypeDefinition() {
                Name = listEntityType.Name
            };

            var typeDocs = GetTypeDocumentation(listEntityType);

            if (typeDocs is not null)
            {
                type.AddTsDocLine(typeDocs.Summary.Trim());
            }

            foreach(var field in listEntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.PropertyType.Namespace.StartsWith("Api.") || TypeConversions.ContainsKey(field.PropertyType))
                {
                    type.AddProperty(field.Name, GetTypeConversion(field.PropertyType));
                }
            }

            foreach(var field in listEntityType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.FieldType.Namespace.StartsWith("Api.") || TypeConversions.ContainsKey(field.FieldType))
                {
                    type.AddProperty(field.Name, GetTypeConversion(field.FieldType));
                }
            }

            return type;
        }

        private bool IsEndpoint(MethodInfo method)
        {
            // Check if any of the MVC HTTP attributes exist on the method
            var httpAttributes = method.GetCustomAttributes(true)
                                    .OfType<Attribute>()
                                    .Any(attr => attr is HttpGetAttribute || 
                                                    attr is HttpPostAttribute || 
                                                    attr is HttpPutAttribute || 
                                                    attr is HttpDeleteAttribute || 
                                                    attr is HttpPatchAttribute ||
                                                    attr is RouteAttribute);

            return httpAttributes;
        }

        private ClassMethod ConvertToTsMethod(MethodInfo method, Type type)
        {
            List<ClassMethodArgument> Arguments = [];

            // make sure the variables are converted to JS variables
            var details = GetEndpointUrl(method).Replace("{", "${");

            var returnType =  $"Promise<{GetTypeConversion(type)}>";

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {  
                // This will trigger for any List<T>, like List<int>, List<string>, etc.
                type = type.GetGenericArguments()[0];

                returnType = $"Promise<ApiList<{GetTypeConversion(type)}>>";
            }
            
            bool foundBodyVarName = false;

            foreach(var param in method.GetParameters())
            {
                var arg = new ClassMethodArgument() {
                    Name = param.Name,
                    Type = GetTypeConversion(param.ParameterType)
                };

                if (arg.Name == "body")
                {
                    foundBodyVarName = true;
                }

                if (param.GetCustomAttribute<FromQueryAttribute>() != null)
                {
                    if (!details.Contains('?'))
                    {
                        details += "?";
                    }

                    details += $"&{param.Name}=${{{param.Name}}}";
                }

                Arguments.Add(arg);
            }

            details = details.Replace("?&", "?");

            var tsMethod = new ClassMethod() {
                Name = LcFirst(method.Name),
                ReturnType = returnType,
                Arguments = Arguments,
            };

            if (foundBodyVarName)
            {
                tsMethod.Injected = ["return getJson(`${this.apiUrl}/" + details + "`, { body })"];
            }
            else
            {
                // composite body build

                var targetParams = method.GetParameters().Where(param => param.GetCustomAttribute<FromBodyAttribute>() != null);

                if (targetParams.Any())
                {
                    tsMethod.Injected = [
                        "return getJson(`${this.apiUrl}/" + details + "`, { body: {" ,
                    ];
                    foreach(var param in targetParams)
                    {
                        tsMethod.Injected.Add($"{param.Name},");
                    }

                    tsMethod.Injected.Add("}})");
                }
                else
                {
                    tsMethod.Injected = ["return getJson(`${this.apiUrl}/" + details + "`)"];
                }
                
            }

            var methodDocumentation = GetMethodDocumentation(method);

            if (methodDocumentation != null)
            {
                tsMethod.AddTsDocLine(string.IsNullOrEmpty(methodDocumentation?.Summary) ? "No summary available" :  methodDocumentation.Summary?.Trim());
                
                if (methodDocumentation.Parameters is not null)
                {
                    foreach(var prop in methodDocumentation.Parameters)
                    {
                        tsMethod.AddTsDocLine("@param {" + prop.Key + "} - " + prop.Value);
                    }
                }
            }

            return tsMethod;
        }

        private string GetEndpointUrl(MethodInfo method)
        {
            // Check if the method has any of the relevant HTTP attributes
            var httpAttributes = method.GetCustomAttributes(true)
                                    .OfType<Attribute>()
                                    .Where(attr => attr is RouteAttribute || 
                                                    attr is HttpGetAttribute || 
                                                    attr is HttpPostAttribute || 
                                                    attr is HttpPutAttribute || 
                                                    attr is HttpDeleteAttribute || 
                                                    attr is HttpPatchAttribute)
                                    .ToList();

            // If no relevant attribute is found, return null or an appropriate default value
            if (!httpAttributes.Any())
            {
                return null;
            }

            // Extract URL from RouteAttribute or the HTTP method attributes
            foreach (var attr in httpAttributes)
            {
                // RouteAttribute has a property called 'Template' which contains the URL pattern
                if (attr is RouteAttribute routeAttribute)
                {
                    return routeAttribute.Template;
                }

                // For HttpGet, HttpPost, etc., the URL is usually passed in the constructor
                // If the attribute has a 'Template' property, we extract that
                if (attr is HttpMethodAttribute httpMethodAttribute && !string.IsNullOrEmpty(httpMethodAttribute.Template))
                {
                    return httpMethodAttribute.Template;
                }
            }

            // Return null or a default string if no URL is found
            return null;
        }

        private static Type GetTrueMethodReturnType(MethodInfo method)
        {
            Type returnType;

            // Check if the return type is a Task<T>
            if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                // If it's a Task<T>, get the generic argument (T)
                returnType = method.ReturnType.GetGenericArguments()[0];
            }
            // Check if the return type is a ValueTask<T>
            else if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                // If it's a ValueTask<T>, get the generic argument (T)
                returnType = method.ReturnType.GetGenericArguments()[0];
            }
            // Check if the return type is a non-generic Task
            else if (method.ReturnType == typeof(Task))
            {
                returnType = typeof(void);
            }
            // Check if the return type is a non-generic ValueTask
            else if (method.ReturnType == typeof(ValueTask))
            {
                returnType = typeof(void);
            }
            else
            {
                // For other types, just return the method's actual return type
                returnType = method.ReturnType;
            }

            // Check for any custom ReturnsAttribute if applicable
            var methodReturnType = method.GetCustomAttribute<ReturnsAttribute>();
            return methodReturnType?.ReturnType ?? returnType;
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
            AddTypeConversion(typeof(void), "void");
            AddTypeConversion(typeof(object), "Record<string, string | number | boolean>");
            AddTypeConversion(typeof(Context), "SessionResponse");
            AddTypeConversion(typeof(JObject), "Record<string, string | number | boolean>");
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