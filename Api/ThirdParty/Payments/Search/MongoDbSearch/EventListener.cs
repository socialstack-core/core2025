using Api.Contexts;
using Api.Database;
using Api.Eventing;
using Api.Startup;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Mysqlx.Crud;
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
				catch(MongoException me)
				{
					Log.Warn("productsearch", me, "MongoDB atlas feature check threw an error. This is harmless but should be fixed.");
				}

				atlasIdentityChecked = hasAtlas;
			}

			if (productCollection == null)
			{
				var dbService = Services.Get<MongoDBService>();
				var db = dbService.GetConnection();
				productCollection = db.GetCollection<Product>(MongoDBService.CollectionName("product"));
			}

			string query = search.Query;
			int skip = search.PageIndex * search.PageSize;

			if (hasAtlas.Value)
			{
				// Atlas Search pipeline

				var compoundDoc = new BsonDocument
				{
					{ "should", new BsonArray
						{
							new BsonDocument("text", new BsonDocument
							{
								{ "query", query },
								{ "path", "Name.en" }
							})
						}
					}
				};

				if (search.AppliedFacets != null)
				{
					List<BsonDocument> filters = null;

					foreach (var filter in search.AppliedFacets)
					{
						var ids = filter.Ids;

						if (ids == null || ids.Count == 0 || filter.Mapping == null)
						{
							continue;
						}

						if (filters == null)
						{
							filters = new List<BsonDocument>();
						}

						filters.Add(new BsonDocument("terms", new BsonDocument
						{
							{ "path", "Mappings." + filter.Mapping.ToLower() },
							{ "value", new BsonArray(ids) }
						}));
					}

					if (filters != null)
					{
						compoundDoc["filter"] = new BsonArray(filters);
					}
				}

				var searchStage = new BsonDocument
				{
					{ "index", "default" },
					{ "compound", compoundDoc }
				};

				var pipeline = new[]
				{
					new BsonDocument("$search", searchStage),
					new BsonDocument("$facet", new BsonDocument
					{
						{ "products", new BsonArray
							{
								new BsonDocument("$skip", skip),
								new BsonDocument("$limit", search.PageSize),
								new BsonDocument("$project", new BsonDocument
								{
									{ "_id", 1 },
									{ "Name", 1 },
									{ "Mappings", 1 }
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
					})
				};

				var cursor = await productCollection.AggregateAsync<BsonDocument>(pipeline);
				var doc = await cursor.FirstOrDefaultAsync();

				var products = doc["products"].AsBsonArray
					.Select(d => BsonSerializer.Deserialize<Product>(d.AsBsonDocument))
					.ToList();

				var categoryFacet = ToFacetSet("productcategories", doc["productcategoriesFacet"].AsBsonArray);
				var attributeFacet = ToFacetSet("attributes", doc["attributesFacet"].AsBsonArray);

				int totalCount = 0;
				var countArray = doc["totalCount"].AsBsonArray;
				if (countArray.Count > 0)
				{
					totalCount = countArray[0]["count"].AsInt32;
				}

				search.Products = products;
				search.Handled = true;
				search.Total = totalCount;
				search.ResultFacets = new List<ProductSearchFacet>() {
					categoryFacet,
					attributeFacet
				};
			}
			else
			{
				// Non-atlas search - doesn't perform fulltext search or return facets.
				// Basic facets are instead derived from the (paginated) returned product set only
				// (that happens in ProductSearchService).

				var filter = Builders<Product>.Filter.Regex("name.en", new BsonRegularExpression(query, "i"));
				
				/*
				 * (description is not plaintext, it is canvas JSON)
				 * 
				 * Builders<Product>.Filter.Or(
					Builders<Product>.Filter.Regex("name.en", new BsonRegularExpression(query, "i")),
					Builders<Product>.Filter.Regex("description.en", new BsonRegularExpression(query, "i"))
				);
				*/

				var products = await productCollection
					.Find(filter)
					.Limit(search.PageSize)
					.ToListAsync();

				search.Total = products.Count;
				search.Products = products;
				search.Handled = true;
			}

			return search;
		});
		
	}

	private ProductSearchFacet ToFacetSet(string mappingName, BsonArray values)
	{
		var vals = new List<ProductSearchFacetResult>();

		foreach (var bsonVal in values)
		{
			var id = bsonVal["_id"].AsInt64;
			var count = bsonVal["count"].AsInt32;

			vals.Add(new ProductSearchFacetResult()
			{
				EntityId = (ulong)id,
				Count = count
			});
		}

		return new ProductSearchFacet()
		{
			Mapping = mappingName,
			Values = vals
		};
	}


}