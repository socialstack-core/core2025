using System;
using Api.Startup;
using Api.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System.Threading.Tasks;
using Api.Permissions;
using Api.Contexts;
using System.Reflection.Metadata;
using MongoDB.Bson;


namespace Api.Database;

/// <summary>
/// MongoDB database service.
/// Connects to a database with the given connection string.
/// </summary>
[LoadPriority(1)]
public partial class MongoDBService : AutoService
{

	/// <summary>
	/// The table name to use for a particular type.
	/// This is generally used on types which are DatabaseRow instances.
	/// </summary>
	public static string CollectionName(string entityName)
	{
		// Just prefixed (e.g. site_product by default):
		var name = AppSettings.DatabaseTablePrefix + entityName.ToLower();

		if (name.Length > 120)
		{
			name = name.Substring(0, 120);
		}

		return name;
	}

	/// <summary>
	/// Returns a connection string, or null, if it isn't configured.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public static ConnectionString GetConfiguredConnectionString()
	{
		return Api.Database.ConnectionString.Get("Mongo");
	}

	/// <summary>
	/// The connection string to use.
	/// </summary>
	public string ConnectionString { get; set; }

	/// <summary>
	/// The latest DB schema.
	/// </summary>
	public Schema Schema { get; set; }

	/// <summary>
	/// Create a new database connector with the given connection string.
	/// </summary>
	public MongoDBService() {
		// Load from appsettings and add a change handler.
		LoadFromAppSettings();

		AppSettings.OnChange += () => {
			LoadFromAppSettings();
		};
	}

	/// <summary>
	/// Indicates the connection string should be loaded or reloaded.
	/// </summary>
	private void LoadFromAppSettings()
	{
		var cs = GetConfiguredConnectionString();
		ConnectionString = cs == null ? null : cs.ConnectionConfig;

		_client = null;
		_database = null;
	}

	private MongoClient _client;
	private IMongoDatabase _database;

	/// <summary>
	/// Gets a shared mongoDB client for connections to a particular database, specified by the ConnectionString.
	/// </summary>
	/// <returns></returns>
	internal IMongoDatabase GetConnection()
	{
		var db = _database;

		if (db == null)
		{
			var settings = MongoClientSettings.FromConnectionString(ConnectionString);
			settings.ConnectTimeout = TimeSpan.FromSeconds(3);
			settings.SocketTimeout = TimeSpan.FromSeconds(3);
			settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
			settings.MaxConnectionPoolSize = 100;
			var client = new MongoClient(settings);
			var mongoUrl = new MongoUrl(ConnectionString);
			db = client.GetDatabase(mongoUrl.DatabaseName);
			_client = client;
			_database = db;
		}

		return db;
	}

	/// <summary>
	/// Gets a list of results from the cache, calling the given callback each time one is discovered.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="queryPair">Both filterA and filterB must have values.</param>
	/// <param name="collection"></param>
	public async ValueTask<int> GetResults<T, ID, INSTANCE_TYPE>(
		Context context, QueryPair<T, ID> queryPair, IMongoCollection<INSTANCE_TYPE> collection
	)
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		where INSTANCE_TYPE : T
	{
		string localeCode = null;
		if (context != null && context.LocaleId > 1)
		{
			var localeId = context.LocaleId;
			var locale = (ContentTypes.Locales != null && localeId <= ContentTypes.Locales.Length ? ContentTypes.Locales[localeId - 1] : null);
			localeCode = locale?.Code;
		}

		if (localeCode == null)
		{
			localeCode = "en";
		}

		var srcA = queryPair.SrcA;
		var srcB = queryPair.SrcB;
		var onResult = queryPair.OnResult;
		var includeTotal = queryPair.QueryA == null ? false : queryPair.QueryA.IncludeTotal;

		int total = 0;

		// FilterA is the user filter. It provides things like the collector and some contextual args.
		var filterA = queryPair.QueryA;

		// FilterB is the permission system filter. Like filterA it can be null.
		var filterB = queryPair.QueryB;

		FilterDefinition<INSTANCE_TYPE> filterDefA = filterA == null ? null : filterA.ToMongo<INSTANCE_TYPE>(
			localeCode,
			context,
			filterA
		);

		FilterDefinition<INSTANCE_TYPE> filterDefB = filterB == null ? null : filterB.ToMongo<INSTANCE_TYPE>(
			localeCode,
			context,
			filterA // Contextual functionality originate from the user filter, not the permission one here. 
		);

		FilterDefinition<INSTANCE_TYPE> filter;

		if (filterDefA == null)
		{
			if (filterDefB == null)
			{
				filter = Builders<INSTANCE_TYPE>.Filter.Empty;
			}
			else
			{
				filter = filterDefB;
			}
		}
		else if (filterDefB == null)
		{
			filter = filterDefA;
		}
		else
		{
			filter = Builders<INSTANCE_TYPE>.Filter.And(filterDefA, filterDefB);
		}

		if (includeTotal)
		{
			var count = await collection.CountDocumentsAsync(filter);
			total = (int)count;
		}

		var index = 0;

		var findOptions = new FindOptions<INSTANCE_TYPE>
		{
			Limit = (filterA.PageSize > 0 ? filterA.PageSize : null),
			Skip = (filterA.PageSize > 0 ? filterA.PageSize * filterA.Offset : 0),
		};

		if (filterA.SortField != null)
		{
			findOptions.Sort = filterA.SortAscending
			? Builders<INSTANCE_TYPE>.Sort.Ascending(filterA.SortField.Name)
			: Builders<INSTANCE_TYPE>.Sort.Descending(filterA.SortField.Name);
		}

		var cursor = await collection.FindAsync(filter, findOptions);

		while (await cursor.MoveNextAsync())
		{
			var result = cursor.Current;

			foreach (var item in result)
			{
				await onResult(context, item, index, srcA, srcB);
				index++;
			}
		}

		return total;
	}

}

/// <summary>
/// A counter in mongoDB.
/// </summary>
public class MongoDBCounter
{
	/// <summary>
	/// The collection ID.
	/// </summary>
	public string _id { get; set; }

	/// <summary>
	/// The current latest value.
	/// </summary>
	public long AutoInc { get; set; }
}