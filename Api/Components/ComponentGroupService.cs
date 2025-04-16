using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.Components
{
	/// <summary>
	/// Handles componentGroups.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ComponentGroupService : AutoService<ComponentGroup>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ComponentGroupService() : base(Events.ComponentGroup)
        {
			// Example admin page install:
			// InstallAdminPages("ComponentGroups", "fa:fa-rocket", new string[] { "id", "name" });
		}
	}
    
}
