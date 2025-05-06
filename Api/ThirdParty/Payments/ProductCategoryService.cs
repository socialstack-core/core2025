using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.Payments
{
	/// <summary>
	/// Handles productCatgeories.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductCategoryService : AutoService<ProductCategory>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductCategoryService() : base(Events.ProductCategory)
        {
			// Example admin page install:
			InstallAdminPages("Product Categories", "fa:fa-rocket", new string[] { "id", "name" });
		}
	}
    
}
