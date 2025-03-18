
using Api.Configuration;
using System.Collections.Generic;

namespace Api.SearchElastic
{
	/// <summary>
	/// Config for elastic search service.
	/// </summary>
	public class SearchElasticServiceConfig : Config
	{
        /// <summary>
        /// The elastic search instance user name
        /// </summary>
        public string UserName { get; set; } = "elastic";

        /// <summary>
        /// The elastic search instance user password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The elastic search instance fingerprint
        /// 
        /// How to get the client fingerprint 
        /// https://www.elastic.co/guide/en/elasticsearch/reference/8.1/configuring-stack-security.html#_connect_clients_to_elasticsearch_5
        /// 
        /// </summary>
        public string FingerPrint { get; set; }

        /// <summary>
        /// The url for connecting to the elastic search instance
        /// </summary>
        public string InstanceUrl { get; set; } = "";

        /// <summary>
        /// The index name 
        /// </summary>
        public string IndexName { get; set; } = "";

        /// <summary>
        /// List of tags to be treated as headers
        /// </summary>		
        public List<string> HeaderTags { get; set; } = new List<string>(){"h1", "h2", "h3", "h4", "h5", "h6"};

        /// <summary>
        /// Should debug info be written to the console?
        /// </summary>
        public bool DebugToConsole { get; set;} = false;

        /// <summary>
        /// Should progress info be written to the console?
        /// </summary>
        public bool ProgressToConsole { get; set; } = false;

        /// <summary>
        /// Should debug info be written for each query to the console?
        /// </summary>
        public bool DebugQueries { get; set; } = false;

        /// <summary>
        /// Should we always update the index or only if the checksum has changed
        /// </summary>
        public bool AlwaysUpdateIndex { get; set; } = true;

        /// <summary>
        /// Flag to optionally disable service.
        /// </summary>
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// The number of shards per index.
        /// </summary>
        public int Shards { get; set; } = 1;

        /// <summary>
        /// The number of replicas per index
        /// </summary>
        public int Replicas { get; set; } = 0;

        /// <summary>
        /// The max number of facet results
        /// </summary>
        public int FacetLimit { get; set; } = 100;

        /// <summary>
        /// The max number of documents to index at at time
        /// </summary>
        public int BulkIndexLimit { get; set; } = 100;

        /// <summary>
        /// Flag to determine if bulk indexing is active
        /// </summary>
        public bool UseBulkIndexing { get; set; } = true;

        /// <summary>
        /// The maximum number of mapping fields per index (index.mapping.total_fields.limit)
        /// </summary>
        public int MappingFieldsLimit { get; set; } = 2000;

    }

}