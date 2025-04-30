using Api.Automations;
using Api.CanvasRenderer;
using Api.Contexts;
using Api.Database;
using Api.Eventing;
using Api.SearchElastic;
using Api.Startup;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Api.Users;

namespace Api.SearchEntities
{
    /// <summary>
    /// Index content for admin panel search
    /// </summary>

    [LoadPriority(9)]
    [HostType("index")]
    [HostType("web")]
    public partial class SearchEntityService : AutoService<SearchEntity>
    {
        private SearchEntityConfig _cfg;
        private readonly FrontendCodeService _frontendService;

        // private Random random;

        // removed any similar chars like 1/ I 5/S 0/O etc 
        private const string refPattern = "ACEFHJKMNPRTUVWXY23456789";

        /// <summary>
        /// The index task key.
        /// </summary>
        public const string fullIndexTaskKey = "entity_index";

        /// <summary>
        /// The index updater task key.
        /// </summary>
        public const string incrementalIndexTaskKey = "entity_index_update";

        /// <summary>
        /// Service to locate newly added entities and index them 
        /// </summary>
        /// 

        public SearchEntityService(FrontendCodeService frontend) : base(Events.SearchEntity)
        {
            _frontendService = frontend;

            if (!IsConfigured())
            {
                return;
            }

            Events.Service.AfterStart.AddEventListener((Context context, object sender) =>
            {
                // This route is suggested rather than dependency injection
                // Because some projects (particularly fully headless and micro instances) don't have a page service installed.
                var pageService = Services.Get<Pages.PageService>();
                if (pageService != null)
                {
                    pageService.Install(
                        new Pages.Page()
                        {
                            Url = "/en-admin/search",
                            Title = "Admin Search",
                            ExcludeFromSearch = true,
                            BodyJson = @"{
	                            ""c"": {
		                            ""t"": ""UI/ContentSearch/SearchContext"",
		                            ""d"": {
			                            ""comment"": ""SearchContext used to link search engine, results and filters""
		                            },
		                            ""c"": {
			                            ""t"": ""UI/SiteContainer"",
			                            ""d"": {},
			                            ""c"": {
				                            ""t"": ""UI/SiteRow"",
				                            ""d"": {},
				                            ""c"": [
					                            {
						                            ""t"": ""UI/SiteColumn"",
						                            ""d"": {
							                            ""width"": ""sidebar""
						                            },
						                            ""c"": {
							                            ""t"": ""UI/EntitySearch"",
							                            ""d"": {
								                            ""contentTypes"": """",
								                            ""aggregationFields"": ""contentType,author.keyword"",
								                            ""hideResults"": true,
								                            ""hideTitle"": true,
								                            ""showCounts"": false,
								                            ""allFields"": false,
								                            ""sortOptions"": [
									                            {
										                            ""name"": ""Best Match"",
										                            ""field"": ""_score"",
										                            ""order"": ""desc""
									                            },
									                            {
										                            ""name"": ""Name (A-Z)"",
										                            ""field"": ""primaryObject.name.keyword"",
										                            ""order"": ""asc""
									                            },
									                            {
										                            ""name"": ""Name (Z-A)"",
										                            ""field"": ""primaryObject.name.keyword"",
										                            ""order"": ""desc""
									                            },
									                            {
										                            ""name"": ""Date (Oldest-Latest)"",
										                            ""field"": ""editedUtc"",
										                            ""order"": ""asc""
									                            },
									                            {
										                            ""name"": ""Date (Latest-Oldest)"",
										                            ""field"": ""editedUtc"",
										                            ""order"": ""desc""
									                            }
								                            ],
								                            ""aggregationOverrides"": [
									                            {
										                            ""name"": ""contentType"",
										                            ""label"": ""Entity Type"",
										                            ""all"": ""Any type"",
										                            ""prompt"": """",
										                            ""inputType"": ""select"",
										                            ""allowAllOptions"": true,
										                            ""buckets"": []
									                            },
									                            {
										                            ""name"": ""author.keyword"",
										                            ""label"": ""Author"",
										                            ""all"": ""Any type"",
										                            ""prompt"": """",
										                            ""inputType"": ""select"",
										                            ""allowAllOptions"": true,
										                            ""buckets"": []
									                            }
								                            ],
								                            ""title"": """",
								                            ""baseQuery"": """"
							                            },
							                            ""i"": 4
						                            },
						                            ""i"": 6
					                            },
					                            {
						                            ""t"": ""UI/SiteColumn"",
						                            ""d"": {
							                            ""width"": ""content""
						                            },
						                            ""c"": {
							                            ""t"": ""UI/EntitySearch/EntitySearchResults"",
							                            ""d"": {
								                            ""title"": """",
								                            ""mobileFilterLabel"": ""Filter entities""
							                            },
							                            ""i"": 14
						                            },
						                            ""i"": 11
					                            }
				                            ],
				                            ""i"": 12
			                            },
			                            ""i"": 13
		                            },
		                            ""i"": 16
	                            },
	                            ""i"": 3
                            }"
                        }
                    );
                }

                if (Services.HasHostType("index"))
                {
                    // The cron expression runs it every day at 2am.
                    Events.Automation(fullIndexTaskKey, "0 0 2 ? * * *", false, "Index all content for use in admin search").AddEventListener(async (Context ctx, AutomationRunInfo runInfo) =>
                    {
                        await Index(ctx, "full");
                        return runInfo;
                    });

                    // The cron expression runs it every 5 minutes.
                    Events.Automation(incrementalIndexTaskKey, "0 0/5 * ? * * *", false, "Index amended content for use in admin search").AddEventListener(async (Context ctx, AutomationRunInfo runInfo) =>
                    {
                        if (_cfg.UseDynamicIndexing)
                        {
                            await Index(ctx, "incremental");
                        }
                        return runInfo;
                    });
                }

                // subscribe to event confirming that the elastic store has been reset 
                Events.Elastic.AfterReset.AddEventListener(async (Context ctx, bool reset, string indexName) =>
                {
                    var connected = false;
                    connected = await Events.Elastic.IsConnected.Dispatch(ctx, connected);

                    if (connected && !_cfg.Disabled && (string.IsNullOrWhiteSpace(indexName) || indexName == _cfg.IndexName))
                    {
                        await Events.Elastic.CreateIndex.Dispatch(ctx, connected, _cfg.IndexName);

                        await Events.GetCronScheduler().Trigger(fullIndexTaskKey);
                    }

                    return reset;
                });

                // once connected to elastic we need to create a custom index
                Events.Elastic.AfterConnected.AddEventListener(async (Context ctx, bool connected, SearchElasticServiceConfig config) =>
                {
                    return await Events.Elastic.CreateIndex.Dispatch(ctx, connected, _cfg.IndexName);
                });

                // check if elastic is available and if so will trigger onconnected to create indexes etc 
                var connected = false;
                connected = Events.Elastic.IsConnected.Dispatch(context, connected).Result;

                return new ValueTask<object>(sender);
            });

