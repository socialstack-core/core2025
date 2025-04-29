using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.Payments
{
	/// <summary>
	/// Handles productAttributes.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductAttributeService : AutoService<ProductAttribute>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductAttributeService() : base(Events.ProductAttribute)
        {
			// Example admin page install:
			InstallAdminPages("Product Attributes", "fa:fa-rocket", new string[] { "id", "name" });
		}
	}
    
}
