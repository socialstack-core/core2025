using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Startup;
using System;
using System.Linq;
using Api.Pages;
using Api.CanvasRenderer;

namespace Api.Payments
{
	/// <summary>
	/// Handles products.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductService : AutoService<Product>
    {
		private PermalinkService _permalinks;
		private ProductConfig _config;
		private PriceService _prices;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductService(PermalinkService permalinks, PageService pages, PriceService prices) : base(Events.Product)
        {
			_permalinks = permalinks;
			_prices = prices;
			_config = GetConfig<ProductConfig>();

			InstallAdminPages("Products", "fa:fa-shopping-basket", ["id", "name", "minQuantity"]);

			Events.Product.BeforeCreate.AddEventListener(async (Context context, Product product) => {
				if (product == null)
				{
					return null;
				}

				// Ensure a slug is generated and is unique.
				if (string.IsNullOrEmpty(product.Slug))
				{
					product.Slug = await SlugGenerator.GenerateUniqueSlug(this, context, product.Name.Get(context));
				}

				await ValidateProduct(context, product);

				return product;
			});

			HashSet<string> excludeFields = new HashSet<string>() { "Categories", "Tags" };
            HashSet<string> nonAdminExcludeFields = new HashSet<string>() { "RolePermits", "UserPermits" };

            Events.Product.BeforeSettable.AddEventListener((Context ctx, JsonField<Product, uint> field) =>
            {
                if (field == null)
                {
                    return new ValueTask<JsonField<Product, uint>>(field);
                }

                // hide the core taxonomy fields as we have product specific ones
                if (excludeFields.Contains(field.Name))
                {
                    field.Writeable = false;
                    field.Hide = true;
                }

                // only admin can amend the critical fields
				// todo move this into a seperate service for all entites? 
                if (field.ForRole != Roles.Developer && field.ForRole != Roles.Admin && nonAdminExcludeFields.Contains(field.Name))
                {
                    field.Writeable = false;
                    field.Hide = true;
                }

				if (field.Name == "Attributes" || field.Name == "AdditionalAttributes")
				{
					field.Module = "Admin/Payments/AttributeSelect";
				}

                if (field.Name == "ProductCategories")
                {
                    field.Module = "Admin/Payments/ProductCategorySelect";
                }

				if (field.Name == "Variants")
				{
					field.Module = "Admin/Payments/Variants/ValueEditor";
				}

				return new ValueTask<JsonField<Product, uint>>(field);
            });

			Events.Page.BeforePageInstall.AddEventListener((Context context, PageBuilder builder) =>
			{
				if (builder.ContentType == typeof(Product) && builder.PageType == CommonPageType.AdminList)
				{
					builder.GetContentRoot()
						.Empty()
						.AppendChild(new CanvasNode("Admin/Payments/ProductCategoryTree"));
				}

				return new ValueTask<PageBuilder>(builder);
			});

			pages.Install(
				// Install a default primary product category page.
				// Note that this does not define a URL, because we want nice readable slug based URLs.
				// Because slugs can change, the URL is therefore not necessarily constant and thus
				// must be handled at the permalink level, which the event handler further down does.
				new PageBuilder()
				{
					Key = "primary:product",
					PrimaryContentIncludes = "productCategories,attributes,attributes.attribute,calculatedPrice,variants,variants.calculatedPrice,variants.additionalAttributes,variants.additionalAttributes.attribute,variants.attributes.attribute,variants.calculatedPrice,breadcrumb",
					Title = "${product.name}",
					BuildBody = (PageBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("UI/Product/View").WithPrimaryLink("product")
						);
					}
				},
				new PageBuilder()
				{
					Key = "product_search",
					Url = "product/search",
					Title = "Search for products",
					BuildBody = (PageBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("UI/Product/Search")
						);
					}
				}
			);

			Events.Product.AfterCreate.AddEventListener(async (Context context, Product product) => {

				// Permalink target which will be for whichever page wants to handle a product as its primary content.
				// If a specific page for this product exists, it will ultimately pick that.
				var linkTarget = permalinks.CreatePrimaryTargetLocator(this, product);

				Product parentProduct = null;

				if (product.VariantOfId.HasValue && product.VariantOfId.Value != 0)
				{
					parentProduct = await Get(context, product.VariantOfId.Value);

					if (parentProduct == null)
					{
						throw new PublicException(
							"Product is a variant of #" + product.VariantOfId + " but that product does not exist.", 
							"product/parent_not_found"
						);
					}
				}

				await permalinks.Create(
					context,
					new Permalink()
					{
						Url = GetInitialProductUrl(product, parentProduct),
						Target = linkTarget
					},
					DataOptions.IgnorePermissions
				);

				return product;
			});
			
			Events.Product.BeforeUpdate.AddEventListener(async (Context context, Product toUpdate, Product original) => {

				if (toUpdate == null)
				{
					return null;
				}

				// Validate:
				await ValidateProduct(context, toUpdate);

				Product parentProduct = null;

				if (toUpdate.VariantOfId.HasValue && toUpdate.VariantOfId.Value != 0)
				{
					parentProduct = await Get(context, toUpdate.VariantOfId.Value);

					if (parentProduct == null)
					{
						throw new PublicException(
							"Product is a variant of #" + toUpdate.VariantOfId + " but that product does not exist.",
							"product/parent_not_found"
						);
					}
				}

				// Did the slug, sku or parent state change?
				if (
					toUpdate.VariantOfId != original.VariantOfId || // Variant of changed (usually to/from 0)
					(toUpdate.Slug != original.Slug && toUpdate.VariantOfId == 0) || // Slug changed and not a variant
					(toUpdate.Sku != original.Sku && toUpdate.VariantOfId != 0) // Sku changed and is a variant
				) {

					// Update the permalink. It's possible that this will attempt to create a duplicate
					// in which case it functionally acts like the requested one is the new canonical link.
					var permalinkInfo = new PermalinkUrlTarget()
					{
						Url = GetInitialProductUrl(toUpdate, parentProduct),
						Target = _permalinks.CreatePrimaryTargetLocator(this, toUpdate)
					};

					await _permalinks.BulkCreate(context, [permalinkInfo]);
				}

				return toUpdate;
			});
			
