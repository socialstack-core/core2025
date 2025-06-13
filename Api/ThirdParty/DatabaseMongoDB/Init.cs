﻿using System;
using Api.Startup;
using Api.Eventing;
using Api.Contexts;
using System.Threading.Tasks;
using Api.Translate;
using System.Collections.Generic;
using Api.Permissions;
using Api.CanvasRenderer;
using Api.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using Api.Configuration;
using System.Linq;
using MongoDB.Bson.Serialization;
using System.Reflection;
using MongoDB.Bson.Serialization.Conventions;

namespace Api.DatabaseMongoDB
{

	/// <summary>
	/// Instances capabilities during the very earliest phases of startup.
	/// </summary>
	[EventListener]
	public class Init
	{
		/// <summary>
		/// True if the DB version has been checked.
		/// </summary>
		private bool? VersionCheckResult;

		/// <summary>
		/// Database version text.
		/// </summary>
		private string VersionText;

		private MongoDBService _database;

		/// <summary>
		/// Instanced automatically.
		/// </summary>
		public Init()
		{
			if (MongoDBService.GetConfiguredConnectionString() == null)
			{
				Log.Info("mongodb", "MongoDB is installed but has not started because it has no configured connection strings. " +
					"(typically MongoConnectionStrings.DefaultConnection in appsettings.json).");
				return;
			}

			var setupHandlersMethod = GetType().GetMethod(nameof(SetupService));

			// Add handler for the initial locale list:
			Events.Locale.InitialList.AddEventListener(async (Context context, List<Locale> locales) => {

				if (_database == null)
				{
					_database = Services.Get<MongoDBService>();
				}

				RegisterClassMap<Locale>();

				try
				{
					var con = _database.GetConnection();
					var collection = con.GetCollection<Locale>(MongoDBService.CollectionName(nameof(Locale)));
					locales = await collection.Find(null).ToListAsync();
				}
				catch
				{
					// The table doesn't exist. Locale set is just the default one:
					locales = new List<Locale>();
				}

				if (locales.Count == 0)
				{
					locales.Add(new Locale()
					{
						Code = "en",
						Name = new Localized<string>("English"),
						Id = 1
					});
				}

				return locales;
			});

			Events.Service.AfterCreate.AddEventListener(async (Context context, AutoService service) => {

				if (service == null || service.ServicedType == null)
				{
					return service;
				}

				if (_database == null)
				{
					_database = Services.Get<MongoDBService>();
				}
				
				var servicedType = service.ServicedType;

				if (servicedType != null)
				{
					// Add data load events:
					var setupType = setupHandlersMethod.MakeGenericMethod(new Type[] {
						servicedType,
						service.IdType,
						service.InstanceType
					});

					var task = (Task)setupType.Invoke(this, new object[] {
						service,
						service.GetEventGroup()
					});

					await task;
				}

				return service;
			}, 2);
		}

