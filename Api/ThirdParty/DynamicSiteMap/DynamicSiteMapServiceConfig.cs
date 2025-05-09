
using Api.Configuration;
using System.Collections.Generic;

namespace Api.DynamicSiteMap
{
    /// <summary>
    /// Config for dynamic site mapping service.
    /// </summary>
    public class DynamicSiteMapServiceConfig : Config
    {
        /// <summary>
        /// The maximum number of pages per sitemap file
        /// </summary>
        public int MaxSiteMapPages { get; set; } = 1000;

        /// <summary>
        ///  Image title attributes 
        /// </summary>
        public List<string> ImageTitleAttributes { get; set; } = new List<string>() { "title", "alt", "aria-label" };

        /// <summary>
        /// Excluded paths (a list of path prefixes to be ignored)
        /// </summary>
        public List<string> ExcludedPaths { get; set; } = new List<string>();

        /// <summary>
        /// Flag to optionally disable service.
        /// </summary>
        public bool Disabled { get; set; } = true;

        /// <summary>
        /// Flag to determine if we are mapping pages based on locales 
        /// </summary>
        public bool UseLocaleDomains { get; set; } = false;
        
        /// <summary>
        /// /// Flag to determine if should expose images into the sitemap
        /// </summary>
        public bool IncludeImages { get; set; } = false;

        /// <summary>
        /// /// Flag to determine if should expose static images into the sitemap
        /// </summary>
        public bool IncludeStaticImages { get; set; } = false;

        /// <summary>
        /// Flag to optionally expose href lang entries for multi locale sites
        /// </summary>
        public bool IncludeHrefLangs { get; set; } = false;
		/// <summary>
		/// Should debug info be written to the console?
		/// </summary>
		public bool DebugToConsole { get; set; } = false;

	}

}