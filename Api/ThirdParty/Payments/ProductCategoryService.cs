using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Pages;

namespace Api.Payments
{
	/// <summary>
	/// Handles productCatgeories.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ProductCategoryService : AutoService<ProductCategory>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ProductCategoryService(PageService pages, PermalinkService permalinks) : base(Events.ProductCategory)
        {
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
												""includes"": ""productCategories,productCategories.primaryUrl""
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
		}
	}
    
}
