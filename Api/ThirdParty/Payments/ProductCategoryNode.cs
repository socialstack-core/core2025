using Newtonsoft.Json;
using System.Collections.Generic;

namespace Api.Payments;


/// <summary>
/// A product category in the cache tree.
/// </summary>
public class ProductCategoryNode
{
	/// <summary>
	/// The category itself.
	/// </summary>
	public ProductCategory Category;
	
	/// <summary>
	/// Local field used for building category structure in memory
	/// </summary>
	public List<ProductCategoryNode> Children { get; set; } = new();

    /// <summary>
    /// Local fields used for building category structure in memory
    /// </summary>
    [JsonIgnore]
    public ProductCategoryNode Parent { get; set; }

	/// <summary>
	/// Local field for any associated products for the category
	/// </summary>
	public List<ProductNode> Products { get; set; } = new();

	/// <summary>
	/// The collapsed set of parent categories including Category itself.
	/// </summary>
	public List<ProductCategory> BreadcrumbCategories;
}