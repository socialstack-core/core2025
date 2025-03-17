using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.Regions
{
	/// <summary>
	/// Handles regions.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class RegionService : AutoService<Region>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public RegionService() : base(Events.Region)
        {
			// Example admin page install:
			InstallAdminPages("Regions", "fa:fa-table-cells-large", ["id", "name"]);
		}
	}
    
}
