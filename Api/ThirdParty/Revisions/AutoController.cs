using System;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Permissions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Api.Startup;
using Api.Users;

/// <summary>
/// A convenience controller for defining common endpoints like create, list, delete etc. Requires an AutoService of the same type to function.
/// Not required to use these - you can also just directly use ControllerBase if you want.
/// Like AutoService this isn't in a namespace due to the frequency it's used.
/// </summary>
public partial class AutoController<T, ID>
{
	/// <summary>
	/// GET /v1/entityTypeName/revision/2/
	/// Returns the data for 1 entity revision.
	/// </summary>
	[HttpGet("revision/{id}")]
	public virtual async ValueTask<T> LoadRevision(Context context, [FromRoute] ID id)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		var result = await revisions.Get(context, id);
		return result;
	}

	/// <summary>
	/// DELETE /v1/entityTypeName/revision/2/
	/// Deletes an entity
	/// </summary>
	[HttpDelete("revision/{id}")]
	public virtual async ValueTask<T> DeleteRevision(Context context, [FromRoute] ID id)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		var result = await revisions.Get(context, id);
		
		if (result == null || !await revisions.Delete(context, result))
		{
			// The handlers have blocked this one from happening, or it failed
			return null;
		}

		return result;
	}

	/// <summary>
	/// GET /v1/entityTypeName/revision/list
	/// Lists all entity revisions of this type available to this user.
	/// </summary>
	/// <returns></returns>
	[HttpGet("revision/list")]
	public virtual ValueTask<ContentStream<T, ID>?> RevisionList(Context context)
	{
		return RevisionList(context, null);
	}

	/// <summary>
	/// POST /v1/entityTypeName/revision/list
	/// Lists filtered entity revisions available to this user.
	/// See the filter documentation for more details on what you can request here.
	/// </summary>
	/// <returns></returns>
	[HttpPost("revision/list")]
	public virtual ValueTask<ContentStream<T, ID>?> RevisionList(Context context, [FromBody] ListFilter filters)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return new ValueTask<ContentStream<T, ID>?>((ContentStream<T, ID>?)null);
		}

		var filter = revisions.LoadFilter(filters) as Filter<T, ID>;
		
		if (filter == null)
		{
			// A handler rejected this request.
			return new ValueTask<ContentStream<T, ID>?>((ContentStream<T, ID>?)null);
		}

		return new ValueTask<ContentStream<T, ID>?>(revisions.GetResults(filter));
	}
	
	/// <summary>
	/// POST /v1/entityTypeName/revision/1
	/// Updates an entity revision with the given RevisionId.
	/// </summary>
	[HttpPost("revision/{id}")]
	[Receives(typeof(PartialContent))]
	public virtual async ValueTask<T> UpdateRevision(Context context, [FromRoute] ID id, [FromBody] JObject body)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		var entity = await UpdateInternal(revisions, context, id, body);
		return entity;
	}
	
	/// <summary>
	/// GET /v1/entityTypeName/publish/1
	/// Publishes the given revision as the new live entry.
	/// </summary>
	[HttpGet("publish/{id}")]
	public virtual async ValueTask<T> PublishRevision(Context context, [FromRoute] ID id)
	{
		return await PublishRevision(context, id, null);
	}
	
	/// <summary>
	/// POST /v1/entityTypeName/publish/1
	/// Publishes the given posted object as an extension to the given revision (if body is not null).
	/// </summary>
	[HttpPost("publish/{id}")]
	[Receives(typeof(PartialContent))]
	public virtual async ValueTask<T> PublishRevision(Context context, [FromRoute] ID id, [FromBody] JObject body)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		var entity = await UpdateInternal(revisions, context, id, body);
		entity = await revisions.PublishRevision(context, entity);
		return entity;
	}
	
	/// <summary>
	/// POST /v1/entityTypeName/draft/
	/// Creates a draft.
	/// </summary>
	[HttpPost("draft")]
	[Receives(typeof(PartialContent))]
	public virtual async ValueTask<T> CreateDraft(Context context, [FromBody] JObject body)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		return await CreateInternal(revisions, context, body, (Context ctx, T entity) => {
			var vc = entity as VersionedContent<ID>;

			if (vc != null)
			{
				vc.IsDraft = true;
			}
		});
	}
}