            // subscribe to events triggered by content types so we can locate entity data to index
            Events.Service.AfterCreate.AddEventListener((Context ctx, AutoService service) =>
            {
                if (service == null)
                {
                    return new ValueTask<AutoService>(service);
                }

                // don't process updates to the control table
                if (this.EntityName == service.EntityName)
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

                //List<System.Attribute> TypeAttributes = ContentField.BuildAttributes(servicedType);
                var isNotSearchable = servicedType.CustomAttributes != null && servicedType.CustomAttributes.Any(a => a.AttributeType == typeof(IsNotSearchableAttribute));
                if (isNotSearchable)
                {
                    var deleted = false;
                    deleted = Events.Elastic.DeleteContentType.Dispatch(ctx, deleted, service.EntityName, new List<string>() { _cfg.IndexName }).Result;

                    return new ValueTask<AutoService>(service);
                }

                // setup the generic method on this class which will basically accept any AutoService.
                var indexEntityMethod = GetType().GetMethod(nameof(IndexEntityMethod));

                // Create a specific flavour of that method:
                var setupIndexEntityMethod = indexEntityMethod.MakeGenericMethod(new Type[] {
                    service.ServicedType,
                    service.IdType
                });

                // And invoke it:
                setupIndexEntityMethod.Invoke(this, new object[] { service });

                return new ValueTask<AutoService>(service);
            });
        }

