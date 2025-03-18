using System.Collections.Generic;

namespace Api.SearchElastic
{
    /// <summary>
    /// Search results with aggregations (facets)
    /// </summary>
    public class DocumentsResult
    {
        /// <summary>
        /// Used to specify which page to get results for.
        /// </summary>
        public long PageIndex { get; set; }
        /// <summary>
        /// The number of results per page.
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// Total results count. This is different from Results.Count.
        /// </summary>
        public long TotalResults { get; set; }

        /// <summary>
        /// The results set
        /// </summary>
        public List<Document> Results { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Aggregation> Aggregations { get; set; }
    }

    /// <summary>
    /// Summary of available aggregations/facets
    /// </summary>
    public class Aggregation
	{
		/// <summary>
		/// The name for this aggregation
		/// </summary>
		public string Name { get; set; }
        /// <summary>
        /// The label for this aggregation
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The buckets in this aggregation
        /// </summary>
        public List<Bucket> Buckets { get; set; }
    }


    /// <summary>
    /// Single aggregation/facet entity
    /// </summary>
    public class Bucket
    {
        /// <summary>
        /// The key of the bucket
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Label to use for this bucket
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Number of entries in the bucket
        /// </summary>
        public long? Count { get; set; }
    }
}
