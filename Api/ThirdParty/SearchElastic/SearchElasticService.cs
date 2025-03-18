using Api.Categories;
using Api.Database;
using Api.Eventing;
using Api.Pages;
using Api.SearchCrawler;
using Api.Startup;
using Api.Tags;
using Api.Translate;
using Api.Users;
using Elasticsearch.Net;
using HtmlAgilityPack;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Context = Api.Contexts.Context;
using Page = Api.Pages.Page;

namespace Api.SearchElastic
{
    /// <summary>
    /// https://wiki.socialstack.dev/index.php?title=Search_Elastic
    /// 
    /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/introduction.html
    /// 
    /// Using elastic search v8 with 7.17 client (v8 client is still wip)
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/connecting-to-elasticsearch-v8.html
    /// 
    /// How to get the client fingerprint 
    /// https://www.elastic.co/guide/en/elasticsearch/reference/8.1/configuring-stack-security.html#_connect_clients_to_elasticsearch_5
    /// 
    /// 
    /// </summary>

    [LoadPriority(9)]
    [HostType("index")]
    [HostType("web")]
    public partial class SearchElasticService : AutoService
    {
        private SearchElasticServiceConfig _cfg;
        private ElasticClient _client;
        private readonly LocaleService _locales;
        private readonly PageService _pageService;
        private readonly TagService _tagService;
        private readonly CategoryService _categoryService;

        private ConcurrentDictionary<string, CrawledPageMeta> _processed = new ConcurrentDictionary<string, CrawledPageMeta>();
        private ConcurrentDictionary<string, string> _existingDocHashes = new ConcurrentDictionary<string, string>();

        //temp store for bulk indexing
        private static object PendingDocumentsLock = new object();
        private Dictionary<string, List<Document>> _pendingDocuments = new Dictionary<string, List<Document>>();

        // index mappings by domain etc 
        private static object IndexMappingsLock = new object();
        private Dictionary<string, string> _indexMappings = null;

        // ss fieldname to elastic type mappings
        private static object FieldMappingsLock = new object();
        private Dictionary<string, Dictionary<string, string>> _fieldMappings = null;

        private List<Locale> _allLocales;

