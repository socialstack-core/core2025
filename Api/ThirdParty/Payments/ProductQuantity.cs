using System;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{
	
	/// <summary>
	/// Tracks a product and a quantity of the product.
	/// </summary>
	[ListAs("ProductQuantities")]
	[ImplicitFor("ProductQuantities", typeof(ShoppingCart))]
	[ImplicitFor("ProductQuantities", typeof(Subscription))]
	[ImplicitFor("ProductQuantities", typeof(Purchase))]
	[HasVirtualField("Product", typeof(Product), "ProductId")]
	public partial class ProductQuantity : VersionedContent<uint>
	{
		/// <summary>
		/// The product that this is a quantity of. The product may permit unlimited usage in which case units does not need to be used.
		/// </summary>
		public uint ProductId;
		
		/// <summary>
		/// The quantity.
		/// </summary>
		public ulong Quantity;
	}

}