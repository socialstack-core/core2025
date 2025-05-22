using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Api.TypeScript.Objects
{
    public partial class AutoController : AbstractTypeScriptObject
    {
        private List<ControllerMethod> _methods = null;

        /// <summary>
        /// Retrieves metadata for all public API endpoint methods defined in the generic <c>AutoController&lt;T, ID&gt;</c> type.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ControllerMethod"/> instances representing valid API endpoints
        /// with route metadata, parameters, and return types.
        /// </returns>
        /// <remarks>
        /// This method performs reflection on the <c>AutoController&lt;T, ID&gt;</c> generic type to identify
        /// public instance methods that are decorated with routing attributes such as <see cref="HttpGetAttribute"/>,
        /// <see cref="HttpPostAttribute"/>, or <see cref="RouteAttribute"/>.
        /// <para>
        /// It filters and adapts method metadata to create <see cref="ControllerMethod"/> representations,
        /// which are later used to generate TypeScript methods.
        /// </para>
        /// <para>
        /// Handles:
        /// </para>
        /// <list type="bullet">
        /// <item><description>Exclusion of overloads with fewer parameters.</description></item>
        /// <item><description>Mapping of <c>[FromRoute]</c> and <c>[FromQuery]</c> parameters into URL templates.</description></item>
        /// <item><description>Detection of body parameters via <c>[FromBody]</c>.</description></item>
        /// <item><description>Marking methods that require session or support includes.</description></item>
        /// </list>
        /// </remarks>
        private List<ControllerMethod> GetEndpointMethods()
        {
            if (_methods is not null)
            {
                return _methods;
            }

            _methods = [];

            foreach (var method in typeof(AutoController<,>).GetMethods(
                         BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                // Skip if method overload with more parameters already exists
                var existing = _methods.Find(m => m.Method.Name == method.Name);
                if (existing is not null)
                {
                    if (existing.Method.GetParameters().Length >= method.GetParameters().Length)
                        continue;
                    else
                        _methods.Remove(existing);
                }

                var methodAttributes = method.GetCustomAttributes();
                var httpAttribute = methodAttributes.FirstOrDefault(attr => attr is RouteAttribute
                    or HttpGetAttribute or HttpPostAttribute or HttpPutAttribute
                    or HttpDeleteAttribute or HttpPatchAttribute);

                if (httpAttribute is null)
                {
                    continue;
                }

                var returnsAttr = method.GetCustomAttribute<ReturnsAttribute>();
                var returnType = returnsAttr is not null
                    ? returnsAttr.ReturnType
                    : TypeScriptService.UnwrapTypeNesting(method.ReturnType);

                var methodParams = method.GetParameters();
                var webSafeParams = new List<ParameterInfo>();

                var controllerMethod = new ControllerMethod
                {
                    TrueReturnType = returnType,
                    Method = method,
                    RequiresSessionSet = returnType == typeof(Context),
                    RequiresIncludes = true,
                    IsApiList = TypeScriptService.IsNestedCollection(method.ReturnType),
                    SendsData = methodParams.Any(p => p.GetCustomAttribute<FromBodyAttribute>() is not null)
                };

                controllerMethod.RequestUrl = httpAttribute switch
                {
                    RouteAttribute route => route.Template,
                    HttpMethodAttribute http => http.Template,
                    _ => ""
                } ?? "";

                controllerMethod.RequestUrl = controllerMethod.RequestUrl.ToLower();

                // Parse parameters
                foreach (var param in methodParams)
                {
                    if (param.Name == "includes")
                    {
                        // includes handled automatically.
                        continue;
                    }
                    if (controllerMethod.RequestUrl.Contains($"{{{param.Name}}}") &&
                        param.GetCustomAttribute<FromRouteAttribute>() is not null)
                    {
                        webSafeParams.Add(param);
                        controllerMethod.RequestUrl = controllerMethod.RequestUrl
                            .Replace($"{{{param.Name}}}", $"' + {param.Name} + '");
                        continue;
                    }

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

                    if (param.GetCustomAttribute<FromBodyAttribute>() is not null)
                    {
                        webSafeParams.Add(param);
                        controllerMethod.SendsData = true;
                        controllerMethod.BodyParam = param;
                    }
                }

                controllerMethod.RequestUrl = controllerMethod.RequestUrl.Replace("?&", "?");
                controllerMethod.WebSafeParams = webSafeParams;

                _methods.Add(controllerMethod);
            }

            return _methods;
        }
    }
}
