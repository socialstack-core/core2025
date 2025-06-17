using System.Threading.Tasks;
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
			
			Events.ProductTemplate.BeforeCreate.AddEventListener(async (ctx, tpl) => await ValidateTemplate(ctx, tpl));
			Events.ProductTemplate.BeforeUpdate.AddEventListener(ValidateTemplate);
		}

		private ValueTask<ProductTemplate> ValidateTemplate(Context context, ProductTemplate template, ProductTemplate original = null)
		{
			
			// TODO: Implement these as the templates structure grows.
			
			return ValueTask.FromResult(template);
		}
	}
    
}
