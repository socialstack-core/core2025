using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Startup;
using System;
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

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductService(PermalinkService permalinks, PageService pages) : base(Events.Product)
        {
			_permalinks = permalinks;
			_config = GetConfig<ProductConfig>();

			InstallAdminPages(["id", "name", "minQuantity"]);

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
					PrimaryContentIncludes = "productCategories",
					Title = "${product.name}",
					BuildBody = (PageBuilder builder) =>
					{
						return builder.AddTemplate(
							new CanvasNode("UI/Product/View").WithPrimaryLink("product")
						);
					}
				}
			);

			Events.Product.AfterCreate.AddEventListener(async (Context context, Product product) => {

				// Permalink target which will be for whichever page wants to handle a product as its primary content.
				// If a specific page for this product exists, it will ultimately pick that.
				var linkTarget = permalinks.CreatePrimaryTargetLocator(this, product);

				await permalinks.Create(
					context,
					new Permalink()
					{
						Url = GetInitialProductUrl(product),
						Target = linkTarget
					},
					DataOptions.IgnorePermissions
				);

				return product;
			});

			Cache();
		}

		/// <summary>
		/// Use the primaryUrl system instead of calling this directly.
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		private string GetInitialProductUrl(Product product)
		{
			return "/product/" + product.Slug;
		}

		/// <summary>
		/// A convenience brute-force method for ensuring that all required permalinks exist.
		/// Best used after a major database edit (such as importing outside of SS).
		/// </summary>
		/// <returns></returns>
		public async ValueTask SyncPermalinks(Context context)
		{
			var allProducts = await Where("", DataOptions.IgnorePermissions).ListAll(context);
			var links = new List<PermalinkUrlTarget>();

			foreach (var product in allProducts)
			{
				// Permalink target which will be for whichever page wants to handle a product as its primary content.
				// If a specific page for this product exists, it will ultimately pick that.
				var linkTarget = _permalinks.CreatePrimaryTargetLocator(this, product);

				var permalinkInfo = new PermalinkUrlTarget() {
					Url = GetInitialProductUrl(product),
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
		public async ValueTask<List<Product>> GetTiers(Context context, Product product)
		{
			if (product == null)
			{
				return null;
			}

			var tiers = await ListBySource(context, this, product.Id, "Tiers", DataOptions.IgnorePermissions);

			if (tiers != null && tiers.Count > 0)
			{
				// Tiers is not necessarily sorted, so:
				tiers.Sort((Product a, Product b) => {
					return a.MinQuantity.CompareTo(b.MinQuantity);
				});
			}

			return tiers;
		}
	}
    
}
