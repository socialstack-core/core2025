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
		public ProductAttributeValueService(PageService pages) : base(Events.ProductAttributeValue)
		{

			pages.Install(
				new Page()
				{
					Url = "/en-admin/productattribute/${productattribute.id}/values",
					Key = "admin_editor:productattributevalue",
					Title = "Edit product attribute values",
					PrimaryContentType = "ProductAttribute",
					PrimaryContentIncludes = "",
					BodyJson = @"{
						""c"": {
							""t"": ""Admin/Payments/ProductAttribute/ValueEditor"",
							""l"": {""attribute"": {""primary"": true}}
						}
					}"
				}
			);

			Cache();
		}

	}
    
}
