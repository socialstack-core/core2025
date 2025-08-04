using Api.Addresses;
using Api.Payments;
using Api.Permissions;
using System.Collections.Generic;

namespace Api.Eventing
{
	/// <summary>
	/// Events are instanced automatically. 
	/// You can however specify a custom type or instance them yourself if you'd like to do so.
	/// </summary>
	public partial class Events
	{
        /// <summary>
        /// Set of events for a subscriptionUsage.
        /// </summary>
        public static EventGroup<SubscriptionUsage> SubscriptionUsage;
	
		/// <summary>
		/// Set of events for a coupon.
		/// </summary>
		public static EventGroup<Coupon> Coupon;

        /// <summary>
        /// Set of events for a productCategory.
        /// </summary>
        public static EventGroup<ProductCategory> ProductCategory;

		/// <summary>
		/// Set of events for a product attribute.
		/// </summary>
		public static EventGroup<ProductAttribute> ProductAttribute;
		
		/// <summary>
		/// Set of events for a product template.
		/// </summary>
		public static EventGroup<ProductTemplate> ProductTemplate;
		
		/// <summary>
		/// Set of events for a product attribute group.
		/// </summary>
		public static EventGroup<ProductAttributeGroup> ProductAttributeGroup;
		
		/// <summary>
		/// Set of events for a product attribute value.
		/// </summary>
		public static EventGroup<ProductAttributeValue> ProductAttributeValue;
		
		/// <summary>
		/// Set of events for a purchase.
		/// </summary>
		public static PurchaseEventGroup Purchase;
		
		/// <summary>
		/// Set of events for a price.
		/// </summary>
		public static EventGroup<Price> Price;

		/// <summary>
		/// Set of events for a shoppingCart.
		/// </summary>
		public static EventGroup<ShoppingCart> ShoppingCart;

		/// <summary>
		/// Set of events for a productQuantity.
		/// </summary>
		public static ProductQuantityEventGroup ProductQuantity;
		
		/// <summary>
		/// Set of events for a productQuantity.
		/// </summary>
		public static DeliveryOptionEventGroup DeliveryOption;
		
		/// <summary>
		/// Set of events for a paymentMethod.
		/// </summary>
		public static PaymentMethodEventGroup PaymentMethod;

		/// <summary>
		/// Set of events for a subscription.
		/// </summary>
		public static SubscriptionEventGroup Subscription;
		
		/// <summary>
		/// Set of events for a product.
		/// </summary>
		public static ProductEventGroup Product;
	}

	/// <summary>
	/// Specialised event group for the PaymentMethod event type.
	/// </summary>
	public partial class PaymentMethodEventGroup : EventGroup<PaymentMethod>
	{

		/// <summary>
		/// Called when checking if BNPL is available to the given context.
		/// </summary>
		public EventHandler<BuyNowPayLater> AuthoriseBuyNowPayLater;

	}
	
	/// <summary>
	/// Specialised event group for the DeliveryOption event type.
	/// </summary>
	public partial class DeliveryOptionEventGroup : EventGroup<DeliveryOption>
	{

		/// <summary>
		/// Called when collecting delivery estimates.
		/// </summary>
		public EventHandler<DeliveryEstimates> Estimate;

	}
	
	/// <summary>
	/// Specialised event group for the Product event type.
	/// </summary>
	public partial class ProductEventGroup : EventGroup<Product>
	{

		/// <summary>
		/// Called when running a search for products.
		/// </summary>
		public EventHandler<ProductSearch> Search;

		/// <summary>
		/// Called when pricing for a product is being established.
		/// </summary>
		public EventHandler<List<Price>, Product> Pricing;

	}

	/// <summary>
	/// Specialised event group for the Purchase type in order to add additional events.
	/// As usual, instanced automatically by the event handler engine.
	/// </summary>
	public partial class PurchaseEventGroup : EventGroup<Purchase>
	{

		/// <summary>
		/// Called just before a purchase is executed. This is the final opportunity to modify its billable items.
		/// </summary>
		public EventHandler<Purchase> BeforeExecute;

		/// <summary>
		/// Called during checkout. Map any custom fields from a checkout submission to the purchase here.
		/// </summary>
		public EventHandler<Purchase, CheckoutInfo> Checkout;

	}

	/// <summary>
	/// Specialised event group for the Subscription type in order to add additional events.
	/// As usual, instanced automatically by the event handler engine.
	/// </summary>
	public partial class SubscriptionEventGroup : EventGroup<Subscription>
	{

		/// <summary>
		/// Called just before the daily process is started.
		/// Can be used to prevent it from doing anything if you know the supporting data is not ready yet.
		/// </summary>
		public EventHandler<DailySubscriptionMeta> BeforeBeginDailyProcess;

	}

	/// <summary>
	/// Specialised event group for the ProductQuantity type in order to add additional events.
	/// As usual, instanced automatically by the event handler engine.
	/// </summary>
	public partial class ProductQuantityEventGroup : EventGroup<ProductQuantity>
	{

	}

	/// <summary>
	/// Event group for Addresses.
	/// </summary>
	public partial class AddressEventGroup : EventGroup<Address>
	{

		/// <summary>
		/// Called when loading up the filter for the cart address book.
		/// </summary>
		public EventHandler<Filter<Address, uint>> GetCartAddressFilter;

	}
}