using Api.Contexts;
using Api.Database;
using Api.Eventing;
using Api.Startup;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Payments;


/// <summary>
/// Binds MongoDB full text Atlas search, when it's available, to product searches.
/// **If you aren't running on mongoDB, delete this file.**
/// </summary>
[EventListener]
public class MongoSearchEventListener
{
	
	/// <summary>
	/// Instanced automatically.
	/// </summary>
	public MongoSearchEventListener()
	{
		bool? atlasIdentityChecked = null;
		IMongoCollection<Product> productCollection = null;
		ProductSearchService searchService = null;

		string productCollectionName = MongoDBService.CollectionName("product");
		
		Events.Product.Search.AddEventListener(async (Context context, ProductSearch search) => {

			var hasAtlas = atlasIdentityChecked;

			if (!hasAtlas.HasValue)
			{
				hasAtlas = false;

				try
				{
					// Atlas has not been checked yet.
					// This can happen repeatedly but it's fine - it's pretty fast and safe anyway.
					var dbService = Services.Get<MongoDBService>();
					var db = dbService.GetConnection();

					// Hello mongoDB! What are you?
					var helloResult = db.RunCommand<BsonDocument>(new BsonDocument("hello", 1));

					if (helloResult.TryGetValue("setName", out var setName))
					{
						if (setName.AsString.Contains("atlas"))
						{
							Log.Info("productsearch", "Atlas search enabled on '" + setName.AsString + "'");
							hasAtlas = true;
						}
						else
						{
							Log.Info("productsearch", "Atlas search disabled due to non-atlas setName: " + setName.AsString);
						}
					}
					else
					{
						Log.Info("productsearch", "MongoDB did not reply to the hello message with a setName so atlas search is disabled.");
					}
				}
				catch (MongoCommandException mce)
				{
					if (mce.CodeName == "CommandNotFound")
					{
						Log.Info("productsearch", "MongoDB did not reply to the hello message with a setName so atlas search is disabled.");
					}
					else
					{
						throw;
					}
				}
				catch (MongoException me)
				{
					Log.Warn("productsearch", me, "MongoDB atlas feature check threw an error. This is harmless but should be fixed.");
				}

				atlasIdentityChecked = hasAtlas;
			}

			if (productCollection == null)
			{
				var dbService = Services.Get<MongoDBService>();
				var db = dbService.GetConnection();
				productCollection = db.GetCollection<Product>(productCollectionName);
			}

			string query = search.Query;
			int skip = search.PageIndex * search.PageSize;

			if (hasAtlas.Value)
			{
				// Atlas Search pipeline

				if (searchService == null)
				{
					searchService = Services.Get<ProductSearchService>();
				}

				var compoundDoc = new BsonDocument();

				if (!string.IsNullOrEmpty(query))
				{
					compoundDoc["should"] = new BsonArray
					{
						new BsonDocument("text", new BsonDocument
						{
							{ "query", query },
							{ "path", "Name.en" },
							{ "score", new BsonDocument
								{
									{ "boost", new BsonDocument
										{
											{ "value", 2 }
										}
									}
								}
							}
						}),
						new BsonDocument("text", new BsonDocument
						{
							{ "query", query },
							{ "path", "DescriptionRaw" }
						})
					};

					compoundDoc["minimumShouldMatch"] = 1;
				}

				List<BsonDocument> filterClauses = new();

				if (search.AppliedFacets != null)
				{
					foreach (var facet in search.AppliedFacets)
					{
						var ids = facet.Ids;

						if (ids == null || ids.Count == 0 || facet.Mapping == null)
						{
							continue;
						}

						var lcName = facet.Mapping.ToLower();

						if (lcName == "productcategories")
						{
							lcName = "childofcategories";
						}

						string mappingPath = "Mappings." + lcName;

						if (search.SearchType == ProductSearchType.Reductive)
						{
							foreach (var id in ids)
							{
								filterClauses.Add(new BsonDocument("equals", new BsonDocument
								{
									{ "path", mappingPath },
									{ "value", (long)id }
								}));
							}
						}
						else
						{
							var termClauses = ids.Select(id =>
								new BsonDocument("equals", new BsonDocument
								{
									{ "path", mappingPath },
									{ "value", (long)id }
								})
							);

							filterClauses.Add(new BsonDocument("compound", new BsonDocument
							{
								{ "should", new BsonArray(termClauses) },
								{ "minimumShouldMatch", 1 }
							}));
						}
					}
				}

				if (search.InStockOnly)
				{
					filterClauses.Add(new BsonDocument("compound", new BsonDocument
					{
						{ "must", new BsonArray
							{
								new BsonDocument("exists", new BsonDocument
								{
									{ "path", "InStock" }
								}),
								new BsonDocument("range", new BsonDocument
								{
									{ "path", "InStock" },
									{ "gt", 0 }
								})
							}
						}
					}));
				}

				if (filterClauses.Count > 0)
				{
					compoundDoc["filter"] = new BsonArray(filterClauses);
				}

				if (compoundDoc.ElementCount == 0)
				{
					// Admin panel with no query and no filters -
					// atlas requires the compound to at least contain something, so we
					// require _id to exist which it always does on MongoDB/ Atlas.
					compoundDoc["must"] = new BsonArray
					{
						new BsonDocument("exists", new BsonDocument("path", "_id"))
					};
				}

				var searchStage = new BsonDocument
				{
					{ "index", searchService.CurrentConfig().AtlasIndex ?? "default" },
					{ "compound", compoundDoc },
					{ "returnStoredSource", true }
				};

				var facetStage = new BsonDocument("$facet", new BsonDocument
					{
						{ "products", new BsonArray
							{
								new BsonDocument("$skip", skip),
								new BsonDocument("$limit", search.PageSize),
								new BsonDocument("$project", new BsonDocument
								{
									{ "_id", 1 }
								}),
								new BsonDocument("$lookup", new BsonDocument
								{
									{ "from", productCollectionName },
									{ "localField", "_id" },
									{ "foreignField", "_id" },
									{ "as", "fullDoc" },
								}),
								new BsonDocument("$unwind", "$fullDoc"),
								new BsonDocument("$replaceRoot", new BsonDocument
								{
									{ "newRoot", "$fullDoc" }
								})
							}
						},
						{ "productcategoriesFacet", new BsonArray
							{
								new BsonDocument("$unwind", "$Mappings.productcategories"),
								new BsonDocument("$sortByCount", "$Mappings.productcategories")
							}
						},
						{ "attributesFacet", new BsonArray
							{
								new BsonDocument("$unwind", "$Mappings.attributes"),
								new BsonDocument("$sortByCount", "$Mappings.attributes")
							}
						},
						{ "totalCount", new BsonArray
							{
								new BsonDocument("$count", "count")
							}
						}
					});

				// Relevance mode (default)
				var pipeline = (string.IsNullOrEmpty(search.SortOrder.Field) || search.SortOrder.Field == "relevance") ? new[] {
					new BsonDocument("$search", searchStage),
					facetStage
				} :
				[
					new BsonDocument("$search", searchStage),
					new BsonDocument(
						"$sort",
						new BsonDocument(search.SortOrder.Field, search.SortOrder.Direction == SortDirection.ASC ? 1 : -1)
					),
					facetStage,
				];

				var cursor = await productCollection.AggregateAsync<BsonDocument>(pipeline);
				var doc = await cursor.FirstOrDefaultAsync();

				var products = doc["products"].AsBsonArray
					.Select(d => BsonSerializer.Deserialize<Product>(d.AsBsonDocument))
					.ToList();

				var categoryFacet = ToCategoryFacets(doc["productcategoriesFacet"].AsBsonArray);
				var attributeFacet = ToAttributeFacets( doc["attributesFacet"].AsBsonArray);

				int totalCount = 0;
				var countArray = doc["totalCount"].AsBsonArray;
				if (countArray.Count > 0)
				{
					totalCount = countArray[0]["count"].AsInt32;
				}

				search.Products = products;
				search.Handled = true;
				search.Total = totalCount;
				search.ResultFacets = new ProductSearchFacets() {
					Categories = categoryFacet,
					Attributes = attributeFacet
				};
			}
			else
			{
				// Non-atlas search - doesn't perform fulltext search or return facets.
				// Basic facets are instead derived from the (paginated) returned product set only
				// (that happens in ProductSearchService).
				var filter = Builders<Product>.Filter.Or(
					Builders<Product>.Filter.Regex("Name.en", new BsonRegularExpression(query, "i")),
					Builders<Product>.Filter.Regex("DescriptionRaw", new BsonRegularExpression(query, "i"))
				);

				if (search.AppliedFacets != null)
				{
					var childFilters = new List<FilterDefinition<Product>>
					{
						filter
					};

					foreach (var facet in search.AppliedFacets)
					{
						var ids = facet.Ids;

						if (ids == null || ids.Count == 0 || facet.Mapping == null)
						{
							continue;
						}

						if (search.SearchType == ProductSearchType.Reductive)
						{
							childFilters.Add(Builders<Product>.Filter.All("Mappings." + facet.Mapping.ToLower(), new BsonArray(ids)));
						}
						else
						{
							childFilters.Add(Builders<Product>.Filter.In("Mappings." + facet.Mapping.ToLower(), new BsonArray(ids)));
						}
					}
					
					if (search.InStockOnly)
					{
						childFilters.Add(
							Builders<Product>.Filter.And(
								Builders<Product>.Filter.Exists("InStock", true),
								Builders<Product>.Filter.Ne("InStock", 0)
							)
						);
					}

					filter = Builders<Product>.Filter.And(childFilters);
					
				}
				
				var sortBuilder = Builders<Product>.Sort;

				SortDefinition<Product> sortDefinition;

				
				var field = search.SortOrder.Field;
				var direction = search.SortOrder.Direction == SortDirection.ASC;

				List<Product> products;

				if (string.IsNullOrEmpty(field) || field == "relevance")
				{
					// Non-atlas search doesn't support relevance so this is just the default lack of sorting.
					products = await productCollection
						.Find(filter)
						.Limit(search.PageSize)
						.ToListAsync();
				}
				else
				{
					sortDefinition = direction
						? sortBuilder.Ascending(field)
						: sortBuilder.Descending(field);

					products = await productCollection
						.Find(filter)
						.Sort(sortDefinition)
						.Limit(search.PageSize)
						.ToListAsync();
				}

				search.Total = products.Count;
				search.Products = products;
				search.Handled = true;
			}

			return search;
		});
		
	}

	private List<AttributeValueFacet> ToAttributeFacets(BsonArray values)
	{
		var vals = new List<AttributeValueFacet>();

		foreach (var bsonVal in values)
		{
			var id = bsonVal["_id"].AsInt64;
			var count = bsonVal["count"].AsInt32;

			vals.Add(new AttributeValueFacet()
			{
				AttributeValueId = (uint)id,
				Count = count
			});
		}

		return vals;
	}

	private List<ProductCategoryFacet> ToCategoryFacets(BsonArray values)
	{
		var vals = new List<ProductCategoryFacet>();

		foreach (var bsonVal in values)
		{
			var id = bsonVal["_id"].AsInt64;
			var count = bsonVal["count"].AsInt32;

			vals.Add(new ProductCategoryFacet()
			{
				ProductCategoryId = (uint)id,
				Count = count
			});
		}

		return vals;
	}


}