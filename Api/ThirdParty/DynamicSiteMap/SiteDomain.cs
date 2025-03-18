using Api.AutoForms;

namespace Api.SiteDomains
{
    partial class SiteDomain
    {
        /// <summary>
        /// Indicates if the domain is excluded from the dynamic sitemaps
        /// </summary>
        [Order(4)]
        [Data("hint", "Indicates if the domain is excluded from the dynamic sitemaps")]
        public bool ExcludeFromSiteMap;
    }
}
