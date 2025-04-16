using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Api.EcmaScript.TypeScript;
using Api.Startup;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        // Types that cannot be handled
        private static readonly Type[] unhandleableTypes = {
            typeof(void),
            typeof(ValueTask),
            typeof(Task)
        };

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

                Type returnType = null;

                // Handle the custom Returns attribute for return type
                var returnsAttr = method.GetCustomAttribute<ReturnsAttribute>();
                if (returnsAttr != null)
                {
                    returnType = GetResolvedType(returnsAttr.ReturnType);
                }

                returnType ??= GetResolvedType(method.ReturnType);

                // Check if the return type is a collection (List or similar)
                var isCollection = IsList(returnType);

                if (isCollection)
                {
                    // Import ApiList when handling collections
                    script.AddImport(new Import
                    {
                        Symbols = [ "ApiList" ],
                        From = "UI/Functions/WebRequest"
                    });

                    returnType = returnType.GetGenericArguments()[0]; // Get the type inside the collection
                }

                // Skip unhandleable types (like void, ValueTask, etc.)
                if (unhandleableTypes.Contains(returnType))
                {
                    continue;
                }

                // Ensure the return type is defined or generate it if necessary
                if (!TypeDefinitionExists(returnType.Name))
                {
                    if (returnType.Name != "Context" && !ecmaService.TypeConversions.ContainsKey(returnType))
                    {
                        if (!IsEntity(returnType))
                        {
                            // It's an undiscovered type, we generate it
                            if (!ignoreParamTypes.Contains(returnType))
                            {
                                var newType = OnNonEntity(returnType, script);
                                script.AddTypeDefinition(newType);
                            }
                        }
                        else
                        {
                            // It's an entity, so it will be handled automatically
                            var existing = script.Children.OfType<TypeDefinition>()
                                .FirstOrDefault(child => child.Name == returnType.Name);

                            if (existing == null)
                            {
                                script.AddImport(new Import
                                {
                                    Symbols = [ returnType.Name ],
                                    From = $"./{returnType.Name}"
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
                        Symbols = [ returnType.Name ],
                        From = $"./{returnType.Name}"
                    });
                }

                // Create the ClassMethod for the API endpoint
                ClassMethod apiMethod = null;

                // Special case for handling ContentStream<,> return type
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ContentStream<,>))
                {
                    apiMethod = new ClassMethod
                    {
                        Name = LcFirst(method.Name),
                        ReturnType = "Promise<Record<string, string | boolean | number>>"
                    };
                }
                else
                {
                    // Handle other return types, including collections and Context
                    apiMethod = new ClassMethod
                    {
                        Name = LcFirst(method.Name),
                        ReturnType = returnType.Name == "Context"
                            ? "Promise<SessionResponse>"
                            : isCollection
                                ? $"Promise<ApiList<{returnType.Name}>>"
                                : $"Promise<{returnType.Name}>"
                    };
                }

                // Add the method parameters
                AddMethodParams(method, apiMethod, script);

                // Add the method body
                AddMethodBody(method, apiMethod, script, baseUrl);

                // Add the method to the class
                target.AddMethod(apiMethod);
            }
        }
    }
}