        /// <summary>
        /// Index all content data 
        /// </summary>
        /// <returns></returns>
        public async Task Index(Context ctx, string mode = null)
        {
            var connected = false;
            connected = Events.Elastic.IsConnected.Dispatch(ctx, connected).Result;
            if (!connected)
            {
                return;
            }

            if (mode == null)
            {
                mode = "full";
            }

            Log.Info(LogTag, $"Elastic Search - Content Indexing ({mode}) - Starting");

            uint processed = 0;

            foreach (var kvp in Services.All)
            {
                // don't process updates to the control table
                if (this.EntityName == kvp.Value.EntityName || kvp.Value.IsTypeProxy)
                {
                    continue;
                }

                // Get the content type for this service and event group:
                var servicedType = kvp.Value.ServicedType;
                if (servicedType == null)
                {
                    // Things like the ffmpeg service.
                    continue;
                }

                // If it's a mapping type, ignore
                if (ContentTypes.IsAssignableToGenericType(servicedType, typeof(Mapping<,>)))
                {
                    continue;
                }

                //List<System.Attribute> TypeAttributes = ContentField.BuildAttributes(servicedType);
                var isNotSearchable = servicedType.CustomAttributes != null && servicedType.CustomAttributes.Any(a => a.AttributeType == typeof(IsNotSearchableAttribute));
                if (isNotSearchable)
                {
                    continue;
                }

                // setup the generic method on this class which will basically accept any AutoService.
                var getItemstoIndexMethod = GetType().GetMethod(nameof(GetItemstoIndex));

                // Create a specific flavour of that method:
                var setupItemstoIndexMethod = getItemstoIndexMethod.MakeGenericMethod(new Type[] {
                        kvp.Value.ServicedType,
                        kvp.Value.IdType
                    });

                // Invoke it asynchronously and await the result:
                var task = (Task<uint>)setupItemstoIndexMethod.Invoke(this, new object[] { kvp.Value, mode });
                processed += await task;
            }
            Log.Info(LogTag, $"Elastic Search - Content Indexing ({mode}) - Completed - [{processed}]");
        }

