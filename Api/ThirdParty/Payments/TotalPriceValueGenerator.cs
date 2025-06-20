using Api.Database;
using Api.Startup;
using System.Threading.Tasks;
using System;
using Api.SocketServerLibrary;
using Api.Contexts;
namespace Api.Payments;


/// <summary>
/// Pairing of price and currency.
/// </summary>
public struct PriceCurrency
{
	/// <summary>
	/// E.g. "GBP".
	/// </summary>
	public string CurrencyCode;
	
	/// <summary>
	/// The total in the currency smallest atomic unit (pence/ cents etc)
	/// </summary>
	public uint Amount;
}

/// <summary>
/// A virtual field value generator for a field called "totalPrice". It can only be used on ProductQuantity.
/// 
/// Automatically instanced and the include field name is derived from the class name by the includes system. See VirtualFieldValueGenerator for more info.
/// </summary>
public partial class TotalPriceValueGenerator<T, ID> : VirtualFieldValueGenerator<T, ID>
	where T : Content<ID>, new()
	where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
{
	private PriceService _priceService;
	private ProductService _productService;
	
	/// <summary>
	/// Generate the value.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="forObject"></param>
	/// <param name="writer"></param>
	/// <returns></returns>
	public override async ValueTask GetValue(Context context, T forObject, Writer writer)
	{
		if(typeof(T) == typeof(ProductQuantity))
		{
			var pq = (ProductQuantity)((object)forObject);
			var price = await GetValueForQuantity(context, pq, writer);
			
			// Falls through to the null otherwise.
			if(price.HasValue){
				writer.WriteASCII("{\"currencyCode\":");
				writer.WriteEscaped(price.Value.CurrencyCode);
				writer.WriteASCII(",\"amount\":");
				writer.WriteS(price.Value.Amount);
				writer.Write((byte)'}');
				return;
			}
		}
		
		writer.WriteASCII("null");
	}
	
	/// <summary>
	/// Gets a total for a given productQuantity.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="pq"></param>
	/// <param name="writer"></param>
	/// <returns></returns>
	public async ValueTask<PriceCurrency?> GetValueForQuantity(Context context, ProductQuantity pq, Writer writer)
	{
		// Get the product:
		if(_productService == null)
		{
			_productService = Services.Get<ProductService>();
		}
		
		if(pq == null || pq.ProductId == 0)
		{
			return null;
		}
		
		// (comes from the cache, resolves instantly)
		var product = await _productService.Get(context, pq.ProductId, DataOptions.IgnorePermissions);
		
		if(product == null)
		{
			return null;
		}
		
		// And get the price:
		if(_priceService == null)
		{
			_priceService = Services.Get<PriceService>();
		}
		
		// (comes from the cache, resolves instantly)
		var price = await _priceService.Get(context, product.PriceId.Get(context), DataOptions.IgnorePermissions);
		
		if(price == null)
		{
			return null;
		}
		
		return new PriceCurrency()
		{
			Amount = (uint)(pq.Quantity * price.Amount),
			CurrencyCode = price.CurrencyCode
		};
	}
	
	/// <summary>
	/// The type, if any, associated with the value being outputted.
	/// For example, if GetValue outputs only strings, this is typeof(string).
	/// </summary>
	/// <returns></returns>
	public override Type OutputType => typeof(PriceCurrency);
}