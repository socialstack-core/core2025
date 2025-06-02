using System;
using System.Threading.Tasks;
using Api.Contexts;
using Api.Permissions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Api.Startup;
using Api.Users;
using Api.Revisions;
using System.Text;

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
	[OmitIfNoService]
	public virtual async ValueTask<Revision<T,ID>> LoadRevision(Context context, [FromRoute] ID id)
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
	[OmitIfNoService]
	public virtual async ValueTask<Revision<T,ID>> DeleteRevision(Context context, [FromRoute] ID id)
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
	[OmitIfNoService]
	public virtual ValueTask<ContentStream<Revision<T, ID>, ID>?> RevisionList(Context context)
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
	[OmitIfNoService]
	public virtual ValueTask<ContentStream<Revision<T, ID>, ID>?> RevisionList(Context context, [FromBody] ListFilter filters)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return new ValueTask<ContentStream<Revision<T, ID>, ID>?>((ContentStream<Revision<T, ID>, ID>?)null);
		}

		var filter = revisions.LoadFilter(filters) as Filter<Revision<T, ID>, ID>;
		
		if (filter == null)
		{
			// A handler rejected this request.
			return new ValueTask<ContentStream<Revision<T, ID>, ID>?>((ContentStream<Revision<T, ID>, ID>?)null);
		}

		return new ValueTask<ContentStream<Revision<T, ID>, ID>?>(revisions.GetResults(filter));
	}
	
	/// <summary>
	/// GET /v1/entityTypeName/publish/1
	/// Publishes the given revision as the new live entry.
	/// </summary>
	[HttpGet("publish/{id}")]
	[OmitIfNoService]
	public virtual async ValueTask<T> PublishRevision(Context context, [FromRoute] ID id)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		var entity = await revisions.Get(context, id);
		return await revisions.PublishRevision(context, entity);
	}
	
	/// <summary>
	/// POST /v1/entityTypeName/draft/
	/// Creates a draft.
	/// </summary>
	[HttpPost("draft")]
	[OmitIfNoService]
	public virtual async ValueTask<Revision<T,ID>> CreateDraft(Context context, [FromBody] JObject body)
	{
		var revisions = _service.Revisions;

		if (revisions == null)
		{
			return null;
		}

		var str = new StringBuilder();
		str.Append('{');
		var first = true;

		var availableFields = await _service.GetTypedJsonStructure(context);

		foreach (var property in body.Properties())
		{
			if (property.Name == "on")
			{
				continue;
			}

			// Attempt to get the available field:
			var field = availableFields.GetField(property.Name, JsonFieldGroup.Any);

			if (field == null)
			{
				continue;
			}

			// Retain only FieldInfo and List virtual field values.
			var val = property.Value;

			if (
				field.FieldInfo != null || 
				(field.ContentField != null && field.ContentField.VirtualInfo != null && field.ContentField.VirtualInfo.IsList)
			) {
				if (first)
				{
					first = false;
				}
				else
				{
					str.Append(',');
				}

				str.Append('"');
				str.Append(property.Name);
				str.Append("\":");

				if (val == null)
				{
					str.Append("null");
				}
				else
				{
					var valStr = val.ToString();

					if (valStr == null)
					{
						str.Append("null");
					}
					else
					{
						str.Append(valStr);
					}
				}
			}
		}

		str.Append('}');

		var now = DateTime.UtcNow;

		return await revisions.Create(context, new Revision<T, ID>() {
			CreatedUtc = now,
			EditedUtc = now,
			ContentJson = str.ToString(),
			IsDraft = true,
			ActionType = 1
		});
	}
}
