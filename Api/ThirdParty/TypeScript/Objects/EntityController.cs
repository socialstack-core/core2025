using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Api.Startup;
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
    public partial class EntityController : AbstractTypeScriptObject
    {
        private readonly (Type controllerType, Type entityType) _referenceTypes;
        private readonly ESModule _container;

        /// <summary>
        /// Constructs an <see cref="EntityController"/> code generator instance.
        /// </summary>
        /// <param name="controllerType">The .NET controller type (e.g. <c>ArticlesController</c>).</param>
        /// <param name="entityType">The associated entity type (e.g. <c>Article</c>).</param>
        /// <param name="container">The ES module container to register generated types and dependencies with.</param>
        /// <remarks>
        /// This constructor registers the target entity type and collects required dependencies such as
        /// other return types and Web API methods. It scans controller method signatures to prepare metadata
        /// for TypeScript output.
        /// </remarks>
        public EntityController(Type controllerType, Type entityType, ESModule container)
        {
            container.AddType(entityType);
            container.MarkAsEntityModule();

            _container = container;
            _referenceTypes = (controllerType, entityType);

            foreach (var method in GetEndpointMethods())
            {
                if (method.ReturnType != _referenceTypes.entityType && !TypeScriptService.IsEntityType(method.ReturnType))
                {
                    container.AddType(method.ReturnType);
                }
                
                TypeScriptService.EnsureParameterTypes(method.WebSafeParams, container);
                TypeScriptService.EnsureApis(method, container, _referenceTypes.entityType);
            }
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
            builder.AppendLine($"export class {svc.GetGenericSignature(_referenceTypes.entityType)}Api extends AutoController<{svc.GetGenericSignature(_referenceTypes.entityType)},uint>{{");

            var baseUrlRoute = _referenceTypes.controllerType.GetCustomAttribute<RouteAttribute>();
            var baseUrl = baseUrlRoute is not null ? baseUrlRoute.Template : "";

            baseUrl = baseUrl.ToLower();

            builder.AppendLine();
            builder.AppendLine("    constructor(){");
            builder.AppendLine($"        super('/{baseUrl}');");
            builder.AppendLine($"        this.includes = new {svc.GetGenericSignature(_referenceTypes.entityType)}Includes();");
            builder.AppendLine("    }");
            builder.AppendLine();

            // Generate a method for each endpoint
            foreach (var method in GetEndpointMethods())
            {
                var fullUrl = (baseUrlRoute != null ? baseUrlRoute.Template : "") + "/" + method.RequestUrl;
                fullUrl = fullUrl.Replace("//", "/");

                var isArrayType = method.IsApiList;

                builder.AppendLine("    /**");
                builder.AppendLine("     * Generated from a .NET type.");
                builder.AppendLine($"     * @see {{{_referenceTypes.controllerType}}}::{{{method.Method.Name}}}");
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
                        ? svc.GetGenericSignature(_referenceTypes.entityType)
                        : svc.GetGenericSignature(param.ParameterType);

                    builder.Append($"{param.Name}{(param.IsOptional ? "?" : "")}: {paramType}");
                    paramCount++;
                }

                if (method.RequiresIncludes)
                {
                    if (paramCount != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append("includes?: ApiIncludes[]");
                }

                // Closing params and declaring return type
                string call = "getJson";

                if (isArrayType)
                {
                    if (method.ReturnType == _referenceTypes.entityType)
                    {
                        call = $"getList<{svc.GetGenericSignature(method.ReturnType)}>";
                        builder.Append($"): Promise<ApiList<{svc.GetGenericSignature(method.ReturnType)}>> => {{");
                    }
                    else if (method.ReturnType.IsGenericTypeDefinition &&
                             method.ReturnType.GetGenericTypeDefinition() == typeof(ContentStream<,>))
                    {
                        call = $"getList<{svc.GetGenericSignature(method.ReturnType)}>";
                        builder.Append($"): Promise<ApiList<{svc.GetGenericSignature(method.ReturnType)}>> => {{");
                    }
                    else
                    {
                        call = $"getJson<{svc.GetGenericSignature(method.ReturnType)}[]>";
                        builder.Append($"): Promise<{svc.GetGenericSignature(method.ReturnType)}[]> => {{");
                    }
                }
                else
                {
                    if (method.RequiresSessionSet)
                    {
                        call = "getJson<SessionResponse>";
                        _container.RequireWebApi(WebApis.GetJson);
                        builder.Append($"): Promise<Session> => {{");
                    }
                    else if (TypeScriptService.IsEntityType(method.ReturnType))
                    {
                        call = $"getOne<{svc.GetGenericSignature(method.ReturnType)}>";
                        builder.Append($"): Promise<{svc.GetGenericSignature(method.ReturnType)}> => {{");
                    }
                    else if (method.ReturnType.IsGenericTypeDefinition &&
                              method.ReturnType.GetGenericTypeDefinition() == typeof(ContentStream<,>))
                    {
                        call = $"getList<{svc.GetGenericSignature(method.ReturnType)}>";
                        builder.Append($"): Promise<ApiList<{svc.GetGenericSignature(method.ReturnType)}>> => {{");
                    }
                    else
                    {
                        call = $"getJson<{svc.GetGenericSignature(method.ReturnType)}>";
                        builder.Append($"): Promise<{svc.GetGenericSignature(method.ReturnType)}> => {{");
                    }
                }

                builder.AppendLine();

                var url = URLBuilder.BuildUrl(method);
                
                builder.AppendLine($"        return {call}(this.apiUrl + '{url}'{(method.SendsData ? $", {method.BodyParam.Name}" : "")})");
                
                if (method.RequiresSessionSet)
                {
                    builder.AppendLine("            .then(setSession)");
                }

                builder.AppendLine("    }");
                builder.AppendLine();
            }

            builder.AppendLine("}");
            builder.AppendLine();

            builder.AppendLine($"export default new {svc.GetGenericSignature(_referenceTypes.entityType)}Api();");
        }
    }
}
