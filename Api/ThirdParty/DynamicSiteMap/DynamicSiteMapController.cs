using Api.Contexts;
using Api.SiteDomains;
using Api.Startup;
using Api.Startup.Routing;
using Api.Translate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.DynamicSiteMap
{
    /// <summary>
    /// Handles dynamic sitemap based on domain/locale
    /// </summary>

    [InternalApi]
    public partial class DomainSiteMapController : AutoController
    {
        private SiteDomainService _siteDomains;
        private DynamicSiteMapService _dynamicMaps;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicMaps"></param>
        /// <param name="siteDomains"></param>
		public DomainSiteMapController(DynamicSiteMapService dynamicMaps, SiteDomainService siteDomains)
        {
            _siteDomains = siteDomains;
			_dynamicMaps = dynamicMaps;
		}

		/// <summary>
		/// Exposes the dynamic site map file
		/// </summary>
		[HttpGet("/sitemap.xml")]
        public virtual async ValueTask<FileContent?> SiteMapXML(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var prefix = string.Empty;

            var _cfg = _dynamicMaps.GetConfiguration();

			if (_cfg.UseSiteDomains)
            {
                var siteDomain = _siteDomains.GetByDomain(request.Host.Value);

                prefix = "-core";

                if (siteDomain != null)
                {
                    prefix = $"-{siteDomain.Code.ToLower()}";
                }
            }
            else if (_cfg.UseLocaleDomains)
            {
                var siteLocaleId = Services.Get<LocaleService>().GetByDomain(request.Host.Value);

                if (!siteLocaleId.HasValue) {
                    var context = await request.GetContext();

                    siteLocaleId = context.LocaleId;
                }
                
                prefix = $"-{siteLocaleId}";
            }

            // This needs replacement
            if (System.IO.File.Exists($"UI/public/sitemap{prefix}.xml"))
            {
                var fileContent = System.IO.File.ReadAllBytes($"UI/public/sitemap{prefix}.xml");
                return new FileContent(fileContent, "application/xml");
            }

            Log.Error("DomainSiteMap", null, $"Failed to locate dynamic sitemap file 'UI/public/sitemap{prefix}.xml'");

            return null;
        }

    }
}