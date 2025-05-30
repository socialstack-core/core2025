using Api.Contexts;
using Api.Eventing;
using Api.Permissions;
using Api.Startup;
using System;
using System.Threading.Tasks;

namespace Api.Database;

/// <summary>
/// Instanced automatically. Inits the main caches.
/// </summary>
[EventListener]
public class Init
{
	
	/// <summary>
	/// Instanced automatically. Inits the main caches.
	/// </summary>
	public Init()
	{
		var setupHandlersMethod = GetType().GetMethod(nameof(SetupService));

		Events.Service.AfterCreate.AddEventListener(async (Context context, AutoService service) => {

			if (service == null || service.ServicedType == null)
			{
				return service;
			}

			var servicedType = service.ServicedType;

			if (servicedType != null)
			{
				// Add data load events:
				var setupType = setupHandlersMethod.MakeGenericMethod(new Type[] {
						servicedType,
						service.IdType
					});

				setupType.Invoke(this, new object[] {
					service
				});
			}

			return service;
		}, 1);

		Events.Service.AfterCreate.AddEventListener(async (Context context, AutoService service) => {

			if (service == null || service.ServicedType == null)
			{
				return service;
			}

			// Service can now attempt to load its cache:
			await service.SetupCacheIfNeeded();

			return service;
		}, 3);
	}

	/// <summary>
	/// Sets up for the given type with its event group along with updating any DB tables.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="ID"></typeparam>
	/// <param name="service"></param>
	public void SetupService<T, ID>(AutoService<T, ID> service)
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		// Special case if it is a mapping type.
		var entityName = service.EntityName;
		var isDbStored = service.DataIsPersistent;

		service.EventGroup.Delete.AddEventListener((Context context, T result) =>
		{
			if (result == null)
			{
				return new ValueTask<T>((T)null);
			}

			// Remove from the primary cache:
			var cache = service.GetCacheForLocale(1);

			if (cache != null)
			{
				cache.Remove(context, result.Id);
			}

			return new ValueTask<T>(result);
		}, 100);

		service.EventGroup.Update.AddEventListener(async (Context context, T entity, ChangedFields changes, DataOptions opts) =>
		{
			if (entity == null)
			{
				return entity;
			}

			// Cache update.
			var locale = context == null ? 1 : context.LocaleId;
			var cache = service.GetCacheForLocale(locale);

			if (cache == null)
			{
				return entity;
			}

			// Future improvement: rather than copying all fields and
			// writing all fields, instead focus only on the ones which changed.

			// Copy fields from entity -> orig.
			var orig = cache.Get(entity.Id);

			// Anything that makes an assumption that the object doesn't change can continue with that assumption.
			service.CloneEntityInto(entity, orig);

			var id = orig.Id;

			T raw = null;

			if (locale == 1)
			{
				raw = orig;
			}
			else
			{
				if (cache != null)
				{
					raw = cache.GetRaw(id);
				}

				if (raw == null)
				{
					raw = new T();
				}

				// Must also update the raw object in the cache (as the given entity is _not_ the raw one).
				T primaryEntity;

				if (cache == null)
				{
					primaryEntity = await service.Get(new Context(1, context.User, context.RoleId), id, DataOptions.IgnorePermissions);
				}
				else
				{
					primaryEntity = service.GetCacheForLocale(1).Get(id);
				}

				service.PopulateRawEntityFromTarget(raw, orig, primaryEntity);
			}

			if (cache != null)
			{
				cache.Add(context, orig, raw);

				if (locale == 1)
				{
					service.OnPrimaryEntityChanged(orig);
				}

			}

			return entity;
		}, 100);

		service.EventGroup.Load.AddEventListener((Context context, T item, ID id) =>
		{
			if (item != null)
			{
				return new ValueTask<T>(item);
			}

			// Load from cache if there is one.
			var cache = service.GetCacheForLocale(context == null ? 1 : context.LocaleId);

			if (cache != null)
			{
				item = cache.Get(id);
			}

			return new ValueTask<T>(item);
		}, 5);

		service.EventGroup.CreatePartial.AddEventListener((Context context, T newEntity) =>
		{

			// If this is a cached type, must add it to all locale caches.
			if (service.CacheAvailable)
			{
				// If the newEntity is not in the primary locale, we will need to derive the raw object.
				// Any localised fields should be set to their default value (null/ 0).
				// [May2023] The above causes issues where an entity is created on a locale other than 1 and is then used by an include
				// which iterates over the cache for locale #1 to collect IDs. DB engine gets it correct but cache does not.

				var raw = context.LocaleId == 1 ? newEntity : new T();

				if (context.LocaleId != 1)
				{
					service.CloneEntityInto(newEntity, raw);
				}

				var localeSet = ContentTypes.Locales;

				for (var i = 0; i < localeSet.Length; i++)
				{
					var locale = localeSet[i];

					if (locale == null)
					{
						continue;
					}

					var cache = service.GetCacheForLocale(locale.Id);

					if (cache == null)
					{
						continue;
					}

					if (i == 0)
					{
						// Primary locale cache - raw and target are the same object.
						cache.Add(context, raw, raw);
					}
					else if (locale.Id == context.LocaleId)
					{
						// Add the given object as-is.
						cache.Add(context, newEntity, raw);
					}
					else
					{
						// Secondary locale. The target object is just a clone of the raw object.
						var entity = (T)Activator.CreateInstance(service.InstanceType);
						service.PopulateTargetEntityFromRaw(entity, raw, raw);

						var localeRaw = (T)Activator.CreateInstance(service.InstanceType);
						service.PopulateTargetEntityFromRaw(localeRaw, raw, raw);

						cache.Add(context, entity, localeRaw);
					}
				}
			}

			return new ValueTask<T>(newEntity);
		});

		service.EventGroup.List.AddEventListener(async (Context context, QueryPair<T, ID> queryPair) =>
		{

			if (queryPair.Handled)
			{
				return queryPair;
			}

			// Do we have a cache?
			var cache = (queryPair.QueryA.DataOptions & DataOptions.CacheFlag) == DataOptions.CacheFlag ? service.GetCacheForLocale(context.LocaleId) : null;

			if (cache != null)
			{
				queryPair.Handled = true;

				// Great - we're using the cache:
				queryPair.Total = await cache.GetResults(context, queryPair, queryPair.OnResult, queryPair.SrcA, queryPair.SrcB);
			}

			return queryPair;
		}, 5);

	}

}