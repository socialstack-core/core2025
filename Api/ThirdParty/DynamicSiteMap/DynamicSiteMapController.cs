using Api.Contexts;
using Api.SiteDomains;
using Api.Startup;
using Api.Translate;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.DynamicSiteMap
{
    /// <summary>
    /// Handles dynamic sitemap based on domain/locale
    /// </summary>

    [InternalApi]
    public partial class DomainSiteMapController : Controller
    {
        private SiteDomainService _siteDomains;
        private DynamicSiteMapService _dynamicMaps;

		public DomainSiteMapController(DynamicSiteMapService dynamicMaps, SiteDomainService siteDomains)
        {
            _siteDomains = siteDomains;
			_dynamicMaps = dynamicMaps;
		}

		/// <summary>
		/// Exposes the dynamic site map file
		/// </summary>
		[HttpGet("/sitemap.xml")]
        public virtual async ValueTask<ActionResult> SiteMapXML()
        {
            var prefix = string.Empty;

            var _cfg = _dynamicMaps.GetConfiguration();

			if (_cfg.UseSiteDomains)
            {
                var siteDomain = _siteDomains.GetByDomain(Request.Host.Value);

                prefix = "-core";

                if (siteDomain != null)
                {
                    prefix = $"-{siteDomain.Code.ToLower()}";
                }
            }
            else if (_cfg.UseLocaleDomains)
            {
                var siteLocaleId = Services.Get<LocaleService>().GetByDomain(Request.Host.Value);

                if (!siteLocaleId.HasValue) {
                    var context = await Request.GetContext();

                    siteLocaleId = context.LocaleId;
                }
                
                prefix = $"-{siteLocaleId}";
            }

            if (System.IO.File.Exists($"UI/public/sitemap{prefix}.xml"))
            {
                var stream = new System.IO.FileStream($"UI/public/sitemap{prefix}.xml", System.IO.FileMode.Open);
                return File(stream, "application/xml");
            }

            Log.Error("DomainSiteMap", null, $"Failed to locate dynamic sitemap file 'UI/public/sitemap{prefix}.xml'");

            return NotFound();
        }

    }
}