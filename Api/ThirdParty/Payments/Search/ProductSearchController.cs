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
		var resultSet = await _productSearchService.Search(context, request.Query, request.SearchType, request.AppliedFacets, request.PageOffset);

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
		//       "attributeValueFacets": {
		//           "results": [attrFacet, attrFacet, ..],
		//           "includes": [any includes for attribute facets]
		//       },
		//       "productCategoryFacets": {
		//           "results": [catFacet, catFacet, ..],
		//           "includes": [any includes for category facets]
		//       }
		//     }
		// }
		
		// These are declared on Product as HasSecondaryResult, which isn't mandatory but does make them support includes as well.
		// This means you can include the attributeValue,category and attribute plus anything else you might want
		// even though these AttributeValueFacet instances are not actually regular content types.
		var secondarySources = new SecondaryListStreamSource<AttributeValueFacet>(
			"attributeValueFacets", 
			resultSet.ResultFacets.Attributes
		);

		secondarySources.Next = new SecondaryListStreamSource<ProductCategoryFacet>(
			"productCategoryFacets",
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
	
	/// <summary>
	/// The max result set
	/// </summary>
	public uint PageSize;
	
	/// <summary>
	/// Is it a reductive or an expansive search.
	/// </summary>
	public ProductSearchType SearchType;
	/// <summary>
	/// Defaults to 0, minimum price.
	/// </summary>
	public double MinPrice = 0;
	
	/// <summary>
	/// Defaults to 5000, maximum price.
	/// </summary>
	public double MaxPrice = 5000;
	
	/// <summary>
	/// Approved stock only
	/// </summary>
	public bool ApprovedStockOnly = false;
	
	/// <summary>
	/// In stock only.
	/// </summary>
	public bool InStockOnly = false;
}

/// <summary>
/// An enum to set whether the query is reductive or expansive. 
/// </summary>
public enum ProductSearchType
{
	/// <summary>
	/// Used to narrow down a target product, works how you'd expect on an admin panel.
	/// I want products that only have...
	/// </summary>
	Reductive,
	/// <summary>
	/// Used to broaden the net cast to the product pool, by saying I want products that have any of....
	/// </summary>
	Expansive
}