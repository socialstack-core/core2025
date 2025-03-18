
using Api.Configuration;
using System.Collections.Generic;

namespace Api.SearchCrawler
{
	/// <summary>
	/// Config for elastic search service.
	/// </summary>
	public class SearchCrawlerConfig : Config
	{
        /// <summary>
        /// Should debug info be written to the console?
        /// </summary>
        public bool DebugToConsole { get; set;} = false;

        /// <summary>
        /// Should the page content be rendered 
        /// </summary>
        public bool IncludePageContent { get; set; } = true;

        /// <summary>
        /// Flag to optionally disable service.
        /// </summary>
        public bool Disabled { get; set; } = true;

    }

}