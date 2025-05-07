using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Startup;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Api.CanvasRenderer;
using System;

namespace Api.Pages
{
	/// <summary>
	/// Handles permalinks. Very similar concept to how wordpress works here: these are like
	/// URL aliases which are retained such that you can minimise link breakage when content changes.
	/// Unlike redirects which are handled by the webserver, permalinks are silent aliases but do have a concept of 
	/// canonical links: when multiple permalinks target the same thing, the most recently created permalink is the 
	/// canonical one. Requests that arrived via the old ones will be redirected.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class PermalinkService : AutoService<Permalink>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public PermalinkService() : base(Events.Permalink)
        {
			Events.Permalink.BeforeUpdate.AddEventListener((Context context, Permalink toUpdate, Permalink orig) => {
				throw new PublicException("Permalinks cannot be edited", "permalink/is-permanent");
			});

			Events.Permalink.AfterCreate.AddEventListener((Context context, Permalink link) => {

				if (_srcDictionary != null)
				{
					AddToDictionary(link, _srcDictionary);
				}

				return new ValueTask<Permalink>(link);
			});

			Events.Permalink.AfterDelete.AddEventListener((Context context, Permalink link) => {
				_srcDictionary = null;
				return new ValueTask<Permalink>(link);
			});
		}

		private Dictionary<string, List<Permalink>> _srcDictionary;

		/// <summary>
		/// Gets a dictionary for all source URLs to all target URLs.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async ValueTask<Dictionary<string, List<Permalink>>> GetSourceDictionary(Context context)
		{
			var result = _srcDictionary;

			if (result == null)
			{
				result = await CreateSourceDictionary(context);
				_srcDictionary = result;
			}

			return result;
		}

		/// <summary>
		/// A dictionary from target URLs to all its sources. 
		/// The first one in the list is always the canonical entry.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private async ValueTask<Dictionary<string, List<Permalink>>> CreateSourceDictionary(Context context)
		{
			var all = await Where().ListAll(context);
			var result = new Dictionary<string, List<Permalink>>();

			foreach (var link in all)
			{
				AddToDictionary(link, result);
			}

			return result;
		}

		private void AddToDictionary(Permalink link, Dictionary<string, List<Permalink>> set)
		{
			if (!set.TryGetValue(link.Target, out List<Permalink> sources))
			{
				sources = new List<Permalink>();
				set[link.Target] = sources;
				sources.Add(link);
				return;
			}

			// There will always be at least 1 entry in the set
			// so check if this incoming one is newer.
			var canon = sources[0];

			if (link.CreatedUtc > canon.CreatedUtc)
			{
				// This is the new canonical entry.
				sources[0] = link;
				sources.Add(canon);
			}
			else
			{
				sources.Add(link);
			}
		}

		private bool ContainsSource(string src, List<Permalink> links)
		{
			foreach (var link in links)
			{
				if (link.Url == src)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Bulk creates a block of permalinks. Entries will be skipped if the exact pairing already exists.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="links"></param>
		/// <returns></returns>
		public async ValueTask BulkCreate(Context context, List<PermalinkUrlTarget> links)
		{
			// Get the (usually cached) set of all permalinks:
			var set = await GetSourceDictionary(context);

			var setToCreate = new List<Permalink>();

			foreach (var link in links)
			{
				// If it already exists, skip it.
				// These are also blocked by the database using a unique index.
				if (set.TryGetValue(link.Target, out List<Permalink> sources))
				{
					if (ContainsSource(link.Url, sources))
					{
						continue;
					}
				}

				setToCreate.Add(new Permalink() {
					CreatedUtc = DateTime.UtcNow,
					UserId = context.UserId
				});
			}

			await CreateAll(context, setToCreate, DataOptions.IgnorePermissions);
		}

	}
    
}
