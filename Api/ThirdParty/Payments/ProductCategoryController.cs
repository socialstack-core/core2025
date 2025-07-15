using Microsoft.AspNetCore.Mvc;

namespace Api.Payments
{
    /// <summary>Handles productCategory endpoints.</summary>
    [Route("v1/productCategory")]
    public class ProductCategoryController
        : BaseCategoryController<ProductCategory, ProductCategoryNode, ProductCategoryService>
    {
        public ProductCategoryController(ProductCategoryService service) : base(service)
        {
        }
    }
}
