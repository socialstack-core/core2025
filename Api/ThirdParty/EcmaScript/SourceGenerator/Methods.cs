using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Api.Contexts;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        public static void AddControllerMethods(Type controller, ClassDefinition target, string baseUrl, Script script)
        {
            var ecmaService = Services.Get<EcmaService>();

            // Iterate through all public instance methods declared in the controller
            foreach (var method in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (!IsEndpoint(method))
                {
                    continue;
                }

                Type resolvedReturnType = null;

                // Handle the custom Returns attribute for return type
                var returnsAttr = method.GetCustomAttribute<ReturnsAttribute>();
                if (returnsAttr != null)
                {
                    resolvedReturnType = GetResolvedType(returnsAttr.ReturnType);
                }

                resolvedReturnType ??= GetResolvedType(method.ReturnType);

                var recieves = method.GetCustomAttribute<ReceivesAttribute>();

                if (recieves is not null)
                {
                    // this is a marker that it can accept the target entity only having for instance 1 field.

                    var classMethod = new ClassMethod() {
                        Name = LcFirst(method.Name),
                        ReturnType = "Promise<T>"
                    };

                    // Add the method parameters
                    AddMethodParams(method, classMethod, script, true);

                    // Add the method body
                    AddMethodBody(method, classMethod, script, baseUrl, resolvedReturnType);

                    // Add the method to the class
                    target.AddMethod(classMethod);
                    continue;
                }

                // Check if the return type is a collection (List or similar)
                var collectionOfType = GetListOfType(resolvedReturnType);
                var isContentList = false;

                if (collectionOfType != null)
                {
                    // Import ApiList if it's a content type.
                    if (IsContentType(collectionOfType))
                    {
                        isContentList = true;

                        script.AddImport(new Import
                        {
                            Symbols = ["ApiList"],
                            From = "UI/Functions/WebRequest"
                        });
                        
                    }

                    resolvedReturnType = collectionOfType; // Get the type inside the collection
                }

                // Ensure the return type is defined or generate it if necessary
                if (resolvedReturnType == typeof(void))
                {
                    // Do nothing in this situation.
                }
                else if (!TypeDefinitionExists(resolvedReturnType.Name))
                {
                    if (resolvedReturnType.Name != "Context" && !ecmaService.TypeConversions.ContainsKey(resolvedReturnType))
                    {
                        if (!IsEntity(resolvedReturnType))
                        {
                            // It's an undiscovered type, we generate it
                            if (!ignoreParamTypes.Contains(resolvedReturnType))
                            {
                                var newType = OnNonEntity(resolvedReturnType, script);
                                script.AddTypeDefinition(newType);
                            }
                        }
                        else
                        {
                            // It's an entity, so it will be handled automatically
                            var existing = script.Children.OfType<TypeDefinition>()
                                .FirstOrDefault(child => child.Name == resolvedReturnType.Name);

                            if (existing == null)
                            {
                                script.AddImport(new Import
                                {
                                    Symbols = [ resolvedReturnType.Name ],
                                    From = $"./{resolvedReturnType.Name}"
                                });
                            }
                        }
                    }
                }
                else
                {
                    // Type already exists, just import it
                    script.AddImport(new Import
                    {
                        Symbols = [ resolvedReturnType.Name ],
                        From = $"./{resolvedReturnType.Name}"
                    });
                }

                // Create the ClassMethod for the API endpoint
                ClassMethod apiMethod = null;

                // Special case for handling ContentStream<,> return type
                if (resolvedReturnType.IsGenericType && resolvedReturnType.GetGenericTypeDefinition() == typeof(ContentStream<,>))
                {
                    var generics = resolvedReturnType.GetGenericArguments();
                    var actualType = generics[0];
                        
                    apiMethod = new ClassMethod
                    {
                        Name = LcFirst(method.Name),
                        ReturnType = $"Promise<{actualType.Name}>"
                    };
                }
                else
                {
                    // Handle other return types, including collections and Context
                    apiMethod = new ClassMethod
                    {
                        Name = LcFirst(method.Name)
					};

                    if (resolvedReturnType == typeof(void))
                    {
                        // uses getText
						apiMethod.ReturnType = "Promise<string>";
					}
                    else if (resolvedReturnType == typeof(Context))
                    {
                        apiMethod.ReturnType = "Promise<SessionResponse>";
                    }
                    else if (resolvedReturnType == typeof(object))
                    {
                        apiMethod.ReturnType = "Promise<any>";
                    }
                    else if (collectionOfType != null)
                    {
                        apiMethod.ReturnType = isContentList
                            ? $"Promise<ApiList<{collectionOfType.Name}>>"
                            : $"Promise<{collectionOfType.Name}[]>";
                    }
                    else
                    {
                        apiMethod.ReturnType = $"Promise<{resolvedReturnType.Name}>";
                    }
				}

                // Add the method parameters
                AddMethodParams(method, apiMethod, script);

                if (IsContentType(resolvedReturnType))
                {
                    apiMethod.Arguments.Add(new() {
                        Name = "includes?", 
                        DefaultValue = "",
                        Type = "ApiIncludes"
                    });
                }
                else if (IsEntity(resolvedReturnType))
                {
                    apiMethod.Arguments.Add(new() {
                        Name = "includes?", 
                        DefaultValue = "",
                        Type = "ApiIncludes"
                    });
                }

                // Add the method body
                AddMethodBody(method, apiMethod, script, baseUrl, resolvedReturnType);

                // Add the method to the class
                target.AddMethod(apiMethod);
            }
        }
    }
}
