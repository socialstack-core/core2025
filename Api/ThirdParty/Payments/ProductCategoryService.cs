using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Pages;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace Api.Payments
{
	/// <summary>
	/// Handles productCategories.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductCategoryService : AutoService<ProductCategory>
    {
		private ProductService _productService;

		/// <summary>
		/// The cached category tree. Use GetTree to obtain one instead of this directly.
		/// </summary>
		private CategoryTree? _categoryTree;

		/// <summary>
		/// Get the cached lookup table (and rebuild if necessary)
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		protected async Task<Dictionary<uint, ProductCategoryNode>> GetLookup(Context ctx)
		{
			var tree = await GetTree(ctx);
			return tree.Lookup;
		}

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductCategoryService(ProductService productService, PageService pages, PermalinkService permalinks) : base(Events.ProductCategory)
        {
			_productService = productService;

			// Example admin page install:
			InstallAdminPages("Product Categories", "fa:fa-rocket", new string[] { "id", "name" });

			pages.Install(
				// Install a default primary product category page.
				// Note that this does not define a URL, because we want nice readable slug based URLs.
				// Because slugs can change, the URL is therefore not necessarily constant and thus
				// must be handled at the permalink level, which the event handler further down does.
				new Page()
				{
					Key = "primary:productcategory",
					Title = "${productcategory.name}",
					BodyJson = @"{
							""c"": {
								""g"": {
									""c"": [
										{
											""t"": ""Component"",
											""d"": {
												""componentType"": ""UI/ProductCategory/View""
											},
											""l"": {
												""productCategory"": {
													""n"": 1,
													""f"": ""output""
												}
											},
											""x"": 465,
											""y"": 36,
											""r"": true
										},
										{
											""t"": ""Content"",
											""d"": {
												""contentType"": ""primary"",
												""includes"": """"
											},
											""x"": 83,
											""y"": 25.5
										}
									]
								},
								""i"": 2
							},
							""i"": 3
						}"
				}
			);

			Events.Service.AfterStart.AddEventListener(async (Context ctx, object sender) =>
			{
				// build the initial memory structure 
				await GetTree(ctx);

				return new ValueTask<object>(sender);
			});

			Events.ProductCategory.BeforeUpdate.AddEventListener(async (Context ctx, ProductCategory update, ProductCategory orig) =>
			{
				// check for circular dependency (recursion loop)  
				if (await HasCircularReference(ctx, update.Id, update.ParentId))
				{
					return null;
				}

				return update;
			});

			Events.ProductCategory.AfterCreate.AddEventListener(async (Context ctx, ProductCategory productCategory) =>
			{
				_categoryTree = null;
				return productCategory;
			});

			Events.ProductCategory.AfterUpdate.AddEventListener(async (Context ctx, ProductCategory productCategory) =>
			{
				_categoryTree = null;
				return productCategory;
			});

			Events.ProductCategory.AfterDelete.AddEventListener(async (Context ctx, ProductCategory productCategory) =>
			{
				var updated = false;

				// relink any children to the parent of the deleted node
				var children = await Where("ParentId=?").Bind(productCategory.Id).ListAll(ctx);

				if (children != null && children.Count > 0)
				{
					foreach (var child in children)
					{
						await Update(ctx, child, (ctx, toUpdate, orig) =>
						{
							toUpdate.ParentId = productCategory.ParentId;
						});

						updated = true;
					}
				}

				// if updated will have already triggered a refresh
				if (!updated)
				{
					_categoryTree = null;
				}

				return productCategory;
			});

			Events.ProductCategory.AfterCreate.AddEventListener(async (Context context, ProductCategory category) => {

				// Permalink target which will be for whichever page wants to handle a product category as its primary content.
				// If a specific page for this category exists, it will ultimately pick that.
				var linkTarget = permalinks.CreatePrimaryTargetLocator(this, category);

				// Todo: collision avoidance
				await permalinks.Create(
					context, 
					new Permalink()
					{
						Url = "/category/" + category.Slug,
						Target = linkTarget
					},
					DataOptions.IgnorePermissions
				);

				return category;
			});

			Cache();
		}

		/// <summary>
		/// Gets the products for a given product category. The result is null if there are none.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<List<Product>> GetProducts(Context ctx, uint id)
		{
			var productCategory = await Get(ctx, id);

			if (productCategory == null)
			{
				return null;
			}

			return await _productService.ListByTarget<ProductCategory, uint>(ctx, productCategory.Id, "ProductCategories");
		}


		/// <summary>
		/// Gets the product categories for a given product. The result is null if there are none.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<List<ProductCategory>> GetProductCategories(Context ctx, uint id)
		{
			var product = await _productService.Get(ctx, id);

			if (product == null)
			{
				return null;
			}

			return await ListBySource<Product, uint>(ctx, _productService, product.Id, "ProductCategories");
		}

		/// <summary>
		/// Gets the product categories for a given product. The result is null if there are none.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<List<ProductCategoryNode>> GetChildren(Context ctx, uint id)
		{
			var productCategory = await Get(ctx, id);

			if (productCategory == null)
			{
				return null;
			}

			if (await GetLookup(ctx) != null && (await GetLookup(ctx)).TryGetValue(id, out var foundCategory))
			{
				return foundCategory.Children?.Count > 0 ? foundCategory.Children : null;
			}

			return null;
		}

		/// <summary>
		/// Get any parents for a product category node
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<List<ProductCategoryNode>> GetParents(Context ctx, uint id)
		{
			var productCategory = await Get(ctx, id);

			if (productCategory == null)
			{
				return null;
			}

			var lookup = await GetLookup(ctx);

			if (lookup != null && lookup.TryGetValue(id, out var foundCategory))
			{
				var parents = new List<ProductCategoryNode>();

				var current = foundCategory;
				while (current.Category.ParentId.HasValue)
				{
					if (lookup.TryGetValue(current.Category.ParentId.Value, out var parent))
					{
						parents.Add(parent);
						current = parent;
					}
					else
					{
						// Break if ParentId is set but not found in lookup (e.g., data issue)
						break;
					}
				}

				return parents;
			}

			return null;
		}

		/// <summary>
		/// Get the entire category tree (hopefully from cache)
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public async Task<CategoryTree> GetTree(Context ctx)
		{
			var tree = _categoryTree;

			if (tree != null)
			{
				return tree.Value;
			}

			// Build a new one:
			var newTree = await BuildCategoryTree(ctx);

			// Cache it:
			_categoryTree = newTree;

			return newTree;
		}

		/// <summary>
		/// Get the entire category tree with products (debug use only)
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public async Task<CategoryTree> GetProductTreeAndProducts(Context ctx)
		{
			// no caching so slooooww
			return await BuildCategoryTree(ctx, true);
		}

		/// <summary>
		///  Create a clean slug value for a category
		///  /// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string ToSlug(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}

			var sb = new StringBuilder();

			foreach (char c in input.ToLowerInvariant())
			{
				if (char.IsLetterOrDigit(c))
				{
					sb.Append(c);
				}
				else
				{
					sb.Append('-');
				}
			}

			string result = Regex.Replace(sb.ToString(), "-{2,}", "-");

			return result.Trim('-');
		}


		/// <summary>
		/// Build the flat database list into a nested category structure
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="includeProducts"></param>
		/// <returns></returns>
		private async Task<CategoryTree> BuildCategoryTree(Context ctx, bool includeProducts = false)
		{
			Log.Info("productcategory", "Building product category structure");

			// get all categories 
			var categories = await Where("", DataOptions.IgnorePermissions).ListAll(ctx);

			// create a quick lookup dictionary
			var lookup = new Dictionary<uint, ProductCategoryNode>();

			foreach (var category in categories)
			{
				lookup[category.Id] = new ProductCategoryNode() {
					Category = category
				};
			}

			List<ProductCategoryNode> roots = new();

			foreach (var cat in categories)
			{
				var node = lookup[cat.Id];

				if (cat.ParentId.HasValue && lookup.TryGetValue(cat.ParentId.Value, out var parent))
				{
					node.Parent = parent;
					parent.Children.Add(node);
				}
				else
				{
					roots.Add(node);
				}
			}

			// calculate all the node full paths
			foreach (var category in roots)
			{
				SetNodeSlugs(category);
			}

			// build a lookup table now we have all the nodes expanded
			var catLookup = BuildCategoryLookup(roots);

			if (includeProducts)
			{
				var products = await _productService.Where("", DataOptions.IgnorePermissions).ListAll(ctx);

				// Attach products to categories (products can be linked to multiple category nodes)
				foreach (var product in products)
				{

					var listOfCategories = await ListBySource<Product, uint>(ctx, _productService, product.Id, "ProductCategories", DataOptions.IgnorePermissions);
					if (listOfCategories.Any())
					{
						foreach (var category in listOfCategories)
						{
							if (lookup.TryGetValue(category.Id, out var categoryNode))
							{
								var nodeProduct = new ProductNode
								{
									Product = product,
									Slug = BuildNodeSlugPath(categoryNode, product)
								};

								categoryNode.Products.Add(nodeProduct);
							}
						}
					}
				}
			}

			return new CategoryTree() {
				Lookup = catLookup,
				Roots = roots
			};
		}


		/// <summary>
		/// Extract the product categories into a lookup table
		/// </summary>
		/// <param name="roots"></param>
		/// <returns></returns>
		private Dictionary<uint, ProductCategoryNode> BuildCategoryLookup(List<ProductCategoryNode> roots)
		{
			var lookup = new Dictionary<uint, ProductCategoryNode>();

			void Traverse(ProductCategoryNode node)
			{
				if (!lookup.ContainsKey(node.Category.Id))
				{
					lookup[node.Category.Id] = node;

					foreach (var child in node.Children)
					{
						Traverse(child);
					}
				}
			}

			foreach (var root in roots)
			{
				Traverse(root);
			}

			return lookup;
		}


		/// <summary>
		/// Extract the full path of a category node
		/// </summary>
		/// <param name="node"></param>
		/// <param name="product"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		private string BuildNodeSlugPath(ProductCategoryNode node, Product product = null, string separator = "/")
		{
			var slugs = new Stack<string>();

			if (product != null)
			{
				if (!string.IsNullOrWhiteSpace(product.Slug))
				{
					slugs.Push(product.Slug);
				}
				else if (string.IsNullOrWhiteSpace(product.Name))
				{
					slugs.Push(ToSlug(product.Name));
				}
			}

			while (node != null)
			{
				if (!string.IsNullOrWhiteSpace(node.Category.Slug))
				{
					slugs.Push(node.Category.Slug);
				}
				else if (string.IsNullOrWhiteSpace(node.Category.Name))
				{
					slugs.Push(ToSlug(node.Category.Name));
				}
				node = node.Parent!;
			}

			return string.Join(separator, slugs.Select(ToSlug));
		}

		private void SetNodeSlugs(ProductCategoryNode node)
		{
			if (node != null)
			{
				node.FullPathSlug = BuildNodeSlugPath(node);

				if (node.Children.Count > 0)
				{
					foreach (var child in node.Children)
					{
						SetNodeSlugs(child);
					}
				}
			}
		}

		/// <summary>
		/// Check for rescursive entries
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="id"></param>
		/// <param name="newParentId"></param>
		/// <returns></returns>
		public async Task<bool> HasCircularReference(Context ctx, uint id, uint? newParentId)
		{
			var currentParentId = newParentId;

			while (currentParentId.HasValue)
			{
				if (currentParentId.Value == id)
				{
					// Found circular reference
					return true;
				}

				// to be sure use the database version 
				var parentCategory = await Get(ctx, currentParentId.Value, DataOptions.NoCacheIgnorePermissions);
				if (parentCategory == null)
				{
					// Parent not found
					return true;
				}

				currentParentId = parentCategory.ParentId;
			}

			return false;
		}
	}

	/// <summary>
	/// The category tree.
	/// </summary>
	public struct CategoryTree
	{
		/// <summary>
		/// Roots of the tree.
		/// </summary>
		public List<ProductCategoryNode> Roots;

		/// <summary>
		/// A lookup to a particular node in the tree.
		/// </summary>
		public Dictionary<uint, ProductCategoryNode> Lookup;
	}
    
}
