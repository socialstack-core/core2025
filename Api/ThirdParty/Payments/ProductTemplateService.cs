using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.Payments
{
	/// <summary>
	/// Handles productTemplates.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductTemplateService : AutoService<ProductTemplate>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductTemplateService() : base(Events.ProductTemplate)
        {
			// Example admin page install:
			// InstallAdminPages("ProductTemplates", "fa:fa-rocket", new string[] { "id", "name" });
		}
	}
    
}
