namespace Api.SearchMetaData
{
    /// <summary>
    /// Config item to store the mapping between a content type and the includes which should be indexed
    /// </summary>
    public class MappingIncludes
    {
        /// <summary>
        /// The name of the content type e.g. User/Event etc
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The includes string 
        /// </summary>
        public string Includes { get; set; } = "*";
    }
}
