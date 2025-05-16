using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Payments
{
    /// <summary>Handles productCategory endpoints.</summary>
    [Route("v1/productCategory")]
	public partial class ProductCategoryController : AutoController<ProductCategory>
    {

		/// <summary>
		/// List the entire product category structure
		/// </summary>
		/// <param name="context"></param>
		/// <param name="includeProducts"></param>
		/// <returns></returns>
		[HttpGet("structure")]
		public virtual async ValueTask<List<ProductCategoryNode>> Structure(Context context, [FromQuery] bool includeProducts = false)
		{
			if (includeProducts)
			{
				if (context.Role == null || !context.Role.CanViewAdmin)
				{
					throw new PublicException("Admin only", "Product Category/admin_required");
				}

				var slowTree = await (_service as ProductCategoryService).GetProductTreeAndProducts(context);
				return slowTree.Roots;
			}
			else
			{
				var tree = await (_service as ProductCategoryService).GetTree(context);
				return tree.Roots;
			}
		}

		/// <summary>
		/// List the products for a category
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/products")]
		public virtual async ValueTask<List<Product>> GetProducts(Context context, [FromRoute] uint id)
		{
			return await (_service as ProductCategoryService).GetProducts(context, id);
		}

		/// <summary>
		/// List the categories for a product
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("product/{id}")]
		public virtual async ValueTask<List<ProductCategory>> GetProductCategories(Context context, [FromRoute] uint id)
		{
			return await (_service as ProductCategoryService).GetProductCategories(context, id);
		}

		/// <summary>
		/// List the children for a category (equiv to a filter: ParentId=id)
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/children")]
		public virtual async ValueTask<List<ProductCategoryNode>> GetChildren(Context context, [FromRoute] uint id)
		{
			return await (_service as ProductCategoryService).GetChildren(context, id);
		}

		/// <summary>
		/// List the parents for a category
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/parents")]
		public virtual async ValueTask<List<ProductCategoryNode>> GetParents(Context context, [FromRoute] uint id)
		{
			return await (_service as ProductCategoryService).GetParents(context, id);
		}

	}
}