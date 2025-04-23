using Microsoft.AspNetCore.Mvc;

namespace Api.Payments
{
    /// <summary>Handles productAttribute endpoints.</summary>
    [Route("v1/productAttribute")]
	public partial class ProductAttributeController : AutoController<ProductAttribute>
    {
    }
}