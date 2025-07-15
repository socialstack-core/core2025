using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Api.Pages.PageController;

namespace Api.Payments
{
    public abstract class BaseCategoryController<TCategory, TCategoryNode, TService> : AutoController<TCategory>
            where TCategory : BaseCategory, new()
            where TCategoryNode : CategoryNode<TCategory, TCategoryNode>, new()
            where TService : BaseCategoryService<TCategory, TCategoryNode>
    {
        protected readonly TService _service;

        protected BaseCategoryController(TService service)
        {
            _service = service;
        }

        /// <summary>
        /// List the entire product category structure
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeProducts"></param>
        /// <returns></returns>
        [HttpGet("structure")]
		public virtual async ValueTask<List<TCategoryNode>> Structure(Context context, [FromQuery] bool includeProducts = false)
		{
			if (includeProducts)
			{
				if (context.Role == null || !context.Role.CanViewAdmin)
				{
					throw new PublicException("Admin only", "Product Category/admin_required");
				}

				var slowTree = await _service.GetProductTreeAndProducts(context);
				return slowTree.Roots;
			}
			else
			{
				var tree = await _service.GetTree(context);
				return tree.Roots;
			}
		}

        [HttpGet("permalink/sync")]
        public async ValueTask<string> PermalinkSync(Context context)
        {
            if (context.Role == null || !context.Role.CanViewAdmin)
            {
                throw new PublicException("You do not have permission to view this endpoint", "permissions/not-admin");
            }

            if (_service.IsSyncRunning)
            {
                return "Sync already running";
            }

            await _service.SyncPermalinks(context);

            return "Content synced";
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

			return await _service.GetTreeNodeAtPath(context, path);
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
			return await _service.GetProducts(context, id);
		}

		/// <summary>
		/// List the categories for a product
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("product/{id}")]
		public virtual async ValueTask<List<TCategory>> GetProductCategories(Context context, [FromRoute] uint id)
		{
			return await _service.GetProductCategories(context, id);
		}

		/// <summary>
		/// List the children for a category (equiv to a filter: ParentId=id)
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/children")]
		public virtual async ValueTask<List<TCategoryNode>> GetChildren(Context context, [FromRoute] uint id)
		{
			return await _service.GetChildren(context, id);
		}


        /// <summary>
        /// List the parents for a category
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/parents")]
		public virtual async ValueTask<List<TCategoryNode>> GetParents(Context context, [FromRoute] uint id)
		{
			return await _service.GetParents(context, id);
		}
	    
		/// <summary>
		/// Gets category descendants as a flat array
		/// </summary>
		/// <param name="context"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}/descendants")]
		public async ValueTask<ContentStream<TCategory, uint>> GetDescendants(Context context, [FromRoute] uint id)
		{
			return new ContentStream<TCategory, uint>(await _service.GetChildrenAsFlatList(context, id), _service);
		}
    }
}