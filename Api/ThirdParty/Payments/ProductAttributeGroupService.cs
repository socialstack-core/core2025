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
			Cache();
		}
	}
    
}
