using Api.CanvasRenderer;
using Api.Eventing;
using Api.Pages;

namespace Api.Payments
{
    public partial class ProductCategoryService : BaseCategoryService<ProductCategory, ProductCategoryNode>
    {

        public override string CategoryLabel => "Product Category";
        public override string CategoryFieldName => "ProductCategories";
        public override string CategoryUrlPrefix => "category";

        public ProductCategoryService(
            ProductService products,
            PageService pages,
            PermalinkService permalinks
        ) : base(Events.ProductCategory, products, pages, permalinks)
        {

            InstallAdminPages("Product categories", "fa:fa-folder", ["id", "name"]);

            pages.Install(
                // Install a default primary product category page.
                // Note that this does not define a URL, because we want nice readable slug based URLs.
                // Because slugs can change, the URL is therefore not necessarily constant and thus
                // must be handled at the permalink level, which the event handler further down does.
                new PageBuilder()
                {
                    Key = "primary:productcategory",
                    Title = "${productcategory.name}",
                    PrimaryContentIncludes = "calculatedPrice, breadcrumb",
                    BuildBody = (PageBuilder builder) =>
                    {
                        return builder.AddTemplate(
                            // A prop called 'productCategory' will be the category referenced by the URL.
                            // If it does not exist, the page 404s, so you can expect it to be not-null always.
                            new CanvasNode("UI/ProductCategory/View").WithPrimaryLink("productCategory")
                        );
                    }
                }
            );
        }
    }
}
