using Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.JavaScript;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Payments;

/// <summary>Handles product search endpoints.</summary>
[Route("v1/product/search")]
public partial class ProductSearchController : AutoController
{
	ProductSearchService productSearchService;

	/// <summary>
	/// Instanced automatically.
	/// </summary>
	/// <param name="_productSearchService"></param>
	public ProductSearchController(ProductSearchService _productSearchService)
	{
		productSearchService = _productSearchService;
	}

	/// <summary>
	/// Faceted product search. You can provide text and optionally facets.
	/// It will then return the product set along with facets to display.
	/// Note that Atlas search is required for this to be full-featured (full facet sets)
	/// but you will still see a functional subset otherwise.
	/// </summary>
	/// <returns></returns>
	[HttpPost("faceted")]
	public async ValueTask<ProductSearchResult> Faceted(Context context, [FromBody] ProductSearchRequest request)
	{
		var resultSet = await productSearchService.Search(context, request.Query, request.AppliedFacets, request.PageOffset);

		if (resultSet == null || resultSet.Products.Count == 0)
		{
			return new ProductSearchResult
			{
				Products = null,
				Total = 0
			};
		}

		// Expand attributes and categories.
		return new ProductSearchResult {
			Products = resultSet.Products,
			Total = resultSet.Total,
			Facets = resultSet.ResultFacets
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