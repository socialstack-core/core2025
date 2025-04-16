

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
        private readonly static Type[] unhandleableTypes = [
            typeof(void),
            typeof(ValueTask),
            typeof(Task)
        ];
        
        public static void AddControllerMethods(Type controller, ClassDefinition target, string baseUrl, Script script)
        {
            
            var ecmaService = Services.Get<EcmaService>();

            foreach(var method in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (!IsEndpoint(method))
                {
                    continue;
                }

                Type returnType = null; 

                var returnsAttr = method.GetCustomAttribute<ReturnsAttribute>();

                if (returnsAttr is not null)
                {
                    returnType = GetResolvedType(returnsAttr.ReturnType);
                }
            
                returnType ??= GetResolvedType(method.ReturnType);
                
                var isCollection = IsList(returnType);

                if (isCollection)
                {
                    script.AddImport(new() {
                        Symbols = ["ApiList"],
                        From = "UI/Functions/WebRequest"
                    });

                    returnType = returnType.GetGenericArguments()[0];
                }

                if (unhandleableTypes.Contains(returnType))
                {
                    continue;
                }

                // lets ensure the returnType exists.

                if (!TypeDefinitionExists(returnType.Name))
                {
                    if (returnType.Name != "Context" && !ecmaService.TypeConversions.ContainsKey(returnType))
                    {
                        if (!IsEntity(returnType)) {
                            // it can be a structure for instance.
                            // we add it to the same class.
                            // then we must create it. 
                            if ( !ignoreParamTypes.Contains(returnType))
                            {
                                var newType = OnNonEntity(returnType, script);
                                script.AddTypeDefinition(newType);
                            }
                        } else {
                            
                            // since its an entity, its guaranteed to exist
                            // even if it doesn't at this exact 
                            // point in time, it will do.

                            var existing = script.Children.OfType<TypeDefinition>().Where(child => child.Name == returnType.Name);

                            if (!existing.Any())
                            {
                                script.AddImport(new() {
                                    Symbols = [returnType.Name],
                                    From = "./" + returnType.Name
                                });
                            }
                        }
                    }
                } else {

                    // it already exists, sweeeet.
                    script.AddImport(new() {
                        Symbols = [returnType.Name],
                        From = "./" + returnType.Name
                    });
                }

                ClassMethod apiMethod = null;

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
                    apiMethod = new ClassMethod() {
                        Name = LcFirst(method.Name),
                        ReturnType = (
                            returnType.Name == "Context" ? 
                            "Promise<SessionResponse>" :
                            (isCollection ? "Promise<ApiList<" + returnType.Name + ">>" : $"Promise<{returnType.Name}>")
                        )
                    };

                }

                // adds all the params, creates the required types too. 
                AddMethodParams(method, apiMethod, script);

                // adds the method body
                AddMethodBody(method, apiMethod, script, baseUrl);
                
                target.AddMethod(apiMethod);     
            }

        }

    }
}