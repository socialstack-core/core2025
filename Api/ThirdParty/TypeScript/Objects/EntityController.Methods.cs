using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Api.TypeScript.Objects
{
    public partial class EntityController : AbstractTypeScriptObject
    {
        private List<ControllerMethod> _methods;

        /// <summary>
        /// Scans the controller for non-standard API methods (excluding CRUD), and maps metadata
        /// for TypeScript generation.
        /// </summary>
        /// <returns>A list of methods to expose to TypeScript.</returns>
        private List<ControllerMethod> GetEndpointMethods()
        {
            if (_methods != null)
                return _methods;

            var ts = Services.Get<TypeScriptService>();
            var controllerType = _referenceTypes.Item1;
            var knownCrudMethods = new HashSet<string> { "List", "Load", "Create", "Update", "Delete" };

            _methods = new List<ControllerMethod>();

            foreach (var method in controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                // Skip constructors and known CRUD methods
                if (method.IsConstructor || knownCrudMethods.Contains(method.Name))
                    continue;

                // Only consider methods with route attributes
                var httpAttr = method.GetCustomAttributes().FirstOrDefault(attr => attr is RouteAttribute or HttpMethodAttribute);
                if (httpAttr == null)
                    continue;

                // Determine the effective return type
                var returnAttr = method.GetCustomAttribute<ReturnsAttribute>();
                var effectiveReturnType = returnAttr != null
                    ? returnAttr.ReturnType
                    : TypeScriptService.UnwrapTypeNesting(method.ReturnType);

                // Create method representation
                var controllerMethod = new ControllerMethod
                {
                    Method = method,
                    RequestUrl = GetRouteTemplate(httpAttr),
                    TrueReturnType = effectiveReturnType,
                    ReturnType = returnAttr is not null ? returnAttr.ReturnType : UnwrapValueTask(method.ReturnType),
                    IsApiList = TypeScriptService.IsNestedCollection(method.ReturnType)
                };

                if (TypeScriptService.IsEntityType(controllerMethod.ReturnType))
                {
                    controllerMethod.RequiresIncludes = true;
                }

                // Analyze method parameters
                var webSafeParams = new List<ParameterInfo>();
                foreach (var param in method.GetParameters())
                {
                    if (param.ParameterType == typeof(Context))
                        continue;

                    bool isFromRoute = controllerMethod.RequestUrl?.Contains($"{{{param.Name}}}") == true &&
                                       param.GetCustomAttribute<FromRouteAttribute>() != null;

                    bool isFromQuery = param.GetCustomAttribute<FromQueryAttribute>() != null;
                    bool isFromBody = param.GetCustomAttribute<FromBodyAttribute>() != null;

                    if (isFromRoute || isFromQuery || isFromBody || ts.GetTypeOverwrite(param.ParameterType) != null)
                    {
                        webSafeParams.Add(param);
                    }

                    if (isFromBody)
                    {
                        controllerMethod.SendsData = true;
                        controllerMethod.BodyParam = param;
                    }

                    if (isFromQuery || ts.GetTypeOverwrite(param.ParameterType) != null || effectiveReturnType == _referenceTypes.Item2)
                    {
                        controllerMethod.RequiresIncludes = true;
                    }
                }

                controllerMethod.WebSafeParams = webSafeParams;
                controllerMethod.RequiresSessionSet = effectiveReturnType == typeof(Context);

                _methods.Add(controllerMethod);
            }

            return _methods;
        }

        private static Type UnwrapValueTask(Type returnType)
        {
            return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)
                ? returnType.GetGenericArguments()[0]
                : returnType;
        }

        private static string GetRouteTemplate(object attribute)
        {
            return attribute switch
            {
                RouteAttribute ra => ra.Template,
                HttpMethodAttribute hma => hma.Template,
                _ => string.Empty
            };
        }
    }

    /// <summary>
    /// Represents metadata about a controller method for TypeScript client generation.
    /// </summary>
    public class ControllerMethod
    {
        /// <summary>
        /// The method
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// URL pattern
        /// </summary>
        public string RequestUrl { get; set; }
        /// <summary>
        /// The body parameter if one is present.
        /// </summary>
        public ParameterInfo BodyParam { get; set; }
        /// <summary>
        /// True if it requires setSession being given to it.
        /// </summary>
        public bool RequiresSessionSet { get; set; } = false;
        /// <summary>
        /// True if the endpoint supports includes
        /// </summary>
        public bool RequiresIncludes { get; set; } = false;
        /// <summary>
        /// True if the endpoint sends anything
        /// </summary>
        public bool SendsData { get; set; } = false;
        /// <summary>
        /// True if the endpoint returns a list
        /// </summary>
        public bool IsApiList { get; set; } = false;
        /// <summary>
        /// The actual stated return type
        /// </summary>
        public Type TrueReturnType { get; set; }
        /// <summary>
        /// The resolved return type
        /// </summary>
        public Type ReturnType { get; set; }
        /// <summary>
        /// The set of usable parameters
        /// </summary>
        public List<ParameterInfo> WebSafeParams { get; set; }
    }
}
