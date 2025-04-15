

using System;
using System.Reflection;
using Api.EcmaScript.TypeScript;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;

namespace Api.EcmaScript
{
    public static partial class SourceGenerator
    {

        public static ClassDefinition OnNonEntityController(Type controller, Script script)
        {

            var ecmaService = Services.Get<EcmaService>();

            EnsureScript(script);

            var definition = new ClassDefinition() {
                Name = GetCleanTypeName(controller).Replace("Controller", "") + "Api"
            };

            var routeAttribute = controller.GetCustomAttribute<RouteAttribute>();

            var baseUrl = routeAttribute?.Template;

            if (baseUrl is null)
            {
                baseUrl = "";
            }

            if (baseUrl.StartsWith("v1/"))
            {
                baseUrl = baseUrl.Substring(3); // start from index 3 till end
            }

            definition.Children.Add(new ClassProperty() {
                PropertyName = "apiUrl",
                PropertyType = "string",
                DefaultValue = baseUrl
            });

            AddControllerMethods(controller, definition, baseUrl, script);

            return definition;
        }

    } 
}