		/// <summary>
		/// Sets up for the given type with its event group along with updating any DB tables.
		/// The event group is passed separately such that the database migration mechanism can use this same method too.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <param name="service"></param>
		/// <param name="eventGroup"></param>
		public async Task SetupService<T, ID, INSTANCE_TYPE>(AutoService<T, ID> service, EventGroup<T,ID> eventGroup)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
			where INSTANCE_TYPE : T
		{
			// Start preparing the queries. Doing this ahead of time leads to excellent performance savings, 
			// whilst also using a high-level abstraction as another plugin entry point.

			// Special case if it is a mapping type.
			var entityName = service.EntityName;
			var isDbStored = service.DataIsPersistent;

			if (!isDbStored)
			{
				return;
			}

			// We have a thing we'll potentially need to reconfigure.
			if (_database == null)
			{
				Log.Warn("databasediff", "The type '" + service.ServicedType.Name + "' did not have its database schema mounted because the database service was not up in time.");
				return;
			}

			await TryCheckVersion();

			RegisterClassMap<INSTANCE_TYPE>();

			// Create the collection and Id index if it does not exist.
			var collectionName = MongoDBService.CollectionName(entityName);
			var link = _database.GetConnection();
			var collection = link.GetCollection<INSTANCE_TYPE>(collectionName);
			var existingIndexes = await collection.Indexes.ListAsync();
			var indexesList = await existingIndexes.ToListAsync();

			var counters = link.GetCollection<MongoDBCounter>("counters");
			var counterFilter = Builders<MongoDBCounter>.Filter.Eq(c => c._id, collectionName);
			var counterUpdateBuilder = Builders<MongoDBCounter>.Update;
			var counterUpdate = counterUpdateBuilder.Inc(c => c.AutoInc, 1);

			try
			{
				await counters.InsertOneAsync(new MongoDBCounter() {
					_id = collectionName,
					AutoInc = 0
				});
			}
			catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
			{
			}

			var options = new FindOneAndUpdateOptions<MongoDBCounter>
			{
				IsUpsert = true,
				ReturnDocument = ReturnDocument.After
			};

			bool indexExists = indexesList.Any(i => i["name"] == "Id_1");

			if (!indexExists)
			{
				var indexOptions = new CreateIndexOptions
				{
					Unique = true
				};
				var indexKeys = Builders<INSTANCE_TYPE>.IndexKeys.Ascending("Id");
				var indexModel = new CreateIndexModel<INSTANCE_TYPE>(indexKeys, indexOptions);

				try
				{
					await collection.Indexes.CreateOneAsync(indexModel);
				}
				catch (MongoCommandException ex)
				{
					if (!ex.Message.Contains("already exists"))
						throw; // rethrow other exceptions
				}
			}

			var filters = Builders<INSTANCE_TYPE>.Filter;

			eventGroup.AfterInstanceTypeUpdate.AddEventListener(async (Context context, AutoService s) => {

				if (s == null)
				{
					return s;
				}

				if (isDbStored)
				{
					Log.Warn(
						"mongodb", 
						"The mongoDB module does not yet support switching type descriptions at runtime. It is very possible though - if you need it, please ask!"
					);

					// INSTANCE_TYPE is used by the mongo Collection object, which in turn is used by all the event handlers.
					// You would need to unregister them all and register new ones ideally.
					// RegisterClassMap<NEW_INSTANCE_TYPE>(); is essential though.

					await TryCheckVersion();
				}

				return s;
			});

			eventGroup.Delete.AddEventListener(async (Context context, T result) =>
			{
				// Delete the entry:
				var filter = filters.Eq("Id", result.Id);
				var deleteResult = await collection.DeleteOneAsync(filter);

				if (deleteResult.DeletedCount == 0)
				{
					return null;
				}

				// Successful delete from the db.
				return result;
			});

			eventGroup.Create.AddEventListener(async (Context context, T entity) =>
			{
				if (entity.Id.Equals(default))
				{
					// Explicit ID has been provided otherwise.
					// Assign an ID:
					var counter = await counters.FindOneAndUpdateAsync(counterFilter, counterUpdate, options);
					var newId = counter.AutoInc;

					// Map this ID via the svc:
					entity.Id = service.ConvertId((ulong)newId);
				}
				
				// Create it:
				await collection.InsertOneAsync((INSTANCE_TYPE)entity);
				
				return entity;
			});

			eventGroup.CreateAll.AddEventListener(async (Context context, List<T> entities) =>
			{
				// Ensure each one has an ID.
				var idsToCollect = 0;

				foreach (var entity in entities)
				{
					if (entity.Id.Equals(default))
					{
						idsToCollect++;
					}
				}

				if (idsToCollect > 0)
				{
					var updateBlock = counterUpdateBuilder.Inc(c => c.AutoInc, idsToCollect);
					var counter = await counters.FindOneAndUpdateAsync(counterFilter, updateBlock, options);
					var startId = (ulong)(counter.AutoInc - (idsToCollect - 1));

					foreach (var entity in entities)
					{
						if (entity.Id.Equals(default))
						{
							entity.Id = service.ConvertId(startId);
							startId++;
						}
					}

				}

				await collection.InsertManyAsync(entities as IEnumerable<INSTANCE_TYPE>);

				return entities;
			});

			eventGroup.Update.AddEventListener(async (Context context, T entity, ChangedFields changes, DataOptions opts) => {

				if (entity == null || changes.None)
				{
					return entity;
				}

				DateTime? prevEdit = changes.PreviousEditedUtc;

				// For each field change, if it is a localised field and locale is not 1 then update the relevant localised field.
				// Otherwise, update the base field.

				// Get the locale code:
				string localeCode = null;
				var localeId = context.LocaleId;

				if (localeId > 1)
				{
					var locale = (ContentTypes.Locales != null && localeId <= ContentTypes.Locales.Length ? 
							ContentTypes.Locales[localeId - 1] : null);
					localeCode = locale.Code;
				}

				UpdateDefinition<INSTANCE_TYPE> update = null;

				foreach (var field in changes)
				{
					var val = field.TargetField.GetValue(entity);
					var name = field.Name;

					if (update == null)
					{
						update = Builders<INSTANCE_TYPE>.Update.Set(name, val);
					}
					else
					{
						update = update.Set(name, val);
					}
				}

				// Add one of the two static where clauses:
				FilterDefinition<INSTANCE_TYPE> filter;

				if (prevEdit.HasValue)
				{
					filter = filters.And(
						filters.Eq("Id", entity.Id),
						filters.Eq("EditedUtc", prevEdit.Value)
					);
				}
				else
				{
					filter = filters
						.Eq("Id", entity.Id);
				}

				var result = await collection.UpdateOneAsync(filter, update);

				if (result.ModifiedCount == 1)
				{
					return entity;
				}

				// It failed.
				return null;
			});

			eventGroup.Load.AddEventListener(async (Context context, T item, ID id) => {

				if (item != null)
				{
					return item;
				}

				var filter = filters.Eq("Id", id);
				var findResult = await collection.Find(filter).FirstOrDefaultAsync();
				return findResult;
			});

			eventGroup.List.AddEventListener(async (Context context, QueryPair<T, ID> queryPair) => {

				if (queryPair.Handled)
				{
					return queryPair;
				}

				queryPair.Handled = true;

				// Get the results from the database:
				queryPair.Total = await _database.GetResults(
					context, 
					queryPair,
					collection
				);

				return queryPair;
			});

		}

