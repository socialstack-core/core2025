using Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.SiteDomains
{
    /// <summary>Handles siteDomain endpoints.</summary>
    [Route("v1/siteDomain")]
    public partial class SiteDomainController : AutoController<SiteDomain>
    {
        private SiteDomainService _siteDomainService;

        /// <summary>
        /// Instanced automatically.
        /// </summary>
        public SiteDomainController(SiteDomainService svc)
        {
            _siteDomainService = svc;
        }

        /// <summary>
        /// GET /v1/domain/code/abc/
        /// Gets the primary site domain mapping via it's code
        /// </summary>
        [HttpGet("code/{code}")]
        public virtual SiteDomain GetByCode(Context context, [FromRoute] string code)
        {
            var siteDomain = _siteDomainService.GetByCode(code);
            return siteDomain;
		}
    }
}