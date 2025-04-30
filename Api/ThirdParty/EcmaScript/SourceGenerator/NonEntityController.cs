using System;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public static ClassDefinition OnNonEntityController(Type controller, Script script)
        {
            // Ensure the script is registered.
            EnsureScript(script);

            // Create the ClassDefinition for the controller's API representation.
            var definition = new ClassDefinition
            {
                Name = GetCleanTypeName(controller).Replace("Controller", "") + "Api"
            };

            // Get the base URL from the RouteAttribute if available.
            var baseUrl = GetControllerBaseUrl(controller);

            // Add the apiUrl property with the base URL.
            definition.Children.Add(new ClassProperty
            {
                PropertyName = "apiUrl",
                PropertyType = "string",
                DefaultValue = baseUrl
            });

            // Add controller methods for this API.
            AddControllerMethods(controller, definition, baseUrl, script);

            return definition;
        }

        /// <summary>
        /// Retrieves the base URL from the controller's RouteAttribute, handling potential versions in the path.
        /// </summary>
        private static string GetControllerBaseUrl(Type controller)
        {
            var routeAttribute = controller.GetCustomAttribute<RouteAttribute>();
            var baseUrl = routeAttribute?.Template ?? "";

            // If the base URL starts with "v1/", remove it.
            if (baseUrl.StartsWith("v1/"))
            {
                baseUrl = baseUrl.Substring(3); // Skip the first 3 characters ('v1/')
            }

            return baseUrl;
        }
    }
}