		/// <summary>
		/// Registers the class map for a given entity type.
		/// </summary>
		private void RegisterClassMap<T>()
		{
			var type = typeof(T);

			if (type.BaseType != null && type.BaseType != typeof(object))
			{
				var setupType = GetType()
					.GetMethod(
						nameof(RegisterClassMap), 
						System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
					).MakeGenericMethod(new Type[] {
						type.BaseType
					});

				setupType.Invoke(this, Array.Empty<object>());
			}

			if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
			{
				BsonClassMap.RegisterClassMap<T>(cm =>
				{
					cm.SetDiscriminatorIsRequired(false);

					var conventionPack = new ConventionPack
					{
						// No camelCase convention here = preserve C# member names
						new IgnoreExtraElementsConvention(true),
						new IgnoreIfNullConvention(false)
					};

					ConventionRegistry.Register("Conventions", conventionPack, t => true);

					var fields = typeof(T).GetFields(
						BindingFlags.Public | 
						BindingFlags.NonPublic | 
						BindingFlags.Instance | 
						BindingFlags.DeclaredOnly
					);

					foreach (var field in fields)
					{
						cm
						.MapField(field.Name)
						.SetElementName(field.Name);
					}
				});
			}
		}

		/// <summary>
		/// Checks the DB version to see if we can auto handle schemas.
		/// </summary>
		/// <returns></returns>
		private async Task<bool> TryCheckVersion()
		{
			if (VersionCheckResult.HasValue)
			{
				return VersionCheckResult.Value;
			}
			string dbVersion = null;
			var tryAgain = true;

			// This is the first db query that happens - if the database is not yet available we'll keep retrying until it is.
			while (tryAgain)
			{
				tryAgain = false;

				try
				{
					var command = new BsonDocument("buildInfo", 1);
					var result = await _database.GetConnection().RunCommandAsync<BsonDocument>(command);

					// Get MongoDB version:
					dbVersion = result["version"].AsString;
				}
				catch (Exception e)
				{
					Log.Warn("mongodb", e, "Authentication or unable to contact MongoDB. Trying again in 5 seconds.");
					await Task.Delay(5000);
					tryAgain = true;
				}
			}

			// Get DB version:
			VersionText = dbVersion;
			var version = VersionText.ToLower().Trim();

			// rc, beta etc.
			var versionPieces = version.Split('-');

			if (!Version.TryParse(versionPieces[0], out Version parsedVersion))
			{
				Log.Warn("mongodb", "Database module disabled due to unrecognised MongoDB version text. It was: " + version);
				VersionCheckResult = false;
				return false;
			}

			Version minVersion = new Version(4, 4);

			// Which version we got?
			if (minVersion == null)
			{
				Log.Warn("mongodb", "Database module disabled. Unrecognised MongoDB variant: " + version);
				VersionCheckResult = false;
				return false;
			}
			else if (parsedVersion < minVersion)
			{
				Log.Warn("mongodb", "Database module disabled. You're using a version of MongoDB that is too old. Its version: " + version);
				VersionCheckResult = false;
				return false;
			}

			BsonSerializer.RegisterSerializer(new JsonStringSerializer());
			BsonSerializer.RegisterSerializer(new MappingDataSerializer());

			BsonSerializer.RegisterGenericSerializerDefinition(
				typeof(Localized<>),
				typeof(LocalizedSerializer<>)
			);

			// Initialise the locale list:
			await Locale.InitialiseLocaleList(new Context());

			VersionCheckResult = true;
			return true;
		}
	}
}
