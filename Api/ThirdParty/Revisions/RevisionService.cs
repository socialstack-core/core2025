using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using System;
using Api.Users;

namespace Api.Revisions
{
	/// <summary>
	/// A typeless revision service.
	/// </summary>
	public partial interface RevisionService
    {
		/// <summary>
		/// Generic publishing by ulong ID.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		ValueTask<object> PublishGenericId(Context context, ulong id, DataOptions options = DataOptions.Default);
	}

	/// <summary>
	/// Revision service for a particular type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="ID"></typeparam>
	public class RevisionService<T, ID> : AutoService<T, ID>, RevisionService
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{

		/// <summary>
		/// The parent service.
		/// </summary>
		public AutoService<T, ID> Parent;

		/// <summary>
		/// Instances as revision service for a particular type. See also: aService.Revisions
		/// </summary>
		public RevisionService(AutoService<T, ID> parent) : base(new EventGroup<T, ID>(), null, parent.EntityName + "_revisions")
		{
			Parent = parent;
		}

		/// <summary>
		/// Gets the revision service on this autoservice, if there is one.
		/// </summary>
		/// <returns></returns>
		public override RevisionService GetRevisions()
		{
			return this;
		}

		/// <summary>
		/// Publish a revision by a generic revision ID.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public async ValueTask<object> PublishGenericId(Context context, ulong id, DataOptions options = DataOptions.Default)
		{
			return await Publish(context, ConvertId(id), options);
		}

		/// <summary>
		/// Publish a revision by a revision ID.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public async ValueTask<T> Publish(Context context, ID id, DataOptions options = DataOptions.Default)
		{
			var content = await Get(context, id);
			return await PublishRevision(context, content, options);
		}

		/// <summary>
		/// Publishes the given entity, which originated from a revision. The entity content ID may not exist at all.
		/// </summary>
		public virtual async ValueTask<T> PublishRevision(Context context, T entity, DataOptions options = DataOptions.Default)
		{
			var vc = entity as VersionedContent<ID>;

			if (vc == null)
			{
				// ??
				return null;
			}

			var revId = vc.RevisionId;

			if (!revId.HasValue)
			{
				throw new Exception("Can't publish a non-revision row.");
			}

			if (vc.Id.Equals(default(ID)))
			{
				// It has not been created at all yet.
				return await Parent.Create(context, entity, options);
			}
			else
			{
				// This is actually an update on the row.
				await Parent.UpdateExact(context, entity, options);
			}

			// Mark the revision as no longer a draft and set its publish time.
			await Update(context, revId.Value, (Context ctx, T toUpdate, T orig) => {
				var vcUpdate = toUpdate as VersionedContent<ID>;
				vcUpdate.PublishDraftDate = DateTime.UtcNow;
				vcUpdate.IsDraft = false;
			}, options);

			return null;
		}

		/// <summary>
		/// Revisions are type proxies: they are not the primary service for type T.
		/// </summary>
		public override bool IsTypeProxy => true;
	}

}
