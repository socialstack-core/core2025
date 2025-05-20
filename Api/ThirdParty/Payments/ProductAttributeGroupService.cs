using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.Payments
{
	/// <summary>
	/// Handles productAttributeGroups.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductAttributeGroupService : AutoService<ProductAttributeGroup>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductAttributeGroupService() : base(Events.ProductAttributeGroup)
        {
			// (Don't inject productAttributeService, it already uses this one)

			// Groups need an edit and basic list page, but not a nav menu entry as their 'home' is via the products link.
			InstallAdminPages(null, null, new string[] { "id", "name" });

			Cache();
		}

	}
    
}
