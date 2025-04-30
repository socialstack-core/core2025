using Api.CanvasRenderer;
using Api.Database;
using Api.Eventing;
using Api.SearchCrawler;
using Api.SocketServerLibrary;
using Api.Startup;
using Api.Tags;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Context = Api.Contexts.Context;

namespace Api.SearchIndexing
{

    /// <summary>
    /// Add additional meta data to the document indexing
    /// </summary>

    [LoadPriority(9)]
    [HostType("index")]

    public partial class SearchIndexingService : AutoService
    {
        private CanvasRendererService _canvasRendererService;
        private SearchIndexingServiceConfig _cfg;

        /// <summary>
        /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
        /// </summary>
        public SearchIndexingService(CanvasRendererService canvasRenderer)
        {
            if (!IsConfigured())
            {
                return;
            }

            _canvasRendererService = canvasRenderer;

            Events.Service.AfterStart.AddEventListener((Context context, object sender) =>
            {
                // subscribe to event after the page html has been loaded to allow for parsing before its 
                // added to the index (remove footers etc)
                Events.Elastic.AfterPage.AddEventListener((Context ctx, HtmlDocument htmlDocument) =>
                {
                    if (_cfg == null || _cfg.NodeSelectors == null)
                    {
                        return new ValueTask<HtmlDocument>(htmlDocument);
                    }

                    foreach (var selector in _cfg.NodeSelectors)
                    {
                        if (string.IsNullOrWhiteSpace(selector))
                        {
                            continue;
                        }

                        var nodes = htmlDocument.DocumentNode.SelectNodes(selector);
                        if (nodes != null)
                        {
                            foreach (var node in nodes)
                            {
                                node.Remove();
                            }
                        }
                    }

					return new ValueTask<HtmlDocument>(htmlDocument);
				});

                // subscribe to event just before elastic document is stored
                // last chance to make any changes 
                Events.Elastic.BeforeDocumentUpdate.AddEventListener((Context ctx, Api.SearchElastic.Document document) =>
                {
                    return new ValueTask<Api.SearchElastic.Document>(document);
                });

                return new ValueTask<object>(sender);
            });

            // subscribe to events triggered by content types so we can add index listeners 
            Events.Service.AfterCreate.AddEventListener((Context ctx, AutoService service) =>
            {
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

                var setupForTypeMethod = GetType().GetMethod(nameof(SetupForType));

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

        private bool IsConfigured()
        {
            _cfg = GetConfig<SearchIndexingServiceConfig>();
            return (_cfg.Mappings != null && _cfg.Mappings.Count > 0) || (_cfg.NodeSelectors != null && _cfg.NodeSelectors.Count > 0);
        }

        /// <summary>
        /// Handler for content types to allow for custom meta data to be extracted
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
                Log.Info(LogTag, $"Attaching content metadata indexer to '{service.EntityName}'");
            }

            // Content types that are configured for additional metaddata indexing processed here.    
            service.EventGroup.BeforeIndexingMetaData.AddEventListener(async (Context ctx, Dictionary<string, List<object>> metaData, CrawledPageMeta pageMeta, IEnumerable<Tag> tags, T po) =>
            {
                if (_cfg == null || _cfg.Mappings == null || po == null)
                {
                    return metaData;
                }

                //if (service.EntityName == "Configuration" && po.Id.ToString() == "11")
                //{
                //    System.Diagnostics.Debugger.Break();
                //}

                HashSet<string> metadataText = new HashSet<string>();

                // extract out any includes for the primaryObject
                var mapping = _cfg.Mappings.FirstOrDefault(i => i.Type == po.Type);

                if (mapping != null && !string.IsNullOrWhiteSpace(mapping.Includes))
                {
                    // get all the includes (as per front end calls we get json back initially)
                    var jsonString = await service.ToJson(ctx, po, mapping.Includes);
                    var searchMetaData = JsonConvert.DeserializeObject<SearchMetaData>("{\"primaryObject\" : " + jsonString + "}");

                    // set state for use when rendering canvas objects back into plan text
                    var state = "{\"po\": " + JsonConvert.SerializeObject(po, jsonSettings) + "}";

                    dynamic includedService = null;
                    string includeType = "";

                    // group together the includes so they can be accessed in a structured way 
                    // metadata.tag.name 
                    if (metaData == null)
                    {
                        metaData = new Dictionary<string, List<object>>();
                    }

                    // also inject the primary object into the metadata 
                    if (_cfg.IncludePrimary)
                    {
                        metadataText.UnionWith(await ExtractAllStringValues(ctx, state, po));

                        if (!metaData.ContainsKey(po.Type.ToLower()))
                        {
                            metaData.Add(po.Type.ToLower(), new List<object>());
                        }
                        metaData[po.Type.ToLower()].Add(po);
                    };

                    // map the includes back to real objects
                    foreach (var include in searchMetaData.Includes.SelectMany(s => s.Values))
                    {
                        string typeName = include["type"]?.ToString();
                        string id = include["id"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(typeName) && !string.IsNullOrWhiteSpace(id))
                        {
                            if (typeName.ToLower() == "user")
                            {
                                continue;
                            }

                            if (typeName != includeType)
                            {
                                // get the relevant service
                                includeType = typeName;
                                includedService = Services.Get($"{includeType}Service");
                            }

                            // map into real object
                            var dataItem = await includedService.Where("Id=?", DataOptions.IgnorePermissions).Bind(Convert.ChangeType(id, TypeCode.UInt32)).First(ctx);
                            if (dataItem != null)
                            {
                                metadataText.UnionWith(await ExtractAllStringValues(ctx, state, dataItem));

                                // add into buckets to allow for structured queries
                                if (!metaData.ContainsKey(includeType.ToLower()))
                                {
                                    metaData.Add(includeType.ToLower(), new List<object>());
                                }
                                metaData[includeType.ToLower()].Add(dataItem);
                            }
                        }
                    }
                }
                else if (_cfg.IncludePrimary)
                {
                    // set state for use when rendering canvas objects back into plan text
                    var state = "{\"po\": " + JsonConvert.SerializeObject(po, jsonSettings) + "}";
                    metadataText.UnionWith(await ExtractAllStringValues(ctx, state, po));
                }

                // pass back any extracted text/string values
                if (metadataText.Any())
                {
                    if (metaData == null)
                    {
                        metaData = new Dictionary<string, List<object>>();
                    }

                    if (!metaData.ContainsKey("metadataText"))
                    {
                        metaData.Add("metadataText", new List<object>());
                    }
                    metaData["metadataText"].AddRange(metadataText);
                }


                return metaData;
            }, 5);
        }

        /// <summary>
        /// Extract the plain text from a canvas object
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="state"></param>
        /// <param name="canvasText"></param>
        /// <returns></returns>
        private async ValueTask<string> ExtractCanvasText(Context ctx, string state, string canvasText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(canvasText))
                {
                    return canvasText;
                }

                var jsonObject = IsValidJson(canvasText);

                if (jsonObject == null)
                {
                    return canvasText;
                }

                // extract any canvas components 
                HashSet<string> stringValues = new HashSet<string>();
                ExtractJsonComponents(jsonObject, stringValues);

                try
                {
                    var renderedCanvas = await _canvasRendererService.Render(ctx, canvasText, state, RenderMode.Text);

                    if (!renderedCanvas.Failed && !string.IsNullOrWhiteSpace(renderedCanvas.Text))
                    {
                        return renderedCanvas.Text + " " + string.Join(" ", stringValues);
                    }
                }
                catch (Exception e)
                {
                    // failed to render canvas 
                    var test = e.Message;
                }
                
                if (stringValues.Any())
                {
                    return string.Join(" ", stringValues);
                }

                return string.Empty;

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        static void ExtractJsonComponents(JToken token, HashSet<string> stringValues)
        {
            if (token.Type == JTokenType.String)
            {
                if (token.ToString().StartsWith("Admin/") || token.ToString().StartsWith("Email/") || token.ToString().StartsWith("UI/"))
                {
                    stringValues.Add(token.ToString());
                }
            }
            else if (token.Type == JTokenType.Object || token.Type == JTokenType.Array || token.Type == JTokenType.Property)
            {
                foreach (var child in token.Children())
                {
                    ExtractJsonComponents(child, stringValues);
                }
            }
        }


        /// <summary>
        /// Locate any string properties in an object
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetStringProperties<TEntity>(TEntity entity)
        {
            return entity.GetType().GetProperties().Where(p => p.PropertyType == typeof(String) || p.PropertyType == typeof(string));
        }

        /// <summary>
        /// Locate any string field elements in an object
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerable<FieldInfo> GetStringFields<TEntity>(TEntity entity)
        {
            return entity.GetType().GetFields().Where(p => p.FieldType == typeof(String) || p.FieldType == typeof(string));
        }

        /// <summary>
        /// Extract any strings from an object
        /// used to extract indexable content from primaryObjects and metadata (includes)
        /// also parses and updates the values in the passed in object 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="state"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private async ValueTask<IEnumerable<string>> ExtractAllStringValues<TEntity>(Context ctx, string state, TEntity instance)
        {
            var stringValues = new List<string>();

            foreach (var field in GetStringFields(instance))
            {
                var value = (string)field.GetValue(instance);
                if (string.IsNullOrWhiteSpace(value))
                {
                    // strip out out any empty fields
                    field.SetValue(instance, null);
                    continue;
                }

                // ignore any and links and images
                if (value.StartsWith("public:") ||
                    value.StartsWith("private:") ||
                    value.StartsWith("http://") ||
                    value.StartsWith("https://"))
                {
                    continue;
                }

                var parsedText = await ExtractCanvasText(ctx, state, value);
                if (!string.IsNullOrWhiteSpace(parsedText))
                {
                    stringValues.Add(parsedText);
                }
            }

            return stringValues;
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

    }
}

