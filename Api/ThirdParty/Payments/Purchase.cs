using System;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;
using Newtonsoft.Json;

namespace Api.Payments
{

	/// <summary>
	/// A purchase for a set of products which are attached as a set of ProductQuantities each with baked in prices.
	/// </summary>
	[HasVirtualField("DeliveryAddress", typeof(Addresses.Address), "DeliveryAddressId")]
	[HasVirtualField("BillingAddress", typeof(Addresses.Address), "BillingAddressId")]
	[HasVirtualField("DeliveryOption", typeof(DeliveryOption), "DeliveryOptionId")]
	public partial class Purchase : VersionedContent<uint>
	{
		/// <summary>
		/// 0 = Created, not completed.
		/// 200 = Fulfilled.
		/// 201 = BNPL ready for fulfilment.
		/// 202 = Paid and ready for fulfilment.
		/// 203 = Pending approval. Only incurred if your site specifically creates some form of approval mechanism.
		/// 101 = Started submit to gateway. If a purchase is stuck in this state you MUST check if the gateway received the request at all.
		/// 102 = Pending at payment gateway.
		/// 500 = Failed (payment gateway rejection).
		/// 400 = Failed (user sourced fault).
		/// 401 = Not approved.
		/// </summary>
		public uint Status;

		/// <summary>
		/// True if this is a BNPL purchase. 
		/// It jumps straight to status 202 (ready for fulfilment) despite being not actually paid.
		/// </summary>
		[JsonIgnore]
		public bool BuyNowPayLater;

		/// <summary>
		/// Set if a coupon was used.
		/// </summary>
		[JsonIgnore]
		public uint CouponId;

		/// <summary>
		/// Exclude tax on this purchase.
		/// </summary>
		[JsonIgnore]
		public bool ExcludeTax;

		/// <summary>
		/// The tax jurisdiction of the purchase.
		/// </summary>
		[JsonIgnore]
		public string TaxJurisdiction;

		/// <summary>
		/// True if this purchase was only authorised, not actually executed.
		/// </summary>
		public bool Authorise;

		/// <summary>
		/// True if this purchase has multiple subscriptions attached to it which fulfil when the purchase does.
		/// The subscriptions are attached via a mapping called "Subscriptions".
		/// </summary>
		public bool MultiExecute;

		/// <summary>
		/// The locale the purchase will occur in. This is used to specify the actual price paid.
		/// </summary>
		public uint LocaleId;

		/// <summary>
		/// An ID provided by the payment gateway.
		/// </summary>
		public string PaymentGatewayInternalId;

		/// <summary>
		/// The gateway ID. Stripe is gateway=1.
		/// </summary>
		public uint PaymentGatewayId;

		/// <summary>
		/// The payment method to use. This is used to specify PaymentGatewayId.
		/// </summary>
		public uint PaymentMethodId;

		/// <summary>
		/// The currency the payment is being made in.
		/// </summary>
		public string CurrencyCode;

		/// <summary>
		/// Total sum of products less any tax. If delivery is free, this equals TotalCostLessTax.
		/// </summary>
		public ulong ProductsCostLessTax;

		/// <summary>
		/// Total sum of products including any tax. If delivery is free, this equals TotalCost.
		/// </summary>
		public ulong ProductsCost;

		/// <summary>
		/// Cost excluding tax, including delivery.
		/// </summary>
		public ulong TotalCostLessTax;

		/// <summary>
		/// The total cost in the currency codes native atomic unit, inclusive of any tax and delivery.
		/// </summary>
		public ulong TotalCost;
		
		/// <summary>
		/// Delivery cost excluding tax.
		/// </summary>
		public ulong DeliveryCostLessTax;

		/// <summary>
		/// The delivery cost in the currency codes native atomic unit, inclusive of any tax.
		/// </summary>
		public ulong DeliveryCost;

		/// <summary>
		/// Present if apportionment was calculated based on the tax status of the delivered goods.
		/// </summary>
		public double? DeliveryApportionment;

		/// <summary>
		/// True if any of the products in this purchase are subs.
		/// </summary>
		[JsonIgnore]
		public bool HasSubscriptions;

		/// <summary>
		/// A field for identifying duplicate purchase requests. Used by the content type.
		/// </summary>
		[JsonIgnore]
		public ulong ContentAntiDuplication;

		/// <summary>
		/// The content type that requested the payment. E.g. "Subscription" or "ShoppingCart".
		/// </summary>
		public string ContentType;

		/// <summary>
		/// The ID of the content type that requested the payment. E.g. an ID of a partiuclar subscription.
		/// </summary>
		public uint ContentId;

		/// <summary>
		/// If delivery relevant, the delivery addr.
		/// </summary>
		public uint DeliveryAddressId;

		/// <summary>
		/// If billing relevant, the billing addr.
		/// </summary>
		public uint BillingAddressId;

		/// <summary>
		/// If delivery relevant, the delivery method.
		/// </summary>
		public uint DeliveryOptionId;
	}

}