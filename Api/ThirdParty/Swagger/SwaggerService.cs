using Api.Startup;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Api.Swagger
{
    /// <summary>
    /// Service to assist with swagger content management
    /// </summary>
    [LoadPriority(101)]
    [HostType("web")]
    public partial class SwaggerService : AutoService
    {
        private SwaggerConfig _cfg;

        /// <summary>
        /// True if the service is active.
        /// </summary>
        private bool _isConfigured;

        /// <summary>
        /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
        /// </summary>
        public SwaggerService()
        {
            _cfg = GetConfig<SwaggerConfig>();
            UpdateIsConfigured();

            _cfg.OnChange += () =>
            {
                UpdateIsConfigured();
                return new ValueTask();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void FilterDocuments(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Get all controllers flagged with the internal attribute
            var excludedControllers = context.ApiDescriptions
                .Where(desc =>
                    desc.ActionDescriptor is ControllerActionDescriptor controllerDescriptor &&
                    controllerDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(InternalApiAttribute), true).Any())
                .Select(desc => desc.RelativePath)
                .Distinct()
                .ToList();

            foreach (var path in excludedControllers)
            {
                swaggerDoc.Paths.Remove("/" + path);
            }

            if (!_isConfigured)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(_cfg.Title))
            {
                swaggerDoc.Info.Title = _cfg.Title;
            }

            if (!string.IsNullOrWhiteSpace(_cfg.Description))
            {
                swaggerDoc.Info.Description = _cfg.Description;
            }

            if (!string.IsNullOrWhiteSpace(_cfg.Version))
            {
                swaggerDoc.Info.Version = _cfg.Version;
            }

            if (_cfg.IncludedServices != null && _cfg.IncludedServices.Any())
            {
                var notIncludedServices = context.ApiDescriptions
                    .Where(desc =>
                        desc.ActionDescriptor is ControllerActionDescriptor controllerDescriptor &&
                        !string.IsNullOrEmpty(desc.ActionDescriptor.RouteValues["controller"]) &&
                        !_cfg.IncludedServices.Contains(desc.ActionDescriptor.RouteValues["controller"], StringComparer.OrdinalIgnoreCase))
                    .Select(desc => desc.RelativePath)
                    .Distinct()
                    .ToList();

                // Remove the paths from the Swagger document
                foreach (var path in notIncludedServices)
                {
                    swaggerDoc.Paths.Remove("/" + path);
                }
            }
            else if (_cfg.ExcludedServices != null && _cfg.ExcludedServices.Any())
            {
                var excludedServices = context.ApiDescriptions
                    .Where(desc =>
                        desc.ActionDescriptor is ControllerActionDescriptor controllerDescriptor &&
                        !string.IsNullOrEmpty(desc.ActionDescriptor.RouteValues["controller"]) &&
                        _cfg.ExcludedServices.Contains(desc.ActionDescriptor.RouteValues["controller"], StringComparer.OrdinalIgnoreCase))
                .Select(desc => desc.RelativePath)
                .Distinct()
                .ToList();

                // Remove the paths from the Swagger document
                foreach (var path in excludedServices)
                {
                    swaggerDoc.Paths.Remove("/" + path);
                }
            }

            // exclude any entries with specific Operations so POST, DELETE etc
            if (_cfg.ExcludedOperations != null && _cfg.ExcludedOperations.Any())
            {

                var _methodsToRemove = _cfg.ExcludedOperations
                    .Select(method => Enum.TryParse<OperationType>(method.Trim(), true, out var op) ? op : (OperationType?)null)
                    .Where(op => op.HasValue)
                    .Select(op => op.Value)
                    .ToArray();

                if (_methodsToRemove.Any())
                {
                    // Iterate through paths and operations to remove matching HTTP methods
                    foreach (var path in swaggerDoc.Paths.ToList())
                    {
                        foreach (var methodToRemove in _methodsToRemove)
                        {
                            if (path.Value.Operations.ContainsKey(methodToRemove))
                            {
                                // Remove the specific operation
                                path.Value.Operations.Remove(methodToRemove);
                            }
                        }

                        // If no operations remain in the path, remove the entire path
                        if (!path.Value.Operations.Any())
                        {
                            swaggerDoc.Paths.Remove(path.Key);
                        }
                    }
                }
            }

            // finally exclude any entries with specific suffixes so .csv .pot etc
            if (_cfg.ExcludedEndpointTypes != null && _cfg.ExcludedEndpointTypes.Any())
            {
                var pathsToRemove = swaggerDoc.Paths
                    .Where(p => _cfg.ExcludedEndpointTypes.Any(ep => p.Key.EndsWith("." + ep, StringComparison.OrdinalIgnoreCase)))
                    .Select(p => p.Key)
                    .ToList();

                foreach (var path in pathsToRemove)
                {
                    swaggerDoc.Paths.Remove(path);
                }
            }

            // Quicker to use the built in attribute 
            // [ApiExplorerSettings(IgnoreApi = true)]

            /*
            // Get all endpoints flagged with the attribute
            var endpointsToRemove = swaggerDoc.Paths
                        .Where(pathItem =>
                            context.ApiDescriptions.Any(desc =>
                                desc.RelativePath == pathItem.Key.TrimStart('/') &&
                                desc.ActionDescriptor is ControllerActionDescriptor controllerDescriptor &&
                                controllerDescriptor.MethodInfo.GetCustomAttributes(typeof(InternalApiAttribute), true).Any()
                            )
                        )
                        .Select(pathItem => pathItem.Key)
                        .ToList();

            foreach (var path in endpointsToRemove)
            {
                swaggerDoc.Paths.Remove(path);
            }
            */

            if (_cfg.ExcludeSchema)
            {
                // Remove the schemas section from components
                swaggerDoc.Components.Schemas.Clear();
            }
        }

        /// <summary>
        /// Gets the service config (readonly).
        /// </summary>
        /// <returns></returns>
        public SwaggerConfig GetConfiguration()
        {
            return _cfg;
        }


        /// <summary>
        /// True if this service is configured and active.
        /// </summary>
        /// <returns></returns>
        public bool IsConfigured()
        {
            return _isConfigured;
        }

        /// <summary>
        /// Sets _isConfigured
        /// </summary>
        private void UpdateIsConfigured()
        {
            _isConfigured = !string.IsNullOrWhiteSpace(_cfg.Title)
                || !string.IsNullOrWhiteSpace(_cfg.Description)
                || !string.IsNullOrWhiteSpace(_cfg.Version)
                || _cfg.ExcludedOperations.Any()
                || _cfg.ExcludedEndpointTypes.Any()
                || _cfg.IncludedServices.Any()
                || _cfg.ExcludedServices.Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAssemblyAttribute<T>() where T : Attribute
        {
            var thisAsm = typeof(EventListener).Assembly;

            object[] attributes = thisAsm.GetCustomAttributes(typeof(T), false);

            if (attributes.Length == 0)
                return null;

            return attributes.OfType<T>().SingleOrDefault();
        }

    }
}