        /// <summary>
        /// Generic method to check the 'service' fields and if ok adds the create handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ID"></typeparam>
        /// <param name="service"></param>
        public void IndexEntityMethod<T, ID>(AutoService<T, ID> service)
                 where T : Content<ID>, new()
                 where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
        {
            if (_cfg.DebugToConsole)
            {
                Log.Info(LogTag, $"Attaching content indexing to '{service.EntityName}'");
            }

            // wire up the after create event for this entity
            service.EventGroup.AfterCreate.AddEventListener(async (Context context, T entity) =>
            {
                if (!_cfg.UseDynamicIndexing)
                {
                    var exists = await Where("ContentType=? AND ContentId=? AND Action=?", DataOptions.IgnorePermissions)
                    .Bind(service.EntityName)
                    .Bind(service.ReverseId(entity.Id))
                    .Bind("Create")
                    .Any(context);

                    if (!exists)
                    {
                        await Create(context, new SearchEntity()
                        {
                            ContentType = service.EntityName,
                            ContentId = service.ReverseId(entity.Id),
                            Action = "Create"
                        },
                        DataOptions.IgnorePermissions);
                    }
                }
                else
                {
                    // use a clean entity as it may get sanistised by the indexer
                    var newEntity = await service.Get(context, entity.Id, DataOptions.IgnorePermissions);
                    await AddDocument(context, newEntity);
                }

                return entity;
            });

            // wire up the after create event for this entity
            service.EventGroup.AfterUpdate.AddEventListener(async (Context context, T entity) =>
            {
                if (_cfg.UseDynamicIndexing)
                {
                    var exists = await Where("ContentType=? AND ContentId=? AND Action=?", DataOptions.IgnorePermissions)
                    .Bind(service.EntityName)
                    .Bind(service.ReverseId(entity.Id))
                    .Bind("Update")
                    .Any(context);

                    if (!exists)
                    {
                        await Create(context, new SearchEntity()
                        {
                            ContentType = service.EntityName,
                            ContentId = service.ReverseId(entity.Id),
                            Action = "Update"
                        },
                        DataOptions.IgnorePermissions);
                    }
                }
                else
                {
                    // use a clean entity as it may get sanistised by the indexer
                    var newEntity = await service.Get(context, entity.Id, DataOptions.IgnorePermissions);
                    await AddDocument(context, newEntity);
                }

                return entity;
            });

            // wire up the after create event for this entity
            service.EventGroup.BeforeDelete.AddEventListener(async (Context context, T entity) =>
            {
                if (_cfg.UseDynamicIndexing)
                {
                    await Create(context, new SearchEntity()
                    {
                        ContentType = service.EntityName,
                        ContentId = service.ReverseId(entity.Id),
                        Action = "Delete"
                    },
                    DataOptions.IgnorePermissions);
                }
                else
                {
                    var success = false;
                    success = await Events.Elastic.DeleteDocument.Dispatch(context, success, service.EntityName.ToLower() + "-" + entity.Id.ToString(), new List<string>() { _cfg.IndexName });
                }
                return entity;
            });

            async Task<bool> AddDocument(Context context, T entity)
            {
                var connected = false;
                connected = await Events.Elastic.IsConnected.Dispatch(context, connected);
                if (!connected)
                {
                    return false;
                }

                var _canvasRendererService = Services.Get<CanvasRendererService>();
                var _userService = Services.Get<UserService>();

                // see if we have any event handlers to extract custom metadata such as includes 
                Dictionary<string, List<object>> metaData = null;
                if (service.EventGroup.BeforeIndexingMetaData.HasListeners())
                {
                    metaData = await service.EventGroup.BeforeIndexingMetaData.Dispatch(context, metaData, null, null, entity);
                }
                else if (_cfg.DebugToConsole)
                {
                    Log.Warn(LogTag, $"Elastic Search - Content has no metadata handler [{service.EntityName}]");
                }

                // get the title and description/content
                var title = (string)await service.GetMetaFieldValue(context, "title", entity);
                var content = (string)await service.GetMetaFieldValue(context, "description", entity);
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = (string)await service.GetFieldValue(context, "bodyJson", entity);
                }

                if (!string.IsNullOrWhiteSpace(content) && IsValidJson(content) != null)
                {
                    // set state for use when rendering canvas objects back into plan text
                    var state = "{\"po\": " + JsonConvert.SerializeObject(entity, jsonSettings) + "}";

                    try
                    {
                        var renderedCanvas = await _canvasRendererService.Render(context, content, state, RenderMode.Html);
                        if (!renderedCanvas.Failed && !string.IsNullOrWhiteSpace(renderedCanvas.Body))
                        {
                            content = renderedCanvas.Body;
                        }
                        else
                        {
                            content = null;
                        }
                    }
                    catch (Exception)
                    {
                        content = null;
                    }
                }

                // get the content author
                string author = "System";
                uint? authorId = null;
                authorId = (uint?)await service.GetFieldValue(context, "authorId", entity);
                if (authorId.GetValueOrDefault(0) == 0 && typeof(UserCreatedContent<ID>).IsAssignableFrom(typeof(T)))
                {
                    authorId = (entity as UserCreatedContent<ID>).GetCreatorUserId();
                }

                if (authorId.GetValueOrDefault(0) != 0)
                {
                    var authorUser = await _userService.Get(context, authorId.Value, DataOptions.IgnorePermissions);
                    if (authorUser != null)
                    {
                        if (!string.IsNullOrWhiteSpace(authorUser.FirstName))
                        {
                            author = authorUser.FirstName + " " + authorUser.LastName;
                        }
                        else
                        {
                            author = authorUser.Username;
                        }
                    }
                }

                // get the latest update 
                DateTime? editedUtc = null;
                if (typeof(UserCreatedContent<ID>).IsAssignableFrom(typeof(T)))
                {
                    editedUtc = (entity as UserCreatedContent<ID>).GetEditedUtc();
                }

                // get the content icon/image
                string image = null;
                image = (string)await service.GetMetaFieldValue(context, "image", entity);
                if (string.IsNullOrWhiteSpace(image))
                {
                    image = (string)await service.GetMetaFieldValue(context, "icon", entity);
                }
                if (string.IsNullOrWhiteSpace(image) && service.ServicedType.CustomAttributes != null)
                {
                    // Get metadata attributes:
                    var metaAttribs = service.ServicedType.GetCustomAttributes(typeof(MetaAttribute), true);
                    if (metaAttribs != null && metaAttribs.Length > 0)
                    {
                        image = ((MetaAttribute)metaAttribs[0]).Value;
                    }
                }

                Document document = new Document()
                {
                    Title = title,
                    Content = content,
                    Image = image,
                    Author = author,
                    Url = _frontendService.GetPublicUrl(context.LocaleId) + "/en-admin/" + service.EntityName + "/" + entity.Id.ToString(),
                    ContentType = service.EntityName,
                    Id = service.EntityName.ToLower() + "-" + entity.Id.ToString(),
                    PrimaryObject = entity,
                    EditedUtc = editedUtc,
                    MetaDataText = author
                };

                // if we have any metadata/includes then add them in
                if (metaData != null && metaData.Any())
                {
                    if (metaData.ContainsKey("metadataText"))
                    {
                        document.MetaDataText += " " + string.Join(" ", metaData["metadataText"]);
                        metaData.Remove("metadataText");
                    }
                    if (metaData.Any())
                    {
                        document.MetaData = metaData;
                    }
                }

                var success = false;
                success = await Events.Elastic.IndexDocument.Dispatch(context, success, document, new List<string>() { _cfg.IndexName });
                return success;
            }
        }

        /// <summary>
        /// Find and process items to index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ID"></typeparam>
        /// <param name="service"></param>
        /// <param name="mode"></param>
        public async Task<uint> GetItemstoIndex<T, ID>(AutoService<T, ID> service, string mode)
            where T : Content<ID>, new()
            where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
        {
            // Get the field info for the current service type 
            var context = new Context();

            var connected = false;
            connected = await Events.Elastic.IsConnected.Dispatch(context, connected);
            if (!connected)
            {
                return 0;
            }

            uint processed = 0;

            List<T> deleteList = null;
            List<T> entityList = null;

            List<uint> idsToIndex = null;
            List<uint> idsToDelete = null;
            List<uint> idsProcessed = null;

            if (mode == "full")
            {
                entityList = await service.Where(DataOptions.IgnorePermissions).ListAll(context);
            }
            else
            {
                var items = await this.Where("ContentType=?", DataOptions.IgnorePermissions).Bind(service.EntityName).ListAll(context);
                if (items != null && items.Any())
                {
                    idsToIndex = items.Where(i => i.Action != "Delete").Select(x => (uint)x.ContentId).ToList();
                    idsToDelete = items.Where(i => i.Action == "Delete").Select(x => (uint)x.ContentId).ToList();
                    idsProcessed = items.Select(x => x.Id).ToList();

                    if (idsToIndex.Any())
                    {
                        entityList = await service.Where("Id=[?]", DataOptions.IgnorePermissions).Bind(idsToIndex).ListAll(context);
                    }

                    if (idsToDelete.Any())
                    {
                        deleteList = await service.Where("Id=[?]", DataOptions.IgnorePermissions).Bind(idsToDelete).ListAll(context);
                    }
                }
            }

            if (entityList == null && deleteList == null)
            {
                return 0;
            }

            if (deleteList != null && deleteList.Any())
            {
                List<Document> documents = new List<Document>();
                var success = false;

                foreach (var entity in deleteList)
                {
                    success = await Events.Elastic.DeleteDocument.Dispatch(context, success, service.EntityName.ToLower() + "-" + entity.Id.ToString(), new List<string>() { _cfg.IndexName });
                }
            }

            if (entityList != null && entityList.Any())
            {
                var _canvasRendererService = Services.Get<CanvasRendererService>();
                var _userService = Services.Get<UserService>();

                List<Document> documents = new List<Document>();
                var success = false;

                foreach (var entity in entityList)
                {
                    // see if we have any event handlers to extract custom metadata such as includes 
                    Dictionary<string, List<object>> metaData = null;
                    if (service.EventGroup.BeforeIndexingMetaData.HasListeners())
                    {
                        metaData = await service.EventGroup.BeforeIndexingMetaData.Dispatch(context, metaData, null, null, entity);
                    }
                    else if (!documents.Any() && _cfg.DebugToConsole)
                    {
                        Log.Warn(LogTag, $"Elastic Search - Content has no metadata handler [{service.EntityName}]");
                    }

                    // get the title and description/content
                    var title = (string)await service.GetMetaFieldValue(context, "title", entity);
                    var content = (string)await service.GetMetaFieldValue(context, "description", entity);
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        content = (string)await service.GetFieldValue(context, "bodyJson", entity);
                    }

                    if (!string.IsNullOrWhiteSpace(content) && IsValidJson(content) != null)
                    {
                        // set state for use when rendering canvas objects back into plan text
                        var state = "{\"po\": " + JsonConvert.SerializeObject(entity, jsonSettings) + "}";

                        try
                        {
                            var renderedCanvas = await _canvasRendererService.Render(context, content, state, RenderMode.Html);
                            if (!renderedCanvas.Failed && !string.IsNullOrWhiteSpace(renderedCanvas.Body))
                            {
                                content = renderedCanvas.Body;
                            }
                            else
                            {
                                content = null;
                            }
                        }
                        catch (Exception)
                        {
                            content = null;
                        }
                    }

                    // get the content author
                    string author = "System";
                    uint? authorId = null;
                    authorId = (uint?)await service.GetFieldValue(context, "authorId", entity);
                    if (authorId.GetValueOrDefault(0) == 0 && typeof(UserCreatedContent<ID>).IsAssignableFrom(typeof(T)))
                    {
                        authorId = (entity as UserCreatedContent<ID>).GetCreatorUserId();
                    }

                    if (authorId.GetValueOrDefault(0) != 0)
                    {
                        var authorUser = await _userService.Get(context, authorId.Value, DataOptions.IgnorePermissions);
                        if (authorUser != null)
                        {
                            if (!string.IsNullOrWhiteSpace(authorUser.FirstName))
                            {
                                author = authorUser.FirstName + " " + authorUser.LastName;
                            }
                            else
                            {
                                author = authorUser.Username;
                            }
                        }
                    }

                    // get the latest update 
                    DateTime? editedUtc = null;
                    if (typeof(UserCreatedContent<ID>).IsAssignableFrom(typeof(T)))
                    {
                        editedUtc = (entity as UserCreatedContent<ID>).GetEditedUtc();
                    }

                    // get the content icon/image
                    string image = null;
                    image = (string)await service.GetMetaFieldValue(context, "image", entity);
                    if (string.IsNullOrWhiteSpace(image))
                    {
                        image = (string)await service.GetMetaFieldValue(context, "icon", entity);
                    }
                    if (string.IsNullOrWhiteSpace(image) && service.ServicedType.CustomAttributes != null)
                    {
                        // Get metadata attributes:
                        var metaAttribs = service.ServicedType.GetCustomAttributes(typeof(MetaAttribute), true);
                        if (metaAttribs != null && metaAttribs.Length > 0)
                        {
                            image = ((MetaAttribute)metaAttribs[0]).Value;
                        }
                    }

                    Document document = new Document()
                    {
                        Title = title,
                        Content = content,
                        Image = image,
                        Author = author,
                        Url = _frontendService.GetPublicUrl(context.LocaleId) + "/en-admin/" + service.EntityName + "/" + entity.Id.ToString(),
                        ContentType = service.EntityName,
                        Id = service.EntityName.ToLower() + "-" + entity.Id.ToString(),
                        PrimaryObject = entity,
                        EditedUtc = editedUtc,
                        MetaDataText = author
                    };

                    // if we have any metadata/includes then add them in
                    if (metaData != null && metaData.Any())
                    {
                        if (metaData.ContainsKey("metadataText"))
                        {
                            document.MetaDataText += " " + string.Join(" ", metaData["metadataText"]);
                            metaData.Remove("metadataText");
                        }
                        if (metaData.Any())
                        {
                            document.MetaData = metaData;
                        }
                    }

                    processed++;
                    documents.Add(document);

                    if (documents.Count == _cfg.BulkIndexLimit)
                    {
                        success = await Events.Elastic.IndexDocuments.Dispatch(context, success, documents, new List<string>() { _cfg.IndexName });
                        documents.Clear();
                    }
                }

                if (documents.Any())
                {
                    success = await Events.Elastic.IndexDocuments.Dispatch(context, success, documents, new List<string>() { _cfg.IndexName });
                    documents.Clear();
                }

                if (idsProcessed != null && idsProcessed.Any())
                {
                    foreach (var id in idsProcessed)
                    {
                        await this.Delete(context, id, DataOptions.IgnorePermissions);
                    }
                }
            }

            if (mode == "full" || processed > 0)
            {
                Log.Info(LogTag, $"Elastic Search - Content Indexed ({mode}) [{service.EntityName}] - {processed}");
            }

            return processed;
        }

        /// <summary>
        /// check if an element is json/canvas
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        private JObject IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return null; }

            var testString = strInput.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            if ((testString.StartsWith("{") && testString.EndsWith("}")) || //For object
                (testString.StartsWith("[") && testString.EndsWith("]"))) //For array
            {
                try
                {
                    return JObject.Parse(testString);
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    if (_cfg.DebugToConsole)
                    {
                        Log.Warn(LogTag, $"Failed to parse potential json [{testString}] [{jex.Message}");
                    }
                    return null;
                }
                catch (Exception ex) //some other exception
                {
                    if (_cfg.DebugToConsole)
                    {
                        Log.Warn(LogTag, $"Failed to parse potential json [{testString}] [{ex.Message}");
                    }
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        private bool IsConfigured()
        {
            _cfg = GetConfig<SearchEntityConfig>();
            return !string.IsNullOrWhiteSpace(_cfg.IndexName) && !_cfg.Disabled;
        }

    }
}

