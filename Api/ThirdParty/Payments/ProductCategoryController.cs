using Api.Contexts;
using Api.Startup;
using Api.Startup.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Api.Pages.PageController;

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
		/// Gets information about a category tree node. The path is a 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		/// <exception cref="PublicException"></exception>
		[HttpPost("tree")]
		public async ValueTask<TreeNodeDetail?> GetTreeNode(Context context, [FromBody] CategoryTreeLocation location)
		{
			return await GetTreeNodePath(context, location.Path);
		}

		/// <summary>
		/// Gets information about a category tree node at a path defined by slug/slug/.. . 
		/// Only actually the last slug matters.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="PublicException"></exception>
		[HttpGet("tree")]
		public async ValueTask<TreeNodeDetail?> GetTreeNodePath(Context context, [FromQuery] string path)
		{
			if (!context.Role.CanViewAdmin)
			{
				throw new PublicException("Admins only", "category_tree/admin_only");
			}

			return await (_service as ProductCategoryService).GetTreeNodeAtPath(context, path);
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
	    
		/// <summary>
		/// Gets category descendants as a flat array
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/descendants")]
		public async ValueTask<ContentStream<ProductCategory, uint>> GetDescendants(Context context, [FromRoute] uint id)
		{
			return new ContentStream<ProductCategory, uint>(await (_service as ProductCategoryService).GetChildrenAsFlatList(context, id), _service);
		}

	}

	/// <summary>
	/// A location in the category tree.
	/// </summary>
	public struct CategoryTreeLocation
	{
		/// <summary>
		/// The path to resolve relative to. Empty string (not /) indicates root set.
		/// </summary>
		public string Path;
	}

}