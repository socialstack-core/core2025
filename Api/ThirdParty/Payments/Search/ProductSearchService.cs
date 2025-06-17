using Api.Contexts;
using Api.Eventing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Payments;


/// <summary>
/// Searches for products.
/// </summary>
public class ProductSearch
{
	/// <summary>
	/// The textual query.
	/// </summary>
	public string Query;
	/// <summary>
	/// Page offset.
	/// </summary>
	public int PageIndex;
	/// <summary>
	/// The page size.
	/// </summary>
	public int PageSize;
	/// <summary>
	/// True if the search was handled.
	/// </summary>
	public bool Handled;
	/// <summary>
	/// Total result count.
	/// </summary>
	public int Total;
	/// <summary>
	/// The discovered products.
	/// </summary>
	public List<Product> Products;
	/// <summary>
	/// The set of applied facet filters. Can be null if none are active.
	/// </summary>
	public List<ProductSearchFilter> AppliedFacets;
	/// <summary>
	/// Facets in the results. Can be null for DBE's that don't support it.
	/// </summary>
    public List<ProductSearchFacet> ResultFacets;
}

/// <summary>
/// A search filter for a particular mapping.
/// </summary>
public struct ProductSearchFilter
{
	/// <summary>
	/// The name of the mapping (e.g. "attributes").
	/// </summary>
	public string Mapping;

	/// <summary>
	/// The set of selected IDs. Should not be null if a filter is in use.
	/// </summary>
	public List<ulong> Ids;
}

/// <summary>
/// A search filter for a particular mapping.
/// </summary>
public struct ProductSearchFacet
{
	/// <summary>
	/// The name of the mapping (e.g. "attributes").
	/// </summary>
	public string Mapping;

	/// <summary>
	/// The set of facet results
	/// </summary>
	public List<ProductSearchFacetResult> Values;
}

/// <summary>
/// A particular facet result.
/// </summary>
public struct ProductSearchFacetResult
{
	/// <summary>
	/// E.g. the attribute value ID.
	/// </summary>
	public ulong EntityId;

	/// <summary>
	/// The number of results it has.
	/// </summary>
	public int Count;
}

/// <summary>
/// Handles product search with facets etc.
/// </summary>
public class ProductSearchService : AutoService
{
	private bool _searchFallback;

	/// <summary>
	/// Instanced automatically.
	/// </summary>
	/// <param name="products"></param>
	public ProductSearchService(ProductService products)
	{

		Events.Product.Search.AddEventListener(async (Context context, ProductSearch search) => {

			if (search.Handled)
			{
				// No need to fallback.
				return search;
			}

			if (!_searchFallback)
			{
				_searchFallback = true;
				Log.Info("productsearch", "Using basic search fallback due to no dedicated engine mounting the search event.");
			}

			// This fallback does not support full facet info.
			var filter = products
				.Where("Name contains ? or Description contains ?")
				.Bind(search.Query)
				.Bind(search.Query);
			filter.SetPage(search.PageIndex, search.PageSize);
			var productSet = await filter.ListAll(context);

			search.Total = productSet.Count;
			search.Handled = true;
			search.Products = productSet;
			return search;
		}, 11);
    }

	/// <summary>
	/// Searches products.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="query"></param>
	/// <param name="appliedFacets">Facets to filter by (e.g. you want products which are colour 'red' via an attributes facet)</param>
	/// <param name="pageOffset"></param>
	/// <param name="pageSize"></param>
	/// <returns></returns>
	public async ValueTask<ProductSearch> Search(
		Context context, string query, List<ProductSearchFilter> appliedFacets = null, int pageOffset = 0, int pageSize = 50)
    {
        var search = new ProductSearch(){
			Query = query,
			PageIndex = pageOffset,
			PageSize = pageSize
		};
		
		search = await Events.Product.Search.Dispatch(context, search);
		
		if(!search.Handled || search.Products == null)
		{
			return null;
		}

		if (search.ResultFacets == null)
		{
			// Derive basic facets. You need atlas search on MongoDB (or another engine handling the Search event above)
			// to have a proper facet set. These are basic because the result is paginated, meaning facets for the vast majority of 
			// the results will be simply not factored in here.
			var categories = new Dictionary<ulong, int>();
			var attributes = new Dictionary<ulong, int>();

			foreach (var product in search.Products)
			{
				var cats = product.Mappings.Get("productCategories");

				if (cats != null)
				{
					foreach (var catId in cats)
					{
						if (categories.TryGetValue(catId, out int currentCounter))
						{
							categories[catId] = currentCounter + 1;
						}
						else
						{
							categories[catId] = 1;
						}
					}
				}
				
				var attribs = product.Mappings.Get("attributes");

				if (attribs != null)
				{
					foreach (var attribId in attribs)
					{
						if (attributes.TryGetValue(attribId, out int currentCounter))
						{
							attributes[attribId] = currentCounter + 1;
						}
						else
						{
							attributes[attribId] = 1;
						}
					}
				}
			}

			search.ResultFacets = new List<ProductSearchFacet>()
			{
				ToFacetSet("productcategories", categories),
				ToFacetSet("attributes", attributes)
			};
		}

		return search;
    }

	private ProductSearchFacet ToFacetSet(string mappingName, Dictionary<ulong, int> uniqueIds)
	{
		var vals = new List<ProductSearchFacetResult>();

		foreach (var kvp in uniqueIds)
		{
			vals.Add(new ProductSearchFacetResult()
			{
				EntityId = kvp.Key,
				Count = kvp.Value
			});
		}

		return new ProductSearchFacet()
		{
			Mapping = mappingName,
			Values = vals
		};
	}
}