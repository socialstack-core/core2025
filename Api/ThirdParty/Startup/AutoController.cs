using Api.Contexts;
using Api.Database;
using Api.Permissions;
using Api.SocketServerLibrary;
using Api.Startup;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// The base of all controllers. Don't use ASP.NET controllers as they will not run!
/// This is because the serverside renderer and the websocket engine mount your 
/// controller methods directly, and thus no actual request happens. This also is used to enforce  
/// field visibility rules always as your controller methods can safely just return content objects.
/// </summary>
public class AutoController
{

	/// <summary>
	/// Outputs a context update.
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="context"></param>
	/// <returns></returns>
	protected async ValueTask OutputContext(HttpContext httpContext, Context context)
	{
		var response = httpContext.Response;

		// Regenerate the contextual token:
		context.SendToken(response);

		response.ContentType = "application/json";
		await Services.Get<ContextService>().ToJson(context, response.Body);
	}

}

/// <summary>
/// A convenience controller for defining common endpoints like create, list, delete etc. Requires an AutoService of the same type to function.
/// Not required to use these - you can also just directly use ControllerBase if you want.
/// Like AutoService this isn't in a namespace due to the frequency it's used.
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class AutoController<T> : AutoController<T, uint>
	where T : Content<uint>, new()
{
}

