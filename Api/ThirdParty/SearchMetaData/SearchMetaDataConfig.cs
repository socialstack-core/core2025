
using Api.Configuration;
using System.Collections.Generic;

namespace Api.SearchMetaData
{
	/// <summary>
	/// Config for search indexing service.
	/// </summary>
	public class SearchMetaDataConfig : Config
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
        /// List include any field names to be ignored (slug/legacyid etc)
        /// </summary>		
        public List<string> IgnoredFields { get; set; }


        /// <summary>
        /// Should debug info be written to the console?
        /// </summary>
        public bool DebugToConsole { get; set; } = false;
    }

}