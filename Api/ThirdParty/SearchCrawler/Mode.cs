using System;

namespace Api.SearchCrawler
{
    /// <summary>
    /// Used to track the search crawlers mode
    /// </summary>
    [Flags]
    public enum SearchCrawlerMode : ushort
    {
        /// <summary>
        /// Not specified
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Search Indexing
        /// </summary>
        Indexing = 1,
        /// <summary>
        /// Sitemap creation
        /// </summary>
        Sitemap = 2
    }
}
