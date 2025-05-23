using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Api.TypeScript.Objects
{
    /// <summary>
    /// Generates a TypeScript API class for a specific entity/controller pair.
    /// </summary>
    /// <remarks>
    /// This controller generator builds a concrete TypeScript API class based on a server-side controller
    /// and entity type. It ensures required API modules are registered and produces methods that match
    /// the .NET controller's endpoints.
    ///
    /// The resulting class extends <c>AutoController&lt;T, ID&gt;</c> and provides a typed frontend interface
    /// with IntelliSense and route bindings.
    /// </remarks>
    public partial class NonEntityController : AbstractTypeScriptObject
    {
        private readonly Type _referenceType;
        private readonly ESModule _container;

        private readonly List<Type> _requiredImports = [];

        /// <summary>
        /// Constructs an <see cref="EntityController"/> code generator instance.
        /// </summary>
        /// <param name="controllerType">The .NET controller type (e.g. <c>ArticlesController</c>).</param>
        /// <param name="container">The ES module container to register generated types and dependencies with.</param>
        /// <remarks>
        /// This constructor registers the target entity type and collects required dependencies such as
        /// other return types and Web API methods. It scans controller method signatures to prepare metadata
        /// for TypeScript output.
        /// </remarks>
        public NonEntityController(Type controllerType, ESModule container)
        {

            _container = container;
            _referenceType = controllerType;

            foreach (var method in GetEndpointMethods())
            {
                
                foreach (var param in method.WebSafeParams)
                {
                    if (!param.ParameterType.IsPrimitive)
                    {
                        container.AddType(param.ParameterType);
                    }
                }

                var isArrayType = method.IsApiList;

                if (!container.HasTypeDefinition(method.TrueReturnType, out _))
                {
                    if (TypeScriptService.IsEntityType(method.TrueReturnType))
                    {
                        // import instead
                        _requiredImports.Add(method.TrueReturnType);
                    }
                    else
                    {
                        container.AddType(method.TrueReturnType);
                    };
                }

                if (isArrayType)
                {
                    _container.RequireWebApi(WebApis.GetJson);
                }
                else
                {
                    if (TypeScriptService.IsEntityType(method.TrueReturnType))
                    {
                        _container.RequireWebApi(WebApis.GetOne);
                    }
                    else
                    {
                        container.AddType(method.TrueReturnType);
                        _container.RequireWebApi(WebApis.GetJson);
                    }
                }
            }
        }

        public List<Type> GetRequiredImports()
        {
            return _requiredImports;
        }

        /// <summary>
        /// Emits the TypeScript source code for the generated API class for the given controller/entity pair.
        /// </summary>
        /// <param name="builder">A <see cref="StringBuilder"/> to receive the generated TypeScript source.</param>
        /// <param name="svc">The <see cref="TypeScriptService"/> used for formatting types and identifiers.</param>
        /// <remarks>
        /// Outputs a TypeScript class that extends <c>AutoController&lt;T, ID&gt;</c> with route-specific methods.
        /// Each method includes appropriate return types, parameters, and request bindings (e.g., <c>getJson</c>,
        /// <c>getList</c>, <c>getOne</c>).
        /// </remarks>
        public override void ToSource(StringBuilder builder, TypeScriptService svc)
        {
            builder.AppendLine();
            builder.AppendLine($"export class {svc.GetGenericSignature(_referenceType)} {{");

            var baseUrlRoute = _referenceType.GetCustomAttribute<RouteAttribute>();
            var baseUrl = baseUrlRoute is not null ? baseUrlRoute.Template : "";

            builder.AppendLine();
            builder.AppendLine($"   private apiUrl: string = '/{baseUrl}';");
            builder.AppendLine();

            // Generate a method for each endpoint
            foreach (var method in GetEndpointMethods())
            {
                var fullUrl = (baseUrlRoute != null ? baseUrlRoute.Template : "").ToLower() + "/" + method.RequestUrl;
                fullUrl = fullUrl.Replace("//", "/");

                var isArrayType = method.IsApiList;

                builder.AppendLine("    /**");
                builder.AppendLine("     * Generated from a .NET type.");
                builder.AppendLine($"     * @see {{{_referenceType}}}::{{{method.Method.Name}}}");
                builder.AppendLine($"     * @url '{fullUrl}'");
                builder.AppendLine("     */");
                builder.Append($"    {TypeScriptService.LcFirst(method.Method.Name)} = (");

                int paramCount = 0;
                if (method.RequiresSessionSet)
                {
                    builder.Append("setSession: (s: SessionResponse) => Session");
                    paramCount++;
                }

                foreach (var param in method.WebSafeParams)
                {
                    if (paramCount > 0) builder.Append(", ");

                    string paramType = param.ParameterType == typeof(JObject)
                        ? "Record<string, any>"
                        : svc.GetGenericSignature(param.ParameterType);

                    builder.Append($"{param.Name}{(param.IsOptional ? "?" : "")}: {paramType}");
                    paramCount++;
                }

                // Closing params and declaring return type
                string call = "getJson";

                if (isArrayType)
                {
                    if (TypeScriptService.IsEntityType(method.TrueReturnType))
                    {
                        call = $"getList<{svc.GetGenericSignature(method.TrueReturnType)}>";
                        builder.Append($"): Promise<ApiList<{svc.GetGenericSignature(method.TrueReturnType)}>> => {{");
                    }
                    else
                    {
                        call = $"getJson<{svc.GetGenericSignature(method.TrueReturnType)}[]>";
                        builder.Append($"): Promise<{svc.GetGenericSignature(method.TrueReturnType)}[]> => {{");
                    }
                }
                else
                {
                    if (TypeScriptService.IsEntityType(method.TrueReturnType))
                    {
                        call = $"getOne<{svc.GetGenericSignature(method.TrueReturnType)}>";
                        builder.Append($"): Promise<{svc.GetGenericSignature(method.TrueReturnType)}> => {{");
                    }
                    else
                    {
                        call = $"getJson<{svc.GetGenericSignature(method.TrueReturnType)}>";
                        builder.Append($"): Promise<{svc.GetGenericSignature(method.TrueReturnType)}> => {{");
                    }
                }

                builder.AppendLine();

                string url = URLBuilder.BuildUrl(method);
                
                builder.AppendLine($"        return {call}(this.apiUrl + '{url}'{(method.SendsData ? $", {method.BodyParam.Name}" : "")})");
                
                if (method.RequiresSessionSet)
                {
                    builder.AppendLine("            .then(session => { setSession(session) })");
                }


                builder.AppendLine("    }");
                builder.AppendLine();
            }

            builder.AppendLine("}");
            builder.AppendLine();

            builder.AppendLine($"export default new {svc.GetGenericSignature(_referenceType)}();");
        }
        
        /// <summary>
        /// Helper method to extract base type from a collection type (e.g., List<StaticFileInfo> or StaticFileInfo[]).
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The element type of the collection, or the original type if it is not a collection.</returns>
        public Type ExtractElementType(Type type)
        {
            // If it's an array type, get the element type
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            // If it's a generic collection type (e.g., List<T>), get the generic argument
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return type.GetGenericArguments()[0];
            }

            // Return the original type if it's neither an array nor a generic collection
            return type;
        }
    }
}