/// <summary>
/// A convenience controller for defining common endpoints like create, list, delete etc. Requires an AutoService of the same type to function.
/// Not required to use these - you can also just directly use ControllerBase if you want.
/// Like AutoService this isn't in a namespace due to the frequency it's used.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="ID"></typeparam>
[ApiController]
public partial class AutoController<T,ID> : AutoController
	where T : Content<ID>, new()
	where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
{

	/// <summary>
	/// The underlying autoservice used by this controller.
	/// </summary>
	protected AutoService<T, ID> _service;

    /// <summary>
    /// Instanced automatically.
    /// </summary>
    public AutoController()
    {
        // Find the service:
        if (Api.Startup.Services.AutoServices.TryGetValue(typeof(AutoService<T, ID>), out AutoService svc))
		{
			_service = (AutoService<T, ID>)svc;
		}
		else
		{
			throw new Exception(
				"Unable to use AutoController for type " + typeof(T).Name + " as it doesn't have an AutoService. " +
				"You must also declare an :AutoService<" + typeof(T).Name + "> as it'll use that for the underlying functionality."
			);
		}

	}

	/// <summary>
	/// GET /v1/entityTypeName/2/
	/// Returns the data for 1 entity.
	/// </summary>
	[HttpGet("{id}")]
	public virtual async ValueTask<T> Load(Context context, [FromRoute] ID id)
	{
		var result = await _service.Get(context, id);
		return result;
    }

	/// <summary>
	/// DELETE /v1/entityTypeName/2/
	/// Deletes an entity
	/// </summary>
	[HttpDelete("{id}")]
    public virtual async ValueTask<T> Delete(Context context, [FromRoute] ID id)
	{
		var result = await _service.Get(context, id);

		if (result == null || !await _service.Delete(context, result))
		{
			// The handlers have blocked this one from happening, or it failed
			return null;
		}

		return result;
	}

	/// <summary>
	/// GET /v1/entityTypeName/recache
	/// Repopulates the cache for this service (if it is cached, and if you are an admin).
	/// </summary>
	/// <returns></returns>
	[HttpGet("recache")]
	public virtual async ValueTask Recache(Context context)
	{
		if (context.Role == null || !context.Role.CanViewAdmin)
		{
			throw PermissionException.Create("recache", context);
		}

		await _service.Recache();
	}

	/// <summary>
	/// GET /v1/entityTypeName/list
	/// Lists all entities of this type available to this user.
	/// </summary>
	/// <returns></returns>
	[HttpGet("list")]
	public virtual ValueTask<ContentStream<T, ID>?> ListAll(Context context)
	{
		return List(context, null);
	}

	/// <summary>
	/// POST /v1/entityTypeName/list
	/// Lists filtered entities available to this user.
	/// See the filter documentation for more details on what you can request here.
	/// </summary>
	/// <returns></returns>
	[HttpPost("list")]
	public virtual ValueTask<ContentStream<T, ID>?> List(Context context, [FromBody] ListFilter filters)
	{
		var filter = _service.LoadFilter(filters) as Filter<T, ID>;
		
		if (filter == null)
		{
			// A handler rejected this request.
			return new ValueTask<ContentStream<T, ID>?>((ContentStream<T, ID>?)null);
		}

		var streamer = _service.GetResults(filter);
		return new ValueTask<ContentStream<T, ID>?>(streamer);
	}

    /// <summary>
    /// POST /v1/entityTypeName/
    /// Creates a new entity. Returns the ID. Includes everything by default.
    /// </summary>
    [HttpPost]
	public virtual async ValueTask<T> Create(Context context, [FromBody] JObject body)
	{
		// Start building up our object.
		// Most other fields, particularly custom extensions, are handled by autoform.
		var entity = (T)Activator.CreateInstance(_service.InstanceType);

		// If it's user created we'll set the user ID now:
		var userCreated = (entity as Api.Users.UserCreatedContent<ID>);

		if (userCreated != null)
		{
			userCreated.UserId = context.UserId;
		}
		
		// Set the actual fields now:
		await SetFieldsOnObject(entity, context, body, JsonFieldGroup.Default);

		// Not permitted to create with a specified ID via the API. Ensure it's 0:
		entity.SetId(default);

		entity = await _service.CreatePartial(context, entity, DataOptions.Default);
		
		if(entity == null)
		{
			return null;
		}
		
		// Set post ID fields:
		await SetFieldsOnObject(entity, context, body, JsonFieldGroup.AfterId);

		// If it has an on object, create the mapping entry now if we have read visibility of the target:
		var on = body["on"];

		if (on != null && on.Type == JTokenType.Object)
		{
			// Get relevant fields:
			var type = on["type"];
			var id = on["id"];
			var map = on["map"];

			// If map is null, we'll use the primary map. First though, attempt to get the actual content type:
			var contentType = ContentTypes.GetType(type.Value<string>());

			if (contentType != null)
			{
				var svc = Services.GetByContentType(contentType);

				if (svc != null)
				{
					var srcObject = await svc.GetObject(context, "Id", id.Value<string>());

					if (srcObject != null)
					{
						// Mapping permitted.
						string mapName;

						if (map == null)
						{
							// "this" service is the one which has a ListAs:
							mapName = _service.GetContentFields().PrimaryMapName;

							if (string.IsNullOrEmpty(mapName))
							{
								throw new PublicException(
									"This type '" + typeof(T).Name + "' doesn't have a primary map name so you'll need to specify a particular map: in your on:{}.",
									"no_map"
								);
							}
						}
						else
						{
							mapName = map.Value<string>();

							if (!ContentFields.GlobalVirtualFields.ContainsKey(mapName.ToLower()))
							{
								throw new PublicException(
									"A map called '" + mapName + "' doesn't exist.",
									"no_map"
								);
							}
						}

						// Create map from srcObject -> entity via the map called MapName. First though, get the mapping service:
						var mappingService = await MappingTypeEngine.GetOrGenerate(svc, _service, mapName);
						await mappingService.CreateMapping(context, srcObject, entity, DataOptions.IgnorePermissions);
					}
				}
			}
		}

		// Complete the call (runs AfterCreate):
		entity = await _service.CreatePartialComplete(context, entity);

		if (entity == null)
		{
			return null;
		}

		return entity;
	}

	/// <summary>
	/// Sets the fields from the given JSON object on the given target object, based on the user role in the context.
	/// Note that there's 2 sets of fields - a primary set, then also a secondary set which are set only after the ID of the object is known.
	/// E.g. during create, the object is instanced, initial fields are set, it's then actually created, and then the after ID set is run.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="context"></param>
	/// <param name="body"></param>
	/// <param name="fieldGroup"></param>
	protected async ValueTask SetFieldsOnObject(T target, Context context, JObject body, JsonFieldGroup fieldGroup = JsonFieldGroup.Any)
	{
        // Get the JSON meta which will indicate exactly which fields are editable by this user (role):
		var availableFields = await _service.GetTypedJsonStructure(context);

		foreach (var property in body.Properties())
		{
			if (property.Name == "on")
			{
				continue;
			}

			// Attempt to get the available field:
			var field = availableFields.GetField(property.Name, fieldGroup);

			if (field == null)
			{
				continue;
			}

			// Try setting the value now:
			await field.SetFieldValue(context, target, property.Value);
		}
	}
	
	/// <summary>
	/// POST /v1/entityTypeName/1/
	/// Updates an entity with the given ID. Includes everything by default.
	/// </summary>
	[HttpPost("{id}")]
	public virtual async ValueTask<T> Update(Context context, [FromRoute] ID id, [FromBody] JObject body)
	{
		var originalEntity = await _service.Get(context, id);
		
		if (originalEntity == null)
		{
			return null;
		}

		if (originalEntity == null)
		{
			return null;
		}

		var entityToUpdate = await _service.StartUpdate(context, originalEntity);

		if (entityToUpdate == null)
		{
			// Can't start update (no permission, typically - it throws in that scenario).
			return null;
		}

		// In this case the entity ID is definitely known, so we can run all fields at the same time:
		await SetFieldsOnObject(entityToUpdate, context, body, JsonFieldGroup.Any);

		// Make sure it's still the original ID:
		entityToUpdate.SetId(id);

		entityToUpdate = await _service.FinishUpdate(context, entityToUpdate, originalEntity);

		if (entityToUpdate == null)
		{
			return null;
		}

		return entityToUpdate;
	}

}