        /// <summary>
        /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
        /// </summary>
        public SearchElasticService(LocaleService locales, PageService pageService, TagService tagService, CategoryService categoryService)
        {
            _locales = locales;
            _pageService = pageService;
            _tagService = tagService;
            _categoryService = categoryService;

            if (!IsConfigured())
            {
                return;
            }

            Events.Service.AfterStart.AddEventListener((Context context, object sender) =>
            {
                // This route is suggested rather than dependency injection
                // Because some projects (particularly fully headless and micro instances) don't have a page service installed.
                var pageService = Services.Get<PageService>();
                if (pageService != null)
                {
                    pageService.Install(
                        new Page()
                        {
                            Url = "/search",
                            Title = "Site Search",
                            ExcludeFromSearch = true,
                            BodyJson = @"{
	                            ""c"": [
		                            {
			                            ""t"": ""UI/ContentSearch/SearchContext"",
			                            ""d"": {
				                            ""comment"": ""SearchContext used to link search engine, results and filters""
			                            },
			                            ""c"": [
				                            {
					                            ""t"": ""UI/ContentSearch"",
					                            ""d"": {
						                            ""aggregationFields"": """",
						                            ""contentTypes"": """",
						                            ""title"": ""Search"",
						                            ""baseQuery"": """",
						                            ""sortOptions"": [],
						                            ""aggregationOverrides"": [],
						                            ""hideResults"": true,
						                            ""hideTitle"": true,
						                            ""showStaticFilters"": false,
						                            ""allFields"": false,
						                            ""inlineTitle"": true,
						                            ""mobileFilters"": false
					                            },
					                            ""i"": 4
				                            },
				                            {
					                            ""t"": ""UI/ContentSearch/ContentSearchResults"",
					                            ""d"": {
						                            ""stickyHeader"": false,
						                            ""useSiteCards"": true,
						                            ""hideTotal"": false,
						                            ""hideHeader"": true,
						                            ""mobileFilters"": false
					                            },
					                            ""i"": 14
				                            }
			                            ],
			                            ""i"": 13
		                            }
	                            ],
	                            ""i"": 3
                            }"
                        }
                    );
                }

                // subscribe to event requesting if the elastic store is connected
                Events.Elastic.IsConnected.AddEventListener(async (Context ctx, bool connected) =>
                {
                    return SetupClient(ctx);
                });

                // subscribe to event requesting that a new index is created 
                Events.Elastic.CreateIndex.AddEventListener(async (Context ctx, bool success, string indexName) =>
                {
                    return CreateIndex(indexName);
                });

                // subscribe to event requesting a document is deleted
                Events.Elastic.DeleteDocument.AddEventListener(async (Context ctx, bool success, string id, List<string> indexes) =>
                {
                    SetupClient(ctx);
                    if (_client != null)
                    {
                        return await DeleteDocument(ctx, id, indexes);
                    }

                    return false;
                });

                // subscribe to event requesting a document is deleted
                Events.Elastic.DeleteContentType.AddEventListener(async (Context ctx, bool success, string type, List<string> indexes) =>
                {
                    SetupClient(ctx);
                    if (_client != null)
                    {
                        return await DeleteDocumentsByType(ctx, type, indexes);
                    }

                    return false;
                });

                // subscribe to event requesting a document is indexed
                Events.Elastic.IndexDocument.AddEventListener(async (Context ctx, bool success, Document document, List<string> indexes) =>
                {
                    SetupClient(ctx);
                    if (_client != null)
                    {
                        return await IndexDocument(ctx, document, indexes);
                    }

                    return false;
                });

                // subscribe to event requesting a batch of documents are indexed
                Events.Elastic.IndexDocuments.AddEventListener(async (Context ctx, bool success, List<Document> documents, List<string> indexes) =>
                {
                    SetupClient(ctx);
                    if (_client != null)
                    {
                        return await IndexDocuments(ctx, documents, indexes);
                    }

                    return false;
                });

                // subscribe to site crawler which will extract pages for all locales
                Events.Crawler.PageCrawledNoPrimaryContent.AddEventListener(async (Context ctx, CrawledPageMeta pageMeta, SearchCrawlerMode mode) =>
                {
                    if ((mode & SearchCrawlerMode.Indexing) == SearchCrawlerMode.Indexing)
                    {
                        SetupClient(ctx);
                        if (_client != null)
                        {
                            var tags = await _tagService.ListBySource<Page, uint>(ctx, _pageService, pageMeta.Page.Id, "Tags", DataOptions.IgnorePermissions);

                            // see if we have any event handlers to extract custom metadata for the page
                            Dictionary<string, List<object>> metaData = null;
                            if (Events.Elastic.BeforeIndexingMeta.HasListeners())
                            {
                                metaData = await Events.Elastic.BeforeIndexingMeta.Dispatch(ctx, metaData, pageMeta, tags);
                            }

                            // get the baseline index(es) for the content
                            List<string> indexKeys = GetStorageIndexKeys(ctx, pageMeta.Url);

                            await ProcessPage<object>(ctx, pageMeta, tags, indexKeys, null, null, metaData);
                        }
                    }
                    return pageMeta;
                });

                // subscribe to site crawler status change event
                Events.Crawler.CrawlerStatus.AddEventListener(async (Context ctx, SearchCrawlerStatus status, SearchCrawlerMode mode) =>
                {
                    if ((mode & SearchCrawlerMode.Indexing) != SearchCrawlerMode.Indexing)
                    {
                        return status;
                    }

                    if (status == SearchCrawlerStatus.Started)
                    {
                        SetupClient(ctx);
                        if (_client != null)
                        {
                            // do we need the existing docs to compare checksums
                            if (!_cfg.AlwaysUpdateIndex)
                            {
                                GetIndexedDocuments(ctx);
                            }

                            // reset the cached list so that we can delete old content later
                            _processed = new ConcurrentDictionary<string, CrawledPageMeta>();

                            Log.Info(LogTag, "Page Indexing starting");
                        }
                    }

                    if (status == SearchCrawlerStatus.Completed)
                    {
                        SetupClient(ctx);
                        if (_client != null)
                        {
                            // reset the mappings to account for new documents
                            // and hence dynamic properties
                            lock (FieldMappingsLock)
                            {
                                _fieldMappings = null;
                            }

                            if (_cfg.UseBulkIndexing)
                            {
                                // save any remaining documents 
                                lock (PendingDocumentsLock)
                                {
                                    if (_pendingDocuments.Any())
                                    {
                                        foreach (var indexSet in _pendingDocuments)
                                        {
                                            if (indexSet.Value.Any())
                                            {
                                                var indexKeys = indexSet.Key.Split("::", StringSplitOptions.RemoveEmptyEntries).ToList();
                                                var success = IndexDocuments(ctx, indexSet.Value, indexKeys).Result;
                                            }
                                        }

                                        _pendingDocuments.Clear();
                                    }
                                }
                            }

                            // all done so get the content and delete any old pages
                            Purge();

                            Log.Ok(LogTag, "Page Indexing completed.");
                        }
                    }

                    return status;
                });

                return new ValueTask<object>(sender);
            });

            // subscribe to events triggered by content types so we can add index listeners 
            // ensure that any hybrid data type services are set to load after this
            // as  otherwise they listener wont be created 
            // add the attribute [LoadPriority(200)]

            Events.Service.AfterCreate.AddEventListener((Context ctx, AutoService service) =>
            {
                var setupForTypeMethod = GetType().GetMethod(nameof(SetupForType));

                if (service == null)
                {
                    return new ValueTask<AutoService>(service);
                }
                // Get the content type for this service and event group:
                var servicedType = service.ServicedType;
                if (servicedType == null)
                {
                    // Things like the ffmpeg service.
                    return new ValueTask<AutoService>(service);
                }

                // If it's a mapping type, ignore
                if (ContentTypes.IsAssignableToGenericType(servicedType, typeof(Mapping<,>)))
                {
                    return new ValueTask<AutoService>(service);
                }

                // Add List event:
                var setupType = setupForTypeMethod.MakeGenericMethod(new Type[] {
                    servicedType,
                    service.IdType
                });

                setupType.Invoke(this, new object[] {
                    service
                });
                return new ValueTask<AutoService>(service);
            });
        }

        /// <summary>
        /// Handler for content types to expose content related data such as tags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ID"></typeparam>
        /// <param name="service"></param>
        public void SetupForType<T, ID>(AutoService<T, ID> service)
            where T : Content<ID>, new()
            where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
        {
            if (_cfg.DebugToConsole)
            {
                Log.Info(LogTag, $"Attaching search indexing to '{service.EntityName}'");
            }

            // Content types that can be used by the page system all appear here.    
            // Let's hook up to the PageCrawled event which tells us when a page of this primary object type (and from this service) has been crawled:
            service.EventGroup.PageCrawled.AddEventListener(async (Context ctx, CrawledPageMeta pageMeta, T po, SearchCrawlerMode mode) =>
            {
                if ((mode & SearchCrawlerMode.Indexing) == SearchCrawlerMode.Indexing)
                {
                    SetupClient(ctx);
                    if (_client != null)
                    {
                        var tags = await _tagService.ListBySource<Page, uint>(ctx, _pageService, pageMeta.Page.Id, "Tags", DataOptions.IgnorePermissions);

                        // Page crawled! The PO is the given one
                        var poTags = await _tagService.ListBySource<T, ID>(ctx, service, po.Id, "Tags", DataOptions.IgnorePermissions);

                        // see if we have any event handlers to extract custom metadata such as includes 
                        Dictionary<string, List<object>> metaData = null;
                        if (service.EventGroup.BeforeIndexingMetaData.HasListeners())
                        {
                            metaData = await service.EventGroup.BeforeIndexingMetaData.Dispatch(ctx, metaData, pageMeta, tags.Union(poTags), po);
                        }

                        // get the baseline index(es) for the content
                        List<string> indexKeys = GetStorageIndexKeys(ctx, pageMeta.Url);

                        // see if based on the primary object we are altering the index
                        if (service.EventGroup.BeforeIndexGetKeys.HasListeners())
                        {
                            indexKeys = await service.EventGroup.BeforeIndexGetKeys.Dispatch(ctx, indexKeys, pageMeta, tags.Union(poTags), po);
                        }

                        // if one one the BeforeIndexGetKeys handlers has rejected/stopped the index process for this page/content
                        if (indexKeys == null)
                        {
                            return pageMeta;
                        }

                        await ProcessPage(ctx, pageMeta, tags.Union(poTags), indexKeys, po.Type, po, metaData);
                    }
                }
                return pageMeta;
            });
        }

        /// <summary>
        /// Remove any orphaned docs from the search index
        /// </summary>
        private void GetIndexedDocuments(Context ctx)
        {
            _existingDocHashes = new ConcurrentDictionary<string, string>();

            SetupClient(ctx);
            if (_client == null)
            {
                return;
            }

            foreach (var index in _indexMappings)
            {
                // get the indexed docs for the mapping
                var elasticDocs = GetAllDocuments(index.Value);

                foreach (var doc in elasticDocs)
                {
                    if (!string.IsNullOrWhiteSpace(doc.Hash) && !string.IsNullOrWhiteSpace(doc.CheckSum))
                    {
                        _existingDocHashes.TryAdd(doc.Hash, doc.CheckSum);
                    }
                }
            }
        }

        /// <summary>
        /// Get all the active locales 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private List<Locale> GetAllLocales(Context ctx)
        {
            if (_allLocales != null && _allLocales.Any())
            {
                return _allLocales;
            }

            // Get all the current locales:
            var locales = _locales.Where("").ListAll(ctx).Result;

            if (locales != null && locales.Any())
            {
                _allLocales = locales;
            }
            else
            {
                _allLocales = new List<Locale>();
            }

            return _allLocales;
        }

        /// <summary>
        /// Remove any orphaned docs from the search index
        /// </summary>
        private void Purge()
        {
            if (_processed.Any())
            {
                foreach (var index in _indexMappings)
                {
                    // get the indexed docs for the mapping
                    var elasticDocs = GetAllDocuments(index.Value);

                    foreach (var doc in elasticDocs)
                    {
                        // purge any docs in the index we have not just processed
                        if (!string.IsNullOrWhiteSpace(doc.Id) &&
                            !string.IsNullOrWhiteSpace(doc.Hash) &&
                            !_processed.ContainsKey(doc.Hash))
                        {
                            if (_cfg.DebugToConsole)
                            {
                                Log.Info(LogTag, $"Elastic Search - Deleting - {index.Value} {doc.Url} {doc.Hash} {doc.CheckSum}");
                            }
                            _client.Delete(new DeleteRequest(index.Value, doc.Id));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extract and index content from the current page
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pageDocument"></param>
        /// <param name="tags"></param>
        /// <param name="indexKeys"></param>
        /// <param name="contentType"></param>
        /// <param name="content"></param>
        /// <param name="metaData"></param>
        private async Task<bool> ProcessPage<T>(Context ctx, CrawledPageMeta pageDocument, IEnumerable<Tag> tags, List<string> indexKeys, string contentType, T content, Dictionary<string, List<object>> metaData)
        {
            // missing url invalid url
            if (string.IsNullOrWhiteSpace(pageDocument.Url))
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Info(LogTag, $"Elastic Search - Invalid - {ctx.LocaleId} [No Page Url] {pageDocument.Title} ");
                }
                return true;
            }

            // invalid url - page has no slug data so only multiple slashes
            if (pageDocument.Url.Length > 1 && pageDocument.Url.Count(x => (x == '/')) == pageDocument.Url.Length)
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Info(LogTag, $"Elastic Search - Invalid - {ctx.LocaleId} {pageDocument.Url} {pageDocument.Title} ");
                }
                return true;
            }

            var url = pageDocument.Url.ToLower().Trim();
            if (url.Length > 1 && url.EndsWith("/"))
            {
                url = url.TrimEnd(new[] { '/' }).Trim();
            }

            if (url.Contains(' '))
            {
                Log.Warn(LogTag, $"Elastic Search - [Warning] - Url Contains Space - {ctx.LocaleId} [{url}] {pageDocument.Title} ");
            }

            // do we need to alter the url based on target domain for example
            if (Events.Elastic.AfterUrl.HasListeners())
            {
                url = await Events.Elastic.AfterUrl.Dispatch(ctx, url, pageDocument);
            }

            var documentText = new List<string>();

            // use hashset to ignore duplicate headings
            var headings = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            if (pageDocument.BodyCompressedBytes != null && pageDocument.BodyCompressedBytes.Length > 0)
            {
                var page = new HtmlDocument();
                page.Load(pageDocument.GetBodyStream());

                page = await Events.Elastic.AfterPage.Dispatch(ctx, page);

                foreach (var node in page.DocumentNode.Descendants())
                {
                    if (_cfg.HeaderTags.Contains(node.Name.ToLower()) && !string.IsNullOrWhiteSpace(node.InnerText))
                    {
                        headings.Add(node.InnerText);
                    }

                    if (node.NodeType == HtmlNodeType.Text &&
                        node.ParentNode.Name != "script" &&
                        node.ParentNode.Name != "style"
                    )
                    {
                        documentText.Add(node.InnerText);
                    }
                }
            }

            // categorise tags
            var tagList = new List<string>();
            var taxonomy = new Dictionary<string, List<string>>();

            // extract out sub tags
            if (tags != null && tags.Any())
            {
                tags = await ExtractSubTags(ctx, tags);
            }

            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    if (!tagList.Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase))
                    {
                        tagList.Add(tag.Name);
                    }

                    var categories = await _categoryService.ListBySource(ctx, _tagService, tag.Id, "Categories", DataOptions.IgnorePermissions);
                    if (categories != null && categories.Any())
                    {
                        foreach (var category in categories)
                        {
                            var categoryName = category.Name.Replace(" ", "").ToLower();
                            if (taxonomy.ContainsKey(categoryName))
                            {
                                if (!taxonomy[categoryName].Contains(tag.Name, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    taxonomy[categoryName].Add(tag.Name);
                                }
                            }
                            else
                            {
                                taxonomy.Add(categoryName, new List<string>() { tag.Name });
                            }
                        }
                    }
                }
            }

            DateTime? editedUtc = null;
            if (typeof(UserCreatedContent<uint>).IsAssignableFrom(typeof(T)))
            {
                editedUtc = (content as UserCreatedContent<uint>).GetEditedUtc();
            }

            var document = new Document()
            {
                Id = url,
                Title = pageDocument.Title,
                Url = url,
                Hash = GetHash($"{ctx.LocaleId}-{url}"),
                Headings = headings,
                Content = string.Join(" ", documentText),
                ContentType = contentType,
                Tags = tagList.Count > 0 ? tagList : null,
                Taxonomy = taxonomy.Count > 0 ? taxonomy : null,
                Keywords = string.Join(" ", tags.Select(s => s.Name)),
                PrimaryObject = content,
                EditedUtc = editedUtc
            };

            // if we have any metadata/includes then add them in
            if (metaData != null && metaData.Any())
            {
                if (metaData.ContainsKey("metadataText"))
                {
                    document.MetaDataText = string.Join(" ", metaData["metadataText"]);
                    metaData.Remove("metadataText");
                }
                if (metaData.Any())
                {
                    document.MetaData = metaData;
                }
            }

            if (_cfg.UseBulkIndexing)
            {
                lock (PendingDocumentsLock)
                {
                    // extract index values into usable value for dictionary key
                    var key = string.Join("::", indexKeys);

                    if (_pendingDocuments.ContainsKey(key))
                    {
                        _pendingDocuments[key].Add(document);

                        if (_pendingDocuments[key].Count == _cfg.BulkIndexLimit)
                        {
                            var indexes = key.Split("::", StringSplitOptions.RemoveEmptyEntries).ToList();
                            var success = IndexDocuments(ctx, _pendingDocuments[key], indexes).Result;

                            _pendingDocuments[key] = new List<Document>();
                        }
                    }
                    else
                    {
                        _pendingDocuments.Add(key, new List<Document>() { document });
                    }
                }
            }
            else
            {
                // live update to index
                var success = await IndexDocument(ctx, document, indexKeys);
            }

            // keep track of the docs we have processed 
            _processed.TryAdd(document.Hash, pageDocument);

            return true;
        }

        /// <summary>
        /// Add document(s) to the index 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="documents"></param>
        /// <param name="indexKeys"></param>
        /// <returns></returns>
        private async Task<bool> IndexDocuments(Context ctx, List<Document> documents, List<string> indexKeys = null)
        {

            for (int i = 0; i < documents.Count; i++)
            {
                // allow other services to update the document before its saved
                documents[i] = await Events.Elastic.BeforeUpdate.Dispatch(ctx, documents[i]);

                // only needed if we are comparing for changes against indexed doc
                if (!_cfg.AlwaysUpdateIndex)
                {
                    documents[i].CheckSum = GetHash(documents[i]);
                }

                if (documents[i].TimeStamp == DateTime.MinValue)
                {
                    documents[i].TimeStamp = DateTime.UtcNow;
                }
            }

            if (indexKeys == null || !indexKeys.Any())
            {
                // get the baseline index(es) for the content
                indexKeys = GetStorageIndexKeys(ctx);
            }

            var success = false;

            foreach (var indexKey in indexKeys)
            {
                var indexName = _indexMappings.ContainsKey(indexKey) ? _indexMappings[indexKey] : indexKey;

                var bulkResponse = await _client.IndexManyAsync(documents, indexName);

                if (bulkResponse.IsValid && !bulkResponse.Errors)
                {
                    if (_cfg.ProgressToConsole)
                    {
                        Log.Info(LogTag, $"Elastic Search - Indexed - [{indexName}] {bulkResponse.Items.Count}");
                    }

                    if (!success)
                    {
                        success = true;
                    }
                }
                else
                {
                    var errors = string.Empty;

                    if (bulkResponse.OriginalException != null)
                    {
                        errors = bulkResponse.OriginalException.Message;
                    }

                    foreach (var item in bulkResponse.ItemsWithErrors)
                    {
                        errors += $" [{item.Id} = {item.Error.Reason}] ";
                    }

                    // Some items failed, inspect individual items
                    int successCount = bulkResponse.Items.Count(item => item.IsValid);
                    int failedCount = bulkResponse.Items.Count(item => !item.IsValid);

                    if (_cfg.DebugToConsole)
                    {
                        Log.Error(LogTag, null, $"Bulk Index Failed - [{indexName}] {successCount} {failedCount} {errors} {bulkResponse.DebugInformation}");
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Add document to the index 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="document"></param>
        /// <param name="indexKeys"></param>
        /// <returns></returns>
        private async Task<bool> IndexDocument(Context ctx, Document document, List<string> indexKeys = null)
        {
            // allow other services to update the document before its saved
            document = await Events.Elastic.BeforeUpdate.Dispatch(ctx, document);

            // only needed if we are comparing for changes against indexed doc
            if (!_cfg.AlwaysUpdateIndex)
            {
                document.CheckSum = GetHash(document);
            }

            // has the document changed ? 
            if (!_cfg.AlwaysUpdateIndex && _existingDocHashes.ContainsKey(document.Hash) && _existingDocHashes[document.Hash] == document.CheckSum)
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Info(LogTag, $"Elastic Search - Ignoring - {ctx.LocaleId} {document.Url} {document.Hash} {document.CheckSum}");
                }
                return true;
            }

            if (document.TimeStamp == DateTime.MinValue)
            {
                document.TimeStamp = DateTime.UtcNow;
            }

            if (indexKeys == null || !indexKeys.Any())
            {
                // get the baseline index(es) for the content
                indexKeys = GetStorageIndexKeys(ctx);
            }

            var success = false;

            foreach (var indexKey in indexKeys)
            {
                var indexName = _indexMappings.ContainsKey(indexKey) ? _indexMappings[indexKey] : indexKey;

                // add the document into index
                var response = await _client.IndexAsync(document, request => request.Index(indexName));

                if (response.IsValid)
                {
                    if (_cfg.ProgressToConsole)
                    {
                        Log.Info(LogTag, $"Elastic Search - Indexed - [{indexName}] {document.Url}");
                    }

                    if (!success)
                    {
                        success = true;
                    }
                }
                else if (_cfg.DebugToConsole)
                {
                    Log.Error(LogTag, null, $"Index Failed - [{indexName}] {document.Url} {response.ServerError} {response.DebugInformation}");
                }
            }

            return success;
        }

        /// <summary>
        /// Remove a document from the index 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <param name="indexKeys"></param>
        /// <returns></returns>
        private async Task<bool> DeleteDocument(Context ctx, string id, List<string> indexKeys = null)
        {
            if (indexKeys == null || !indexKeys.Any())
            {
                // get the baseline index(es) for the content
                indexKeys = GetStorageIndexKeys(ctx);
                indexKeys.AddRange(GetStorageIndexKeys(ctx));
            }

            var success = false;

            foreach (var indexKey in indexKeys)
            {
                var indexName = _indexMappings.ContainsKey(indexKey) ? _indexMappings[indexKey] : indexKey;

                // remove the document into index
                var response = await _client.DeleteAsync<object>(id, d => d.Index(indexName));

                if (response.IsValid)
                {
                    if (_cfg.ProgressToConsole)
                    {
                        Log.Info(LogTag, $"Elastic Search - Deleted - [{indexName}] {id}");
                    }

                    if (!success)
                    {
                        success = true;
                    }
                }
                else
                {
                    if (_cfg.DebugToConsole) Log.Error(LogTag, null, $"Delete Failed - [{indexName}] {id} {response.ServerError} {response.DebugInformation}");
                }
            }

            return success;
        }

        /// <summary>
        /// Remove documents from the index based on content type
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="type"></param>
        /// <param name="indexKeys"></param>
        /// <returns></returns>
        private async Task<bool> DeleteDocumentsByType(Context ctx, string type, List<string> indexKeys = null)
        {
            if (indexKeys == null || !indexKeys.Any())
            {
                // get the baseline index(es) for the content
                indexKeys = GetStorageIndexKeys(ctx);
            }

            var success = false;

            foreach (var indexKey in indexKeys)
            {
                var indexName = _indexMappings.ContainsKey(indexKey) ? _indexMappings[indexKey] : indexKey;

                var deleteResponse = await _client.DeleteByQueryAsync<object>(del => del
                    .Index(indexName)
                    .Query(q => q
                        .Term(t => t
                            .Field("contentType")
                            .Value(type)
                        )
                    )
                );

                if (deleteResponse.IsValid)
                {
                    Log.Info(LogTag, $"Elastic Search - Deleted Content Type - [{indexName}] {type} {deleteResponse.Deleted}");
                    if (!success)
                    {
                        success = true;
                    }
                }
                else if (_cfg.DebugToConsole)
                {
                    Log.Error(LogTag, null, $"Delete Failed - [{indexName}] {type} {deleteResponse.ServerError} {deleteResponse.DebugInformation}");
                }
            }

            return success;
        }



        /// <summary>
        /// Get all the possible taxonomy values forom categories/tags
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<AggregationOverride> ExtractTaxonomy(Context ctx, string name)
        {
            // see if we have a category to extract the tags from 
            var category = await _categoryService.Where("Name=?").Bind(name).First(ctx);
            if (category == null)
            {
                return null;
            }

            List<Tag> tags;

            // Need to get tags that respect the users CurrencyLocale
            if (ctx.LocaleId != ctx.CurrencyLocaleId && ctx.CurrencyLocaleId > 0)
            {
                var f = _tagService.Where("Categories=? AND IsPrice=?", DataOptions.IgnorePermissions);
                f.Sort("Name");
                f.PageSize = 500;
                f.Bind(category.Id);
                f.Bind(false);
                tags = await f.ListAll(ctx);

                var currencyContext = new Context(ctx.Role)
                {
                    LocaleId = ctx.CurrencyLocaleId
                };

                var fl = _tagService.Where("Categories=? AND IsPrice=?", DataOptions.IgnorePermissions);
                fl.Sort("Name");
                fl.PageSize = 500;
                fl.Bind(category.Id);
                fl.Bind(true);
                var priceTagsByLocale = await fl.ListAll(ctx);

                var fcl = _tagService.Where("Categories=? AND IsPrice=?", DataOptions.IgnorePermissions);
                fcl.Sort("Name");
                fcl.PageSize = 500;
                fcl.Bind(category.Id);
                fcl.Bind(true);
                var priceTagsByCurrencyLocale = await fcl.ListAll(currencyContext);

                if (priceTagsByLocale != null && priceTagsByLocale.Any() && priceTagsByCurrencyLocale != null && priceTagsByCurrencyLocale.Any())
                {
                    foreach (var priceTag in priceTagsByLocale)
                    {
                        var currencyLocaleTag = priceTagsByCurrencyLocale.FirstOrDefault(clt => clt.Id == priceTag.Id);

                        if (currencyLocaleTag != null)
                        {
                            // Make sure the label reflects the users currency locale i.e. show $ values instead of £ values
                            priceTag.Description = !string.IsNullOrEmpty(currencyLocaleTag.Description) ? currencyLocaleTag.Description : currencyLocaleTag.Name;
                        }

                        tags.Add(priceTag);
                    }
                }
            }
            else
            {
                var f = _tagService.Where("Categories=?", DataOptions.IgnorePermissions);
                f.Sort("Name");
                f.PageSize = 500;
                f.Bind(category.Id);

                tags = await f.ListAll(ctx);
            }

            if (tags == null || !tags.Any())
            {
                return null;
            }

            // Storage of group tag and sub tags
            // For example the key tag might be a region, with a list if country/destination tags
            // Using tags to allow for sorting etc

            var grouped = new Dictionary<Tag, List<Tag>>();

            // determine if tags are grouped by category
            foreach (var tag in tags)
            {
                List<string> parents = new List<string>();

                // check for sub tags (does it have a single parent category)
                var subTags = await _tagService.ListBySource(ctx, _tagService, tag.Id, "Tags", DataOptions.IgnorePermissions);
                if (subTags != null && subTags.Count == 1)
                {
                    foreach (var subTag in subTags)
                    {
                        // do the linked tags have a single category ? 
                        var categories = await _categoryService.ListBySource(ctx, _tagService, subTag.Id, "Categories", DataOptions.IgnorePermissions);
                        if (categories != null && categories.Count == 1)
                        {
                            if (!parents.Contains(categories.First().Name, StringComparer.InvariantCultureIgnoreCase))
                            {
                                parents.Add(categories.First().Name);
                            }
                        }
                    }
                }

                // now either add into group bucket or to the end
                if (subTags.Count == 1 && parents.Count == 1)
                {
                    // get the grouping/parent tag
                    var parentTag = subTags.First();

                    // do we already have the parent/group tag 
                    if (grouped.Any(gt => gt.Key.Id == parentTag.Id))
                    {
                        // check for duplicate values
                        if (!grouped.First(gt => gt.Key.Id == parentTag.Id).Value.Any(t => t.Id == tag.Id))
                        {
                            grouped.First(gt => gt.Key.Id == parentTag.Id).Value.Add(tag);
                        }
                    }
                    else
                    {
                        // new group so add
                        grouped.Add(parentTag, new List<Tag>() { tag });
                    }
                }
                else
                {
                    // do we already have the generic tag container
                    if (grouped.Any(t => t.Key.Name == ""))
                    {
                        // check for duplicate values
                        if (!grouped.First(t => t.Key.Name == "").Value.Any(t => t.Id == tag.Id))
                        {
                            grouped.First(t => t.Key.Name == "").Value.Add(tag);
                        }
                    }
                    else
                    {
                        // first generic tag so add container
                        grouped.Add(new Tag(), new List<Tag>() { tag });
                    }
                }
            }

            if (!grouped.Any())
            {
                return null;
            }

            var aggregationOverride = new AggregationOverride();

            // no grouping (as no categories found)
            if (grouped.Count == 1 && grouped.Any(g => g.Key.Name == "") && grouped.First(g => g.Key.Name == "").Value.Count > 0)
            {
                aggregationOverride.Buckets = new List<Bucket>();

                foreach (var tag in grouped.First(g => g.Key.Name == "").Value.OrderBy(t => t.Order).ThenBy(t => t.Name))
                {
                    aggregationOverride.Buckets.Add(new Bucket()
                    {
                        Label = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(tag.Description) ? tag.Name : tag.Description),
                        Key = tag.Name
                    }
                    );
                }
                return aggregationOverride;
            }

            // must be grouped 
            aggregationOverride.Groups = new List<Group>();
            foreach (var grp in grouped.OrderBy(g => g.Key.Order).ThenBy(g => g.Key.Name))
            {
                // ignore empty buckets
                if (!grp.Value.Any())
                {
                    continue;
                }

                var group = new Group()
                {
                    Name = grp.Key.Name,
                    Buckets = new List<Bucket>()
                };
                aggregationOverride.Groups.Add(group);

                foreach (var tag in grp.Value.OrderBy(t => t.Order).ThenBy(t => t.Name))
                {
                    group.Buckets.Add(new Bucket()
                    {
                        Label = HttpUtility.HtmlDecode(string.IsNullOrWhiteSpace(tag.Description) ? tag.Name : tag.Description),
                        Key = tag.Name
                    }
                    );
                }
            }

            if (!aggregationOverride.Groups.Any())
            {
                // no meaningful buckets so nullify
                aggregationOverride = null;
            }

            return aggregationOverride;
        }

        /// <summary>
        /// Extract any sub tags so for example destinations within a region
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Tag>> ExtractSubTags(Context ctx, IEnumerable<Tag> tags)
        {
            List<Tag> newTags = tags.ToList();

            foreach (var tag in tags)
            {
                var subTags = await _tagService.ListBySource(ctx, _tagService, tag.Id, "Tags", DataOptions.IgnorePermissions);
                if (subTags != null && subTags.Any())
                {
                    foreach (var subTag in subTags)
                    {
                        if (!newTags.Any(t => t.Id == subTag.Id))
                        {
                            newTags.Add(subTag);
                        }
                    }
                }
            }
            return newTags;
        }


        /// <summary>
        /// Get all the possible taxonomy values from content for a field 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private async Task<AggregationOverride> ExtractContentTaxonomy(Context ctx, string type, string field)
        {
            // see if we have a category to extract the tags from 
            var contentService = Services.Get(type + "Service");
            if (contentService == null)
            {
                return null;
            }

            var tags = await contentService.ExtractTaxonomy(ctx, field);

            if (tags == null || !tags.Any())
            {
                return null;
            }

            var aggregationOverride = new AggregationOverride();

            aggregationOverride.Buckets = new List<Bucket>();

            foreach (var tag in tags.OrderBy(t => t.Order).ThenBy(t => t.Name))
            {
                if (!aggregationOverride.Buckets.Any(b => b.Key == tag.Name))
                {
                    aggregationOverride.Buckets.Add(new Bucket()
                    {
                        Label = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(tag.Description) ? tag.Name : tag.Description),
                        Key = tag.Name
                    }
                    );
                }
            }
            return aggregationOverride;
        }

        /// <summary>
        /// Gets an md5 lowercase hash for the given content.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetHash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private string GetHash(Document input)
        {
            // Use object to calculate MD5 hash/fingerprint
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var jsonObject = JsonConvert.SerializeObject(input, Formatting.None);
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(jsonObject));

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private List<Document> GetAllDocuments(string index)
        {
            List<Document> indexedList = new List<Document>();

            var scanResults = _client.Search<Document>(s => s
                            .Index(index)
                            .From(0)
                            .Size(1000)
                            .Source(sf => sf
                                .Includes(i => i
                                    .Fields(
                                        f => f.Id,
                                        f => f.Url,
                                        f => f.Hash,
                                        f => f.CheckSum
                                    )
                                )
                            )
                            .Scroll("5m")
                        );

            if (scanResults != null && scanResults.Documents.Any())
            {
                var documents = scanResults?.Documents;

                while (documents.Any())
                {
                    indexedList.AddRange(documents);

                    var scrollRequest = new ScrollRequest(scanResults.ScrollId, "5m");
                    documents = _client.Scroll<Document>(scrollRequest).Documents;
                }
            }
            return indexedList;
        }

        /// <summary>
        /// Get the active list of indexes
        /// </summary>
        private void GetAllIndexes()
        {
            if (_indexMappings != null)
            {
                return;
            }

            lock (IndexMappingsLock)
            {
                _indexMappings = new Dictionary<string, string>();

                var ctx = new Context();

                // Get all the current locales:
                var all_locales = GetAllLocales(ctx);

                // For each locale..
                foreach (var locale in all_locales)
                {
                    ctx.LocaleId = locale.Id;

                    var prefix = $"{ctx.LocaleId}";

                    var indexname = $"{ctx.LocaleId}-{_cfg.IndexName}";

                    _indexMappings.Add(prefix, indexname);

                    // allow other services to extend the list of index mappings
                    // such as when we have subsites/domains
                    _indexMappings = Events.Elastic.AfterGetAllIndexes.Dispatch(ctx, _indexMappings, prefix, indexname).Result;
                }
            }
        }

        /// <summary>
        /// Get the retrieval index for the page/url
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="url"></param>
        /// <returns></returns>        
        private string GetIndexName(Context ctx, string url = null)
        {
            if (_indexMappings.Count == 1)
            {
                return _indexMappings.First().Value;
            }

            var indexKey = $"{ctx.LocaleId}";

            // allow other services to deterime the key 
            // such as by site domain for example
            indexKey = Events.Elastic.AfterIndexKey.Dispatch(ctx, indexKey, url).Result;

            return _indexMappings[indexKey];
        }

        /// <summary>
        /// Get the storage index keys for the page/url
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="url"></param>
        /// <returns></returns>        
        private List<string> GetStorageIndexKeys(Context ctx, string url = null)
        {
            var indexKeys = new List<string>()
            {
                $"{ctx.LocaleId}"
            };

            // allow other services to extend the key 
            // such as by site domain for example
            return Events.Elastic.AfterStorageIndexKeys.Dispatch(ctx, indexKeys, url).Result;
        }

        private bool IsConfigured()
        {
            _cfg = GetConfig<SearchElasticServiceConfig>();
            return !string.IsNullOrWhiteSpace(_cfg.InstanceUrl) && !_cfg.Disabled;
        }

        private bool SetupClient(Context ctx)
        {
            if (_client != null)
            {
                GetAllIndexes();

                return true;
            }

            _cfg = GetConfig<SearchElasticServiceConfig>();

            // Not configured
            if (string.IsNullOrWhiteSpace(_cfg.InstanceUrl))
            {
                return false;
            }

            // build map of the indexes
            GetAllIndexes();

            var pool = new SingleNodeConnectionPool(new Uri(_cfg.InstanceUrl));

            var settings = new ConnectionSettings(pool)
                    .CertificateFingerprint(_cfg.FingerPrint)
                    .BasicAuthentication(_cfg.UserName, _cfg.Password)
                    .EnableApiVersioningHeader();

            if (_cfg.DebugToConsole)
            {
                settings.EnableDebugMode();
            }

            _client = new ElasticClient(settings);

            try
            {
                if (_client != null)
                {
                    var ping = _client.Ping();
                    if (ping != null && ping.IsValid)
                    {
                        // create initial indexes 
                        CreateCoreIndexes();

                        var connected = true;
                        connected = Events.Elastic.AfterConnected.Dispatch(ctx, connected, _cfg).Result;
                        return connected;
                    }
                    else
                    {
                        Log.Error(LogTag, null, $"Failed to connect to Elastic Server {(_cfg.DebugToConsole ? ping.DebugInformation : "")}");
                    }
                }
            }
            catch (Exception e)
            {
                if (_cfg.DebugToConsole)
                {
                    Log.Error(LogTag, e, "Failed to connect to Elastic Server");
                }
            }

            _client = null;
            return false;
        }

        private void CreateCoreIndexes()
        {
            if (_client == null && _indexMappings == null)
            {
                return;
            }

            foreach (var index in _indexMappings)
            {
                CreateIndex(index.Value);
            }
        }

        /// <summary>
        /// create a new index 
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        private bool CreateIndex(string indexName)
        {
            if (_client == null)
            {
                return false;
            }

            var indexExists = _client.Indices.Exists(indexName);
            if (indexExists.Exists)
            {
                return true;
            }

            // number of shards and indexes are set as part of index creation
            var createIndexResponse = _client.Indices.Create(indexName, c => c
            .Settings(s => s
                .Setting("index.mapping.total_fields.limit", _cfg.MappingFieldsLimit)
                .NumberOfReplicas(_cfg.Replicas)
                .NumberOfShards(_cfg.Shards)
                .Analysis(a => a
                    .Normalizers(n => n.Custom("n_insensitive_ascii_folding", c => c.Filters("lowercase", "asciifolding")))
                    .TokenFilters(t => t.AsciiFolding("my_ascii_folding", c => c.PreserveOriginal()))
                    .Analyzers(a => a.Custom("a_standard_insensitive_ascii_folding", c => c.Tokenizer("standard").Filters("lowercase", "asciifolding")))
                )
            )
            .Map<Document>(m => m
                .AutoMap<Document>()
                    .Properties(p => p
                        .Text(st => st
                            .Name("primaryObject.name") // allow for case-insensitive matching/sorting etc 
                            .Fields(f => f
                                .Keyword(kw => kw.Name("keyword").Normalizer("n_insensitive_ascii_folding"))
                            )
                            .Analyzer("a_standard_insensitive_ascii_folding")
                        )
                        .Text(st => st
                            .Name(n => n.Title) // allow for case-insensitive matching/sorting etc 
                            .Analyzer("a_standard_insensitive_ascii_folding")
                        )
                        .Text(st => st
                            .Name(n => n.Content) // allow for case-insensitive matching/sorting etc 
                            .Analyzer("a_standard_insensitive_ascii_folding")
                        )
                    )
                )
            );

            return createIndexResponse.IsValid;
        }

        /// <summary>
        /// Delete a single index
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteIndex(Context ctx, string indexName)
        {
            SetupClient(ctx);
            if (_client == null)
            {
                return false;
            }

            var response = await _client.Indices.DeleteAsync(indexName);

            if (response.IsValid)
            {
                Log.Info(LogTag, $"Index deleted {indexName}.");

                // let other services know, to trigger reindexing etc 
                var reset = true;
                reset = await Events.Elastic.AfterReset.Dispatch(ctx, reset, indexName);
            }
            else
            {
                Log.Error(LogTag, null, $"Failed to delete index {indexName} : {response.ServerError}-{response.DebugInformation}");
            }

            return response.IsValid;
        }


        /// <summary>
        /// Delete the contents of a single index
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Reset(Context ctx, string indexName)
        {
            SetupClient(ctx);
            if (_client == null)
            {
                return false;
            }

            var response = await _client.DeleteByQueryAsync<object>(del => del
                .Index(indexName)
                .Query(q => q.MatchAll()
            ));

            if (response.IsValid)
            {
                // let other services know, to trigger reindexing etc 
                var reset = true;
                reset = await Events.Elastic.AfterReset.Dispatch(ctx, reset, indexName);

                Log.Info(LogTag, $"All documents deleted successfully from index {indexName}.");
            }
            else
            {
                Log.Error(LogTag, null, $"Failed to delete documents from index {indexName} : {response.ServerError}-{response.DebugInformation}");
            }

            return response.IsValid;
        }

        /// <summary>
        /// Delete all the indexes
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Reset(Context ctx)
        {
            SetupClient(ctx);
            if (_client == null)
            {
                return false;
            }

            foreach (var index in _indexMappings)
            {
                await _client.Indices.DeleteAsync(index.Value);
            }

            Log.Info(LogTag, $"Core search indexes deleted");

            // nullify the client so the mappings are recreated
            _client = null;

            // let other services know, to trigger reindexing etc (all indexes)
            var reset = true;
            reset = await Events.Elastic.AfterReset.Dispatch(ctx, reset, string.Empty);
            return reset;
        }

        /// <summary>
        /// Show the elastic instance health
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task<ClusterHealthResponse> Health(Context ctx)
        {
            SetupClient(ctx);
            if (_client == null)
            {
                return null;
            }

            return _client.Cluster.Health();
        }

        public async Task<List<CatShardsRecord>> Shards(Context ctx)
        {
            SetupClient(ctx);
            if (_client == null)
            {
                return null;
            }

            return _client.Cat.Shards(s => s.Verbose().AllIndices().Pretty().Human().SortByColumns("state")).Records.ToList();
        }



        /// <summary>
        /// Perform a basic search with highlighted results for title/content
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="indexName"></param>
        /// <param name="query"></param>
        /// <param name="tags"></param>
        /// <param name="contentTypes"></param>
        /// <param name="aggregations"></param>
        /// <param name="sortField"></param>
        /// <param name="sortOrder"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="allFields"></param>
        /// <returns></returns>
        public async Task<DocumentsResult> Query(Context ctx, string indexName, string query, string tags, string contentTypes, string aggregations, string sortField = "_score", string sortOrder = "descending", int pageIndex = 0, int pageSize = 10, bool allFields = false)
        {
            System.Diagnostics.Stopwatch timer = null;

            if (_cfg.DebugQueries)
            {
                timer = System.Diagnostics.Stopwatch.StartNew();
            }

            SetupClient(ctx);
            if (_client == null)
            {
                return new DocumentsResult();
            }

            if (string.IsNullOrWhiteSpace(indexName))
            {
                indexName = GetIndexName(ctx);
            }

            // set the start point (not a pageindex)
            var from = pageIndex <= 1 ? 0 : (pageIndex - 1) * pageSize;

            SearchDescriptor<Document> search;
            var filters = new List<Func<QueryContainerDescriptor<Document>, QueryContainer>>();
            var aggregationDictionary = new AggregationDictionary();

            // build up tag filter 
            if (!string.IsNullOrWhiteSpace(tags))
            {
                foreach (var tag in tags.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    filters.Add(fq => fq.Term(t => t.Field(f => f.Tags).Value(tag)));
                }
            }

            // build up content type filter 
            if (!string.IsNullOrWhiteSpace(contentTypes))
            {
                var typeList = contentTypes.Split(",", StringSplitOptions.RemoveEmptyEntries);
                filters.Add(fq => fq.Terms(t => t.Field(f => f.ContentType).Terms(typeList)));
            }

            // misc filters can be added into the querystring 
            // e.g. 
            // (primaryObject.price:>=5000 AND primaryObject.price:<1000)
            // or 
            // metaData.car.name:mini
            // if includes are indexed to expose metaData elements (see SearchIndexingService)

            // build up any aggregations/facets
            if (!string.IsNullOrWhiteSpace(aggregations))
            {
                Dictionary<string, string> indexMappings = GetIndexMappings(ctx, indexName);

                foreach (var aggregation in aggregations.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    var keywordFieldName = aggregation;

                    if (!keywordFieldName.EndsWith(".keyword") &&
                        (keywordFieldName.StartsWith("taxonomy.") || (indexMappings.ContainsKey(aggregation) && indexMappings[aggregation] == "Nest.TextProperty")))
                    {
                        keywordFieldName = $"{aggregation}.keyword";
                    }

                    aggregationDictionary.Add(aggregation,
                        new TermsAggregation(aggregation)
                        {
                            Field = keywordFieldName.StartsWith("taxonomy.") ? keywordFieldName.Replace(" ", "").ToLower() : keywordFieldName,
                            Size = _cfg.FacetLimit
                        }
                    );
                }
            }

            // define sort order of results
            var sortOrderBy = SortOrder.Descending;
            if (sortOrder.Equals("Ascending", StringComparison.InvariantCultureIgnoreCase))
            {
                sortOrderBy = SortOrder.Ascending;
            }

            Func<SortDescriptor<Document>, Nest.IPromise<IList<ISort>>> sorting;

            if (string.IsNullOrWhiteSpace(sortField) || sortField == "_score")
            {
                sorting = so => so
                        .Field(f => f
                            .Field(sortField)
                            .Order(sortOrderBy)
                        )
                        .Ascending(SortSpecialField.DocumentIndexOrder);
            }
            else
            {
                sorting = so => so
                        .Field(f => f
                            .Field(sortField)
                            .Order(sortOrderBy)
                        )
                        .Descending(SortSpecialField.Score)
                        .Ascending(SortSpecialField.DocumentIndexOrder);
            }

            Func<FieldsDescriptor<Document>, Nest.IPromise<Fields>> fields = null;

            // only search on the 'content' fields with ranking
            if (!allFields)
            {
                fields = f => f
                        .Field(f => f.Title, 5)
                        .Field(f => f.Headings, 3)
                        .Field(f => f.Keywords, 10)
                        .Field(f => f.Content, 2)
                        .Field(f => f.MetaDataText);
            }


            search = new SearchDescriptor<Document>()
                .Index(indexName)
                .Explain()
                .Query(qu => qu
                    .Bool(b => b
                        .Filter(filters)
                        .Must(must => must
                            .QueryString(qs => qs
                               .Query(query)
                               .Fields(fields)
                            )
                        )
                    )
                )
                .Sort(sorting)
                .From(from)
                .Size(pageSize)
                .Aggregations(aggregationDictionary)
                .Highlight(h => h
                     .Fields(f => f.Field("*"))
                    .PreTags("<mark>")
                    .PostTags("</mark>")
                );

            var response = await _client.SearchAsync<Document>(search);

            var documentResults = new DocumentsResult()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalResults = 0
            };

            if (_cfg.DebugQueries)
            {
                timer.Stop();
                if (response.IsValid)
                {
                    Log.Info(LogTag, $"Elastic query [{query}] [{tags}] [{contentTypes}] [{aggregations}] [{response.Hits?.Count} rows] took {timer.ElapsedMilliseconds}ms");
                } else
                {
                    Log.Warn(LogTag, $"Elastic query FAILED [{query}] [{tags}] [{contentTypes}] [{aggregations}] [{response.DebugInformation}] took {timer.ElapsedMilliseconds}ms");
                }
            }

            if (response.IsValid && response.Hits.Any())
            {
                documentResults.TotalResults = response.HitsMetadata.Total.Value;
                documentResults.Results = MapResults(response);

                // extract out any aggregations and their values
                if (response.Aggregations != null && response.Aggregations.Any())
                {
                    var aggregationList = new List<Aggregation>();

                    // return aggregations in the sequence in whiuch they are requested
                    foreach (var aggregationName in aggregations.Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        foreach (var aggregation in response.Aggregations.Where(a => a.Key == aggregationName))
                        {
                            aggregationList.Add(new Aggregation()
                            {
                                Name = aggregation.Key,
                                Label = CapitalizeFirstLetterInEachWord(Formatlabel(aggregation.Key)),
                                Buckets = response.Aggregations
                                .Terms(aggregation.Key)
                                .Buckets
                                .Select(bucket => new Bucket()
                                {
                                    Key = !string.IsNullOrWhiteSpace(bucket.KeyAsString) ? bucket.KeyAsString : bucket.Key,
                                    Count = bucket.DocCount
                                })
                                .OrderBy(s => s.Key)
                                .ToList()
                            });
                        }
                    }
                    documentResults.Aggregations = aggregationList;
                }
            }
            else
            {
                //setup up empty buckets so that defaults could be rendered
                var aggregationList = new List<Aggregation>();

                // return aggregations in the sequence in whiuch they are requested
                foreach (var aggregationName in aggregations.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    aggregationList.Add(new Aggregation()
                    {
                        Name = aggregationName,
                        Label = CapitalizeFirstLetterInEachWord(Formatlabel(aggregationName)),
                        Buckets = new List<Bucket>()
                    });
                }
                documentResults.Aggregations = aggregationList;
            }


            return documentResults;
        }

        /// <summary>
        /// Get the baseline taxonomy values from categories and tags
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<AggregationStructure> Taxonomy(Context ctx, string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return null;
            }

            var aggregationStructure = new AggregationStructure()
            {
                AggregationOverrides = new List<AggregationOverride>()
            };

            foreach (var field in fields.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                if (field.StartsWith("taxonomy."))
                {
                    var aggregationName = CapitalizeFirstLetterInEachWord(field.Replace("taxonomy.", string.Empty));

                    var aggregationOverride = await ExtractTaxonomy(ctx, aggregationName);

                    if (aggregationOverride != null)
                    {
                        aggregationOverride.Name = field;
                        aggregationOverride.Label = aggregationName;

                        aggregationStructure.AggregationOverrides.Add(aggregationOverride);
                    }
                }

            }

            if (!aggregationStructure.AggregationOverrides.Any())
            {
                return null;
            }

            return aggregationStructure;
        }

        private string Formatlabel(string label)
        {
            if (label.StartsWith("metaData."))
            {
                //metaData.{table}.{fieldname}
                var elements = label.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (elements.Length >= 3)
                {
                    return string.Join(" ", elements.Skip(2)).Replace(".", " ");
                }

                return label;
            }


            return label.Replace("PrimaryObject.", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace("Taxonomy.", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(".", " ");
        }

        private string CapitalizeFirstLetterInEachWord(string sentence)
        {
            if (String.IsNullOrEmpty(sentence.Trim()))
                return "";

            sentence = Regex.Replace(sentence, @"\s+", " ");
            var words = sentence.Trim().Split(" ");

            for (var i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1).ToLower();
            }
            return String.Join(" ", words);
        }

        private Dictionary<string, string> GetIndexMappings(Context ctx, string indexName)
        {
            if (_fieldMappings != null && _fieldMappings.ContainsKey(indexName))
            {
                return _fieldMappings[indexName];
            }

            SetupClient(ctx);
            if (_client == null)
            {
                return new Dictionary<string, string>();
            }

            lock (FieldMappingsLock)
            {
                var indexFieldMappings = new Dictionary<string, string>();

                // need to get the mapping types for dynamic objects 
                // text fields have child a .keyword element which must be used for aggregations
                var result = _client.Indices.GetMapping(new GetMappingRequest(indexName));
                if (result.IsValid)
                {
                    var mappings = result.GetMappingFor(indexName);

                    if (mappings != null && mappings.Properties.ContainsKey("primaryObject"))
                    {
                        var poField = (ObjectProperty)mappings.Properties.First(s => s.Key == "primaryObject").Value;

                        if (poField.Properties != null)
                        {
                            poField.Properties
                                .ToDictionary(s => "primaryObject." + s.Key.ToString(), s => s.Value.GetType().ToString())
                                .ToList()
                                .ForEach(x => indexFieldMappings.Add(x.Key, x.Value));
                        }
                    }

                    if (mappings != null && mappings.Properties.ContainsKey("metaData"))
                    {
                        var poField = (ObjectProperty)mappings.Properties.First(s => s.Key == "metaData").Value;

                        if (poField.Properties != null)
                        {
                            foreach (var mf in poField.Properties)
                            {
                                // ignore the count field
                                if (mf.Key == "count" && mf.Value.Type == "integer")
                                {
                                    continue;
                                }

                                try
                                {
                                    var subFields = (ObjectProperty)poField.Properties.First(s => s.Key == $"{mf.Key}").Value;
                                    if (subFields != null)
                                    {
                                        if (subFields.Properties == null || !subFields.Properties.Any())
                                        {
                                            continue;
                                        }

                                        subFields.Properties
                                        .ToDictionary(s => $"metaData.{mf.Key}.{s.Key}", s => s.Value.GetType().ToString())
                                        .ToList()
                                        .ForEach(x => indexFieldMappings.Add(x.Key, x.Value));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Warn(LogTag, $"GetIndexMappings - Invalid MetaData Mapping - [{mf.Key}] [{mf.Value.Type}] {e.Message} {e.InnerException?.Message}");
                                }
                            }
                        }
                    }
                }

                if (indexFieldMappings.Any())
                {
                    if (_fieldMappings == null)
                    {
                        _fieldMappings = new Dictionary<string, Dictionary<string, string>>();
                    }

                    if (_fieldMappings.ContainsKey(indexName))
                    {
                        _fieldMappings.Remove(indexName);
                    }

                    _fieldMappings.Add(indexName, indexFieldMappings);
                }
            }

            if (_fieldMappings != null && _fieldMappings.ContainsKey(indexName))
            {
                return _fieldMappings[indexName];
            }

            return new Dictionary<string, string>();
        }

        private List<Document> MapResults(ISearchResponse<Document> searchResult)
        {
            var results = new List<Document>();

            foreach (var hit in searchResult.Hits)
            {
                var document = hit.Source;

                // Merge highlights and add the match to the result list
                var titlehighlight = string.Join(" ", hit.Highlight.Where(h => h.Key == "title").SelectMany(h => h.Value));
                if (!string.IsNullOrWhiteSpace(titlehighlight))
                {
                    document.Title = titlehighlight;
                }

                document.Highlights = string.Join(" ", hit.Highlight.Where(h => h.Key == "content").SelectMany(h => h.Value));

                document.Score = hit.Score;
                results.Add(document);

            }
            return results;
        }

    }
}

