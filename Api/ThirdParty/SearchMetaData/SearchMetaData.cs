using Stripe;

namespace Api.SearchMetaData
{
    public class SearchMetaData
    {
        public object PrimaryObject { get; set; }

        public MetaDataInclude[] Includes { get; set; }

    }

    public class MetaDataInclude
    {
        public string Name { get; set; }

        public string Field { get; set; }

        public dynamic[] Values { get; set; }
    }

}
