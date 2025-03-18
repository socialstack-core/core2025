using System.Collections.Generic;

namespace Api.SearchElastic
{
    /// <summary>
    /// Set of aggregation/taxonomy ovverides to allow the structure/order to be set based on categories and tags
    /// </summary>
    public class AggregationStructure
    {
        /// <summary>
        /// Set of aggregation/taxonomy ovverides 
        /// </summary>
        public List<AggregationOverride> AggregationOverrides { get; set; }
    }

    /// <summary>
    /// The baseline/default taxonomy structure, this will be extracted from categories/tags
    /// If no values are defined then the values will be extracted dynamically from the search results
    /// </summary>
    public class AggregationOverride
    {
        /// <summary>
        /// The name of the aggregation, for example "taxonomy.destination"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The label to display to the user, for example "Destination"
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Taxonomy grouping, a way to structuyre/group values such as region/destinations
        /// </summary>
        public List<Group> Groups { get; set; }
        /// <summary>
        /// The actual taxonomy buckets/values, these store the possible values (and counts if passed back from Elastic)
        /// </summary>
        public List<Bucket> Buckets { get; set; }
    }

    /// <summary>
    /// A container for multiple buckets grouped, so like a sub set of destinations for a region 
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The name of the sub group
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The actual taxonomy buckets/values, these store the possible values (and counts if passed back from Elastic)
        /// </summary>
        public List<Bucket> Buckets { get; set; }
    }
}
