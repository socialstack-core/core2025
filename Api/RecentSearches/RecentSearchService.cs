using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.RecentSearches
{
	/// <summary>
	/// Handles recentSearches.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class RecentSearchService : AutoService<RecentSearch>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public RecentSearchService() : base(Events.RecentSearch)
        {
			// Example admin page install:
			// InstallAdminPages("RecentSearches", "fa:fa-rocket", new string[] { "id", "name" });
		}
	}
    
}
