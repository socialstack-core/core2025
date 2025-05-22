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
    public partial class NonEntityController : AbstractTypeScriptObject
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

            foreach (var method in _referenceType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
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
                    RequiresIncludes = methodParams.Any(p => TypeScriptService.IsEntityType(p.ParameterType) || TypeScriptService.IsEntityType(returnType)) ,
                    IsApiList = TypeScriptService.IsNestedCollection(method.ReturnType),
                    SendsData = methodParams.Any(p => p.GetCustomAttribute<FromBodyAttribute>() is not null)
                };

                controllerMethod.RequestUrl = httpAttribute switch
                {
                    RouteAttribute routeAttr => routeAttr.Template,
                    HttpMethodAttribute httpAttr => httpAttr.Template,
                    _ => controllerMethod.RequestUrl ?? ""
                };

                if (controllerMethod.RequestUrl is not null)
                {
                    controllerMethod.RequestUrl = controllerMethod.RequestUrl.ToLower();
                }

                // Parse method parameters
                foreach (var param in methodParams)
                {
                    // Route-bound parameter
                    if (controllerMethod.RequestUrl is not null && controllerMethod.RequestUrl!.Contains($"{{{param.Name}}}") &&
                        param.GetCustomAttribute<FromRouteAttribute>() is not null)
                    {
                        webSafeParams.Add(param);
                        controllerMethod.RequestUrl = controllerMethod.RequestUrl.Replace($"{{{param.Name}}}", $"' + {param.Name} + '");
                        continue;
                    }

                    // Query-bound parameter
                    if (controllerMethod.RequestUrl is not null && param.GetCustomAttribute<FromQueryAttribute>() is not null)
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
                controllerMethod.RequestUrl = controllerMethod.RequestUrl is not null ? controllerMethod.RequestUrl!.Replace("?&", "?") : "";
                controllerMethod.WebSafeParams = webSafeParams;

                _methods.Add(controllerMethod);
            }

            return _methods;
        }
    }
}
