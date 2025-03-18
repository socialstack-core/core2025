
using Api.Configuration;

namespace Api.SearchEntities
{
    /// <summary>
    /// Config for entity search indexing service.
    /// </summary>
    public class SearchEntityConfig : Config
    {
        /// <summary>
        /// Flag to optionally disable service.
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// The index name 
        /// </summary>
        public string IndexName { get; set; } = "content";

        /// <summary>
        /// The max number of documents to index at at time
        /// </summary>
        public int BulkIndexLimit { get; set; } = 100;

        /// <summary>
        /// Flag to determine if content updates are tracked for index sync
        /// </summary>
        public bool UseDynamicIndexing { get; set; } = true;

        /// <summary>
        /// Should debug info be written to the console?
        /// </summary>
        public bool DebugToConsole { get; set; } = false;

    }

}