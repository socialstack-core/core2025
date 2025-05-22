using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.Contexts;
using Api.Startup;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Type = System.Type;

namespace Api.TypeScript.Objects
{
    public partial class EntityController : AbstractTypeScriptObject
    {
        private List<ControllerMethod> _methods = null;

        /// <summary>
        /// Collects and caches controller methods that should be exposed to the TypeScript API layer.
        /// </summary>
        /// <returns>A list of <see cref="ControllerMethod"/> objects describing each API endpoint.</returns>
        /// <remarks>
        /// This method scans declared instance methods on the target .NET controller type and selects only those
        /// that are decorated with HTTP route attributes and are not considered standard CRUD (Create, Read, Update, Delete, List).
        ///
        /// Each discovered method is parsed into a <see cref="ControllerMethod"/> structure, including route templates,
        /// parameter mapping, return types, and other request metadata (e.g., body vs. query params).
        /// </remarks>
        private List<ControllerMethod> GetEndpointMethods()
        {
            if (_methods is not null)
            {
                return _methods;
            }

            _methods = [];
            string[] crud = ["List", "Load", "Create", "Update", "Delete"];

            foreach (var method in _referenceTypes.Item1.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                // Skip known CRUD methods to avoid duplicates
                if (crud.Contains(method.Name))
                {
                    continue;
                }

                var methodAttributes = method.GetCustomAttributes();

                // Look for supported HTTP route attributes
                var httpAttribute = methodAttributes.FirstOrDefault(attr => attr is RouteAttribute 
                    or HttpGetAttribute 
                    or HttpPostAttribute 
                    or HttpPutAttribute 
                    or HttpDeleteAttribute 
                    or HttpPatchAttribute);

                if (httpAttribute is null)
                {
                    continue;
                }

                var returnsAttr = method.GetCustomAttribute<ReturnsAttribute>();
                var returnType = returnsAttr != null 
                    ? returnsAttr.ReturnType 
                    : TypeScriptService.UnwrapTypeNesting(method.ReturnType);

                var methodParams = method.GetParameters();
                var webSafeParams = new List<ParameterInfo>();

                var controllerMethod = new ControllerMethod
                {
                    TrueReturnType = returnType,
                    Method = method,
                    RequiresSessionSet = returnType == typeof(Context),
                    RequiresIncludes = methodParams.Any(p => p.ParameterType == _referenceTypes.Item2) || returnType == _referenceTypes.Item2,
                    IsApiList = TypeScriptService.IsNestedCollection(method.ReturnType),
                    SendsData = methodParams.Any(p => p.GetCustomAttribute<FromBodyAttribute>() is not null)
                };

                controllerMethod.RequestUrl = httpAttribute switch
                {
                    RouteAttribute routeAttr => routeAttr.Template,
                    HttpMethodAttribute httpAttr => httpAttr.Template,
                    _ => controllerMethod.RequestUrl ?? ""
                };
                controllerMethod.RequestUrl = controllerMethod.RequestUrl.ToLower();

                // Parse method parameters
                foreach (var param in methodParams)
                {
                    // Route-bound parameter
                    if (controllerMethod.RequestUrl!.Contains($"{{{param.Name}}}") &&
                        param.GetCustomAttribute<FromRouteAttribute>() is not null)
                    {
                        webSafeParams.Add(param);
                        controllerMethod.RequestUrl = controllerMethod.RequestUrl.Replace($"{{{param.Name}}}", $"' + {param.Name} + '");
                        continue;
                    }

                    // Query-bound parameter
                    if (param.GetCustomAttribute<FromQueryAttribute>() is not null)
                    {
                        webSafeParams.Add(param);

                        if (!controllerMethod.RequestUrl.Contains('?'))
                        {
                            controllerMethod.RequestUrl += '?';
                        }

                        controllerMethod.RequestUrl += $"&{param.Name}=' + {param.Name} + '";
                        continue;
                    }

                    // Body-bound parameter
                    if (param.GetCustomAttribute<FromBodyAttribute>() is not null)
                    {
                        webSafeParams.Add(param);
                        controllerMethod.SendsData = true;
                        controllerMethod.BodyParam = param;
                    }
                }

                // Clean up the query string if needed
                controllerMethod.RequestUrl = controllerMethod.RequestUrl!.Replace("?&", "?");
                controllerMethod.WebSafeParams = webSafeParams;

                _methods.Add(controllerMethod);
            }

            return _methods;
        }
    }

    /// <summary>
    /// Represents a parsed controller method intended for TypeScript code generation.
    /// </summary>
    public class ControllerMethod
    {
        /// <summary>
        /// Indicates whether this method requires access to the user's session via a session setter function.
        /// </summary>
        public bool RequiresSessionSet = false;

        /// <summary>
        /// The final formatted route URL (including dynamic parameters replaced with JS expressions).
        /// </summary>
        public string RequestUrl;

        /// <summary>
        /// The parameter passed in the request body, if applicable.
        /// </summary>
        public ParameterInfo BodyParam;

        /// <summary>
        /// Whether the method makes use of the `includes` system (e.g. for virtual fields).
        /// </summary>
        public bool RequiresIncludes = false;

        /// <summary>
        /// Indicates whether the method sends a payload in the request body.
        /// </summary>
        public bool SendsData = false;

        /// <summary>
        /// The actual .NET <see cref="MethodInfo"/> this controller method maps to.
        /// </summary>
        public MethodInfo Method;

        /// <summary>
        /// The unwrapped return type of the method, considering any `ReturnsAttribute` override.
        /// </summary>
        public Type TrueReturnType;

        /// <summary>
        /// Indicates whether the return type is a list or collection.
        /// </summary>
        public bool IsApiList = false;

        /// <summary>
        /// A list of safe-to-expose parameters that can be passed from the frontend.
        /// </summary>
        public List<ParameterInfo> WebSafeParams;
    }
}
