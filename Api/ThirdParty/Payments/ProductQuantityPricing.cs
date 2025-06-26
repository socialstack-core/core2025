using Newtonsoft.Json;
using System.Collections.Generic;

namespace Api.Payments;


/// <summary>
/// Contents of a shopping cart.
/// </summary>
public class ProductQuantityPricing
{
	/// <summary>
	/// Total incl. any relevant taxes and any coupons applied.
	/// </summary>
	public ulong Total;
	
	/// <summary>
	/// Total without any tax (VAT).
	/// </summary>
	public ulong TotalLessTax;
	
	/// <summary>
	/// The active currency.
	/// </summary>
	public string CurrencyCode;
	
	/// <summary>
	/// The contents of the cart.
	/// </summary>
	public List<LineItem> Contents;

	/// <summary>
	/// An error message if there is something wrong with this set as a whole (typically coupon related).
	/// </summary>
	public string ErrorMessage;

	/// <summary>
	/// Error code if there is something wrong with this set as a whole (typically coupon related). Check this first.
	/// If one is present, checking out will be prevented.
	/// </summary>
	public string ErrorCode;

	/// <summary>
	/// True if any of the contents are subscription based.
	/// </summary>
	public bool HasSubscriptionProducts;
}

/// <summary>
/// A particular cart item.
/// </summary>
public struct LineItem
{
	/// <summary>
	/// The original PQ.
	/// </summary>
	[JsonIgnore]
	public ProductQuantity ProductQuantity;

	/// <summary>
	/// The PQ Id.
	/// </summary>
	public uint ProductQuantityId;

	/// <summary>
	/// The original product.
	/// </summary>
	[JsonIgnore]
	public Product Product;

	/// <summary>
	/// An error message if there is something wrong with this line item.
	/// </summary>
	public string ErrorMessage;

	/// <summary>
	/// Error code if there is something wrong with this line item. Check this first.
	/// </summary>
	public string ErrorCode;

	/// <summary>
	/// The product that this is a quantity of. The product may permit unlimited usage in which case units does not need to be used.
	/// </summary>
	public uint ProductId;
	
	/// <summary>
	/// The quantity.
	/// </summary>
	public ulong Quantity;
	
	/// <summary>
	/// TotalLessTax then sent through the Tax calc, if there is one.
	/// </summary>
	public ulong Total;
	
	/// <summary>
	/// Total without any tax (VAT).
	/// </summary>
	public ulong TotalLessTax;
}