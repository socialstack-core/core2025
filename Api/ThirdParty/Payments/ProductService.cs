using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Startup;
using System;
using Api.Pages;

namespace Api.Payments
{
	/// <summary>
	/// Handles products.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductService : AutoService<Product>
    {
		private ProductConfig _config;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductService(PermalinkService permalinks, PageService pages) : base(Events.Product)
        {
			_config = GetConfig<ProductConfig>();

			InstallAdminPages("Products", "fa:fa-rocket", new string[] { "id", "name", "minQuantity" });

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

			pages.Install(
				// Install a default primary product category page.
				// Note that this does not define a URL, because we want nice readable slug based URLs.
				// Because slugs can change, the URL is therefore not necessarily constant and thus
				// must be handled at the permalink level, which the event handler further down does.
				new Page()
				{
					Key = "primary:product",
					Title = "${product.name}",
					BodyJson = @"{
							""c"": {
								""g"": {
									""c"": [
										{
											""t"": ""Component"",
											""d"": {
												""componentType"": ""UI/Product/View""
											},
											""l"": {
												""product"": {
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
												""includes"": ""productCategories""
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
