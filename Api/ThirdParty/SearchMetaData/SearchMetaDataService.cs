using Api.CanvasRenderer;
using Api.Database;
using Api.Eventing;
using Api.Permissions;
using Api.Startup;
using Api.Translate;
using Mysqlx.Crud;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Context = Api.Contexts.Context;

namespace Api.SearchMetaData
{

    /// <summary>
    /// Add additional meta data to the document indexing
    /// </summary>

    public partial class SearchMetaDataService : AutoService
    {
        private SearchMetaDataConfig _cfg;

        /// <summary>
        /// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
        /// </summary>
        public SearchMetaDataService(ContentFieldAccessRuleService fieldAccessRuleService)
        {
            _cfg = GetConfig<SearchMetaDataConfig>();

            // subscribe to events triggered by content types so we can add index listeners 
            Events.Service.AfterCreate.AddEventListener(async (Context ctx, AutoService service) =>
            {
                if (service == null)
                {
                    return service;
                }

                // Get the content type for this service and event group:
                var servicedType = service.ServicedType;
                if (servicedType == null)
                {
                    // Things like the ffmpeg service.
                    return service;
                }

                if (_cfg == null || _cfg.Mappings == null)
                {
                    return service;
                }

                // ensure there is a mapping for the service
                var mapping = _cfg.Mappings.FirstOrDefault(i => i.Type == service.EntityName);
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.Includes))
                {
                    return service;
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
                return service;
            });

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
                Log.Info(LogTag, $"Registering Search Metadata for {service.EntityName}");
            }

            // Content types that are configured for additional metaddata indexing processed here.    
            service.EventGroup.SearchMetaData.AddEventListener(async (Context ctx, HashSet<string> metadataText, T po) =>
            {
                if (_cfg == null || _cfg.Mappings == null || po == null)
                {
                    return metadataText;
                }

                if (metadataText == null)
                {
                    metadataText = new HashSet<string>();
                }

                // extract out any includes for the primaryObject
                var mapping = _cfg.Mappings.FirstOrDefault(i => i.Type == po.Type);

                if (mapping != null && !string.IsNullOrWhiteSpace(mapping.Includes))
                {
                    // get all the includes (as per front end calls we get json back initially)
                    var jsonString = await service.ToJson(ctx, po, mapping.Includes);
                    var searchMetaData = JsonConvert.DeserializeObject<SearchMetaData>("{\"primaryObject\" : " + jsonString + "}");

                    dynamic includedService = null;
                    string includeType = "";

                    // also inject the primary object into the metadata 
                    if (_cfg.IncludePrimary)
                    {
                        metadataText.UnionWith(ExtractAllStringValues(ctx, po));
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

                            // map into real object so that use field definitions etc 
                            var dataItem = await includedService.Where("Id=?", DataOptions.IgnorePermissions).Bind(Convert.ChangeType(id, TypeCode.UInt32)).First(ctx);
                            if (dataItem != null)
                            {
                                metadataText.UnionWith(ExtractAllStringValues(ctx, dataItem));
                            }
                        }
                    }
                }
                else if (_cfg.IncludePrimary)
                {
                    metadataText.UnionWith(ExtractAllStringValues(ctx, po));
                }

                return metadataText;
            }, 5);


            service.EventGroup.BeforeCreate.AddEventListener(async (ctx, entity) =>
            {
                return await HandleSearchMetaData(ctx, entity, service);
            }, 20);

            service.EventGroup.BeforeUpdate.AddEventListener(async (Context ctx, T updated, T orig) =>
            {
                return await HandleSearchMetaData(ctx, updated, service);
            }, 20);
        }

        private async Task<T> HandleSearchMetaData<T,ID>(Context ctx, T entity, AutoService<T, ID> service)
            where T : Content<ID>, new()
            where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
        {
            if (!service.EventGroup.SearchMetaData.HasListeners())
            {
                return entity;
            }

            var fieldInfo = await service.GetJsonStructure(ctx);
            var metadataField = fieldInfo.AllFields
                .FirstOrDefault(s => s.Key.Equals("descriptionraw", StringComparison.OrdinalIgnoreCase))
                .Value;

            if (metadataField != null)
            {
                var metaData = new HashSet<string>();
                metaData = await service.EventGroup.SearchMetaData.Dispatch(ctx, metaData, entity);
                metadataField.FieldInfo.SetValue(entity, string.Join(" ", metaData));
            }

            return entity;
        }


        /// <summary>
        /// Locate any string field elements in an object
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private IEnumerable<FieldInfo> GetStringFields<TEntity>(TEntity entity)
        {
            return entity.GetType().GetFields().Where(p =>
            p.FieldType == typeof(String) ||
            p.FieldType == typeof(string) ||
            p.FieldType == typeof(Localized<string>) ||
            p.FieldType == typeof(Localized<JsonString>));
        }

        /// <summary>
        /// Extract any strings from an object
        /// used to extract indexable content from primaryObjects and metadata (includes)
        /// also parses and updates the values in the passed in object 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private IEnumerable<string> ExtractAllStringValues<TEntity>(Context ctx, TEntity instance)
        {
            var stringValues = new HashSet<string>();

            foreach (var field in GetStringFields(instance))
            {
                // ignore any coinfigured fields such as slug etc
                if (_cfg.IgnoredFields.Contains(field.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                string value = null;

                if (field.FieldType == typeof(Localized<JsonString>))
                {
                    value = ((Localized<JsonString>)field.GetValue(instance)).Get(ctx).ValueOf();
                }
                else if (field.FieldType == typeof(Localized<string>))
                {
                    value = ((Localized<string>)field.GetValue(instance)).Get(ctx);
                }
                else
                {
                    value = (string)field.GetValue(instance);
                }

                // ignore empty strings and any and links and images
                if (string.IsNullOrWhiteSpace(value) ||
                    value.StartsWith("public:") ||
                    value.StartsWith("private:") ||
                    value.StartsWith("http://") ||
                    value.StartsWith("https://"))
                {
                    continue;
                }

                if (field.FieldType == typeof(Localized<JsonString>))
                {
                    var plainTextItems = GetPlainText(value);

                    if (plainTextItems != null)
                    {
                        stringValues.UnionWith(plainTextItems);
                    }
                }
                else
                {
                    if (_cfg.DebugToConsole)
                    {
                        Log.Info("Search Metadata", $"Plain text value - {instance.GetType().Name} {field.Name} {value}");
                    }

                    stringValues.Add(value);
                }
            }

            return stringValues;
        }

        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        private static HashSet<string> GetPlainText(string content)
        {
            try
            {
                JToken root = JToken.Parse(content);
                return ExtractTextValues(root);
            }
            catch (Exception ex)
            {
                Log.Error("Search Indexing", $"Failed to extract plain text [{content}] [{ex.ToString}]");
            }

            return null;
        }

        /// <summary>
        /// Extract any string/text entries from a canvas element
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static HashSet<string> ExtractTextValues(JToken token)
        {
            var result = new HashSet<string>();

            void Traverse(JToken current)
            {
                if (current.Type == JTokenType.Object)
                {
                    foreach (var prop in current.Children<JProperty>())
                    {
                        if (prop.Name == "s" && prop.Value.Type == JTokenType.String)
                        {
                            result.Add(prop.Value.ToString());
                        }
                        else
                        {
                            Traverse(prop.Value);
                        }
                    }
                }
                else if (current.Type == JTokenType.Array)
                {
                    foreach (var item in current.Children())
                    {
                        Traverse(item);
                    }
                }
            }

            Traverse(token);
            return result;
        }
    }

}

