using System;
using System.Threading.Tasks;
using Api.CanvasRenderer;
using Api.Contexts;
using Api.Eventing;
using Api.Pages;

namespace Api.Payments
{
	/// <summary>
	/// Handles productAttributeValues.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductAttributeValueService : AutoService<ProductAttributeValue>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductAttributeValueService() : base(Events.ProductAttributeValue)
        {
	        Events.Page.BeforeAdminPageInstall.AddEventListener((Context context, Page page, CanvasNode canvas, Type contentType, AdminPageType pageType) =>
	        {
		        if (contentType == typeof(ProductAttributeValue) && pageType == AdminPageType.List)
		        {
			        canvas.Module = "Admin/Payments/ProductAttribute/ValueEditor";
		        }

		        return new ValueTask<Pages.Page>(page);
	        });

	        InstallAdminPages("Product Attribute Values", "fa:rocket", ["id", "value"]);
	        
			Cache();
		}
	}
    
}