			Events.Product.BeforeCreate.AddEventListener(async (ctx, product) =>
			{
				await UpdateProductCategoryMapping(ctx, product);	
				return product;
			});
			Events.Product.BeforeUpdate.AddEventListener(async (ctx, product, original) =>
			{
				await UpdateProductCategoryMapping(ctx, product);
				return product;
			});

			Cache();
		}

		private async ValueTask UpdateProductCategoryMapping(Context context, Product product)
		{
			// couldn't add as a dep due to circular ref
			var _categories = Services.Get<ProductCategoryService>();
			
			// contains all the uint => PCNode mappings, can be null too.
			var tree = await _categories.GetLookup(context);
			
			var allCats = new List<ulong>();

			var mappings = product.Mappings.Get("productcategories");

			if (mappings is null)
			{
				product.Mappings.Remove("childOfCategories");
				return;
			}

			foreach (var catId in mappings)
			{
				if (!tree.TryGetValue((uint) catId, out var categoryNode))
				{
					continue;
				}
				if (categoryNode?.BreadcrumbCategories is null || categoryNode.BreadcrumbCategories.Count == 0)
				{
					continue;
				}
				
				allCats.AddRange(categoryNode.BreadcrumbCategories.Select(category => (ulong) category.Id));
			}
			
			product.Mappings.Set("childOfCategories", allCats.Distinct().ToList());
			
		}
		
		/// <summary>
		/// Adds a validation layer to <c>Product</c> only,
		/// this checks fields strictly on the product.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="product"></param>
		/// <returns></returns>
		/// <exception cref="PublicException"></exception>
		private ValueTask<Product> ValidateProduct(Context context, Product product)
		{

			if (string.IsNullOrEmpty(product.Name.Get(context)))
			{
				throw new PublicException("The product name cannot be empty.", "product-validation/no-name");
			}

			if (string.IsNullOrEmpty(product.Slug))
			{
				throw new PublicException("The product slug cannot be empty.", "product-validation/no-slug");
			}
			
			return ValueTask.FromResult(product);
		}

		/// <summary>
		/// Use the primaryUrl system instead of calling this directly.
		/// </summary>
		/// <param name="product"></param>
		/// <param name="parentProduct">Present if the product is a variant.</param>
		/// <returns></returns>
		private string GetInitialProductUrl(Product product, Product parentProduct)
		{
			if (parentProduct != null)
			{
				// Product is a variant of parentProduct.
				return "/product/" + parentProduct.Slug + "?sku=" + product.Sku;
			}

			return "/product/" + product.Slug;
		}

		/// <summary>
		/// A convenience brute-force method for ensuring that all required permalinks exist.
		/// Best used after a major database edit (such as importing outside of SS).
		/// </summary>
		/// <returns></returns>
		public async ValueTask SyncPermalinks(Context context)
		{
            Log.Warn("product", "Sync product permalinks");

			var allProducts = await Where("", DataOptions.IgnorePermissions).ListAll(context);
			var links = new List<PermalinkUrlTarget>();

			// Created if any variants exist
			Dictionary<uint, Product> lookup = null;


			foreach (var product in allProducts)
			{
				// Permalink target which will be for whichever page wants to handle a product as its primary content.
				// If a specific page for this product exists, it will ultimately pick that.
				var linkTarget = _permalinks.CreatePrimaryTargetLocator(this, product);

				Product parentProduct = null;
				if (product.VariantOfId.HasValue && product.VariantOfId.Value != 0)
				{
					if (lookup == null)
					{
						lookup = new Dictionary<uint, Product>();

						foreach (var prod in allProducts)
						{
							lookup[prod.Id] = prod;
						}
					}

					if (!lookup.TryGetValue(product.VariantOfId.Value, out parentProduct))
					{
						// Invalid variant!
						Log.Warn(LogTag, "Invalid variant: " + product.Id + " is a variant of #" + product.VariantOfId.Value + " but that parent product doesn't exist.");
						continue;
					}
				}

				var permalinkInfo = new PermalinkUrlTarget() {
					Url = GetInitialProductUrl(product, parentProduct),
					Target = linkTarget
				};

				links.Add(permalinkInfo);
			}

			await _permalinks.BulkCreate(context, links);
		}

		/// <summary>
		/// True if it should error if an order for less than the min is placed.
		/// Otherwise it will be rounded up.
		/// </summary>
		public bool ErrorIfBelowMinimum => _config.ErrorIfBelowMinimum;

		/// <summary>
		/// Gets the product tiers for a given product. The result is null if there are none.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<List<Price>> GetPriceTiers(Context context, Product product)
		{
			if (product == null)
			{
				return null;
			}

			// System generated contextual pricing if necessary:
			List<Price> tiers = null;
			tiers = await Events.Product.Pricing.Dispatch(context, tiers, product);

			if (tiers == null)
			{
				// System default prices (priceTiers mapping on a product)
				tiers = await _prices.ListBySource(context, product, "priceTiers", DataOptions.IgnorePermissions);
			}

			if (tiers != null && tiers.Count > 0)
			{
				// Tiers is not necessarily sorted, so:
				tiers.Sort((Price a, Price b) => {
					return a.MinimumQuantity.CompareTo(b.MinimumQuantity);
				});
			}

			return tiers;
		}
	}
    
}
