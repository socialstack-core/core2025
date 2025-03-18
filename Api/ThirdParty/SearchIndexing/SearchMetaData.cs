namespace Api.SearchIndexing
{
    public class SearchMetaData
    {
        public object PrimaryObject { get; set; }

        public MetaDataInclude[] Includes { get; set; }

    }

    public class MetaDataInclude
    {
        public dynamic[] Values { get; set; }
    }

}
