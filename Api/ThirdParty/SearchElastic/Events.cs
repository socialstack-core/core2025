using Api.Eventing;
using Api.SearchCrawler;
using Api.SearchElastic;
using Api.Tags;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace Api.Eventing
{
	/// <summary>
	/// Events are instanced automatically. 
	/// You can however specify a custom type or instance them yourself if you'd like to do so.
	/// </summary>
	public partial class Events
	{

		/// <summary>
		/// Event group for the search elastic indexer.
		/// </summary>
		public static SearchElasticEventGroup Elastic;
		
	}

    public partial class EventGroupCore<T, ID>
    {
        /// <summary>
        /// Called before a page is indexed which has a primary object
		/// to allow for the processing of related data/includes etc
        /// </summary>
        public EventHandler<Dictionary<string, List<object>>, CrawledPageMeta, IEnumerable<Tag>, T> BeforeIndexingMetaData;


        /// <summary>
        /// Called when a document is being indexed which has a primary object
        /// to allow for extra indexes to be defined based on the content
        /// </summary>
        public EventHandler<List<string>, CrawledPageMeta, IEnumerable<Tag>, T> BeforeIndexGetKeys;

    }
}

namespace Api.SearchElastic
{
	/// <summary>
	/// The group of events for services. See also Events.Service
	/// </summary>
	public partial class SearchElasticEventGroup : Eventing.EventGroupCore<object, uint>
    {
		/// <summary>
		/// Called after the core index document is created just prior to saving
		/// </summary>
		public Api.Eventing.EventHandler<Api.SearchElastic.Document> BeforeDocumentUpdate;

        /// <summary>
        /// Called to request a new document is indexed
        /// </summary>
        public Api.Eventing.EventHandler<bool, Api.SearchElastic.Document, List<string>> IndexDocument;

        /// <summary>
        /// Called to request a new document is removed by id
        /// </summary>
        public Api.Eventing.EventHandler<bool, string, List<string>> DeleteDocument;

        /// <summary>
        /// Called to request that any documents are removed by content type
        /// </summary>
        public Api.Eventing.EventHandler<bool, string, List<string>> DeleteContentType;

        /// <summary>
        /// Called to request a new documents are indexed
        /// </summary>
        public Api.Eventing.EventHandler<bool, List<Api.SearchElastic.Document>, List<string>> IndexDocuments;

        /// <summary>
        /// Called to request a new index is created
        /// </summary>
        public Api.Eventing.EventHandler<bool, string> CreateIndex;

        /// <summary>
        /// Called to request that the client is created/connected
        /// </summary>
        public Api.Eventing.EventHandler<bool> IsConnected;

        /// <summary>
        /// Called after a new connection to elastic is established 
        /// </summary>
        public Api.Eventing.EventHandler<bool, SearchElasticServiceConfig> AfterConnected;

        /// <summary>
        /// Called after a reset request to elastic is has been completed
        /// </summary>
        public Api.Eventing.EventHandler<bool, string> AfterReset;

        /// <summary>
        /// Called when before a page is indexed which has NO primary object
        /// </summary>
        public EventHandler<Dictionary<string, List<object>>, CrawledPageMeta, IEnumerable<Tag>> BeforeIndexingMeta;

        /// <summary>
        /// Called after the index document url has been determined
        /// </summary>
        public EventHandler<string, CrawledPageMeta> AfterUrl;

        /// <summary>
        /// Called after the list of indexes is created
        /// </summary>
        public Api.Eventing.EventHandler<Dictionary<string,string>, string, string> AfterGetAllIndexes;

        /// <summary>
        /// Called after the key of the current retrieval index is determined
        /// </summary>
        public Api.Eventing.EventHandler<string, string> AfterIndexKey;

        /// <summary>
        /// Called after the keys of the storage index is determined
        /// </summary>
        public Api.Eventing.EventHandler<List<string>, string> AfterStorageIndexKeys;

        /// <summary>
        /// Called after the page body has been loaded into a html document for parsing
		/// Great time to remove any unwanted footers etc
        /// </summary>
        public Api.Eventing.EventHandler<HtmlDocument> AfterPage;


    }
}
