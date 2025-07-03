using System;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;
using Newtonsoft.Json;


namespace Api.Payments
{

	/// <summary>
	/// A ShoppingCart contains a list of productQuantities.
	/// A user has a "current" shopping cart associated to them, and when they checkout, the shopping cart is converted to a payment.
	/// Each time objects in the cart are changed, the editedUtc of the cart itself *must* change too.
	/// </summary>
	[HasVirtualField("coupon", typeof(Coupon), "CouponId")]
	public partial class ShoppingCart : VersionedContent<uint>
	{
		/// <summary>
		/// A shopping cart in the "pending payment" or "payment completed" state is immutable.
		/// It could, however, be cloned. This is if someone wants to buy the same thing again for example.
		/// </summary>
		public uint Status;

		/// <summary>
		/// The cart key used to ensure an anon user can actually update/ load this cart.
		/// </summary>
		[DatabaseField(Length = 20)]
		public string AnonymousCartKey;

		/// <summary>
		/// The target delivery address.
		/// </summary>
		public uint AddressId;

		/// <summary>
		/// The currently selected delivery option ID, if 
		/// this cart contains any physical delivery items and one is selected.
		/// </summary>
		public uint DeliveryOptionId;

		/// <summary>
		/// An applied coupon, if any. Not directly settable but can be included.
		/// </summary>
		public uint CouponId;

		/// <summary>
		/// The established tax jurisdiction for this shopping cart.
		/// If a delivery address is unknown then the users locale is used as an initial value.
		/// </summary>
		[JsonIgnore]
		public string TaxJurisdiction;
	}

}