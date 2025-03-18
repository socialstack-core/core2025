namespace Api.SearchCrawler
{
    /// <summary>
    /// Used to track the search crawlers status
    /// </summary>
    public enum SearchCrawlerStatus : ushort
    {
        /// <summary>
        /// Not specified
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Crawling started
        /// </summary>
        Started = 1,
        /// <summary>
        /// Crawling completed
        /// </summary>
        Completed = 2
    }
}
