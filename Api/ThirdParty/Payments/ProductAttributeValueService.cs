using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

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
			Cache();
		}
	}
    
}
