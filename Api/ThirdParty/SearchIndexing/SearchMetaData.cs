namespace Api.SearchIndexing
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchMetaData
    {
        /// <summary>
        /// The primary object.
        /// </summary>
        public object PrimaryObject { get; set; }

        /// <summary>
        /// The set of includes when serialising the object.
        /// </summary>
        public MetaDataInclude[] Includes { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class MetaDataInclude
    {
        /// <summary>
        /// 
        /// </summary>
        public dynamic[] Values { get; set; }
    }

}
