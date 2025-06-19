using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Payments;

/// <summary>Handles product search endpoints.</summary>
[Route("v1/product/search")]
public partial class ProductSearchController : AutoController
{
	ProductSearchService _productSearchService;
	ProductService _productService;

	/// <summary>
	/// Instanced automatically.
	/// </summary>
	/// <param name="productSearchService"></param>
	/// <param name="productService"></param>
	public ProductSearchController(ProductSearchService productSearchService, ProductService productService)
	{
		_productSearchService = productSearchService;
		_productService = productService;
	}

	/// <summary>
	/// Faceted product search. You can provide text and optionally facets.
	/// It will then return the product set along with facets to display.
	/// Note that Atlas search is required for this to be full-featured (full facet sets)
	/// but you will still see a functional subset otherwise.
	/// </summary>
	/// <returns></returns>
	[HttpGet("faceted")]
	public ValueTask<ContentStream<Product, uint>?> GetFaceted(Context context, [FromQuery] string query)
	{
		return Faceted(context, new ProductSearchRequest()
		{
			Query = query
		});
	}

	/// <summary>
	/// Faceted product search. You can provide text and optionally facets.
	/// It will then return the product set along with facets to display.
	/// Note that Atlas search is required for this to be full-featured (full facet sets)
	/// but you will still see a functional subset otherwise.
	/// </summary>
	/// <returns></returns>
	[HttpPost("faceted")]
	public async ValueTask<ContentStream<Product, uint>?> Faceted(Context context, [FromBody] ProductSearchRequest request)
	{
		var resultSet = await _productSearchService.Search(context, request.Query, request.AppliedFacets, request.PageOffset);

		if (resultSet == null)
		{
			return null;
		}

		// In order to support includes and help out the SSR on this endpoint
		// we need to be returning a ContentStream. Because we are also returning facet sets
		// we thus need to use a mutli-stream: the primary result set is the list of products
		// and the secondary ones are any of the facets.
		// This results in json of the form:
		// {
		//     "results": [product, product, ..],
		//     "includes": [any includes for the products],
		//     "secondary": {
		//       "attributes": {
		//           "results": [attrFacet, attrFacet, ..],
		//           "includes": [any includes for attribute facets]
		//       },
		//       "productCategories": {
		//           "results": [catFacet, catFacet, ..],
		//           "includes": [any includes for category facets]
		//       }
		//     }
		// }

		var secondarySources = new SecondaryListStreamSource<AttributeValueFacet>(
			"attributes", 
			resultSet.ResultFacets.Attributes
		);

		secondarySources.Next = new SecondaryListStreamSource<ProductCategoryFacet>(
			"productCategories",
			resultSet.ResultFacets.Categories
		);

		return new ContentStream<Product, uint>() {
			ServiceForType = _productService,
			Source = new MultiStreamSource<Product, uint>(
				new ListStreamSource<Product, uint>(
					resultSet.Products
				),
				secondarySources
			)
		};
	}
	
}

/// <summary>
/// A search result.
/// </summary>
public struct ProductSearchResult
{
	/// <summary>
	/// The current page of products.
	/// </summary>
	public List<Product> Products;

	/// <summary>
	/// Total result count.
	/// </summary>
	public int Total;

	/// <summary>
	/// Facets present on this result set.
	/// </summary>
	public ProductSearchFacets Facets;
}

/// <summary>
/// A search request.
/// </summary>
public class ProductSearchRequest
{
	/// <summary>
	/// Page offset.
	/// </summary>
	public int PageOffset;

	/// <summary>
	/// The search text itself.
	/// </summary>
	public string Query;

	/// <summary>
	/// Optional applied facets per mapping (e.g. you want to filter results by attributes containing the colour 'blue').
	/// </summary>
	public List<ProductSearchAppliedFacet> AppliedFacets;
}