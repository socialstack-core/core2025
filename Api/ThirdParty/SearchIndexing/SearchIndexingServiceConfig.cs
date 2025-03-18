
using Api.Configuration;
using System.Collections.Generic;

namespace Api.SearchIndexing
{
	/// <summary>
	/// Config for search indexing service.
	/// </summary>
	public class SearchIndexingServiceConfig : Config
	{
        /// <summary>
        /// Include the primary object in the meta data 
        /// </summary>
        public bool IncludePrimary { get; set; } = true;


        /// <summary>
        /// List include mappings for content types
        /// </summary>		
        public List<MappingIncludes> Mappings { get; set; }


        /// <summary>
        /// List of html document selectors for removing page nodes prior to indexing
        /// e.g.
        /// //*[contains(concat(' ', @class, ' '), ' donotsearch ')]
        /// </summary>		
        public List<string> NodeSelectors { get; set; }

        /// <summary>
        /// Should debug info be written to the console?
        /// </summary>
        public bool DebugToConsole { get; set; } = false;
    }

}