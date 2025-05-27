using System;
using System.Threading.Tasks;
using Api.CanvasRenderer;
using Api.Contexts;
using Api.Eventing;
using Api.Pages;
using Api.Startup;

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
			
			Events.Page.BeforeAdminPageInstall.AddEventListener((Context context, Page page, CanvasNode canvasNode, Type type, AdminPageType pageType) => {
				
				if (type == typeof(ProductAttributeValue) && pageType == AdminPageType.List)
				{
					// // clear out any children.
					// canvasNode.Content = [];
					// canvasNode.Module = "Admin/Template/SinglePage";
					canvasNode.Module = "Admin/Payments/ProductAttribute/ValueEditor";
					page.Url = "/en-admin/productattribute/${productattribute.id}/values";
				}

				return new ValueTask<Page>(page);
			}, 2);
			
			Cache();

			InstallAdminPages(["id", "value"]);
		}

	}
    
}
