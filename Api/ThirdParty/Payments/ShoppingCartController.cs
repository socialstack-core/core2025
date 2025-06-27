using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Payments
{
    /// <summary>Handles shoppingCart endpoints.</summary>
    [Route("v1/shoppingCart")]
	public partial class ShoppingCartController : AutoController<ShoppingCart>
    {

		/// <summary>
		/// Applies a coupon to the shopping cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="couponInfo"></param>
		/// <returns></returns>
		[HttpPost("apply_coupon")]
        public async ValueTask<ShoppingCart> ApplyCoupon(Context context, [FromBody] CartCoupon couponInfo)
        {
			return await (_service as ShoppingCartService)
				.ApplyCoupon(context,
					couponInfo.ShoppingCartId,
					couponInfo.Code
				);
		}

		/// <summary>
		/// Removes a coupon from the shopping cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="couponInfo"></param>
		/// <returns></returns>
		[HttpPost("remove_coupon")]
		public async ValueTask<ShoppingCart> RemoveCoupon(Context context, [FromBody] RemoveCoupon couponInfo)
		{
			return await (_service as ShoppingCartService)
				.RemoveCoupon(context,
					couponInfo.ShoppingCartId
				);
		}

		/// <summary>
		/// Adds or removes items from the specified cart. The contextual user must have access to the cart.
		/// If the cart ID is zero, a new cart will be spawned and returned.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="itemChanges"></param>
		/// <returns></returns>
		[HttpPost("change_items")]
        public async ValueTask<ShoppingCart> ChangeItems(Context context, [FromBody] CartItemChanges itemChanges)
        {
            return await (_service as ShoppingCartService)
                .AddToCart(context,
					itemChanges.ShoppingCartId,
                    itemChanges.Items
                );
        }

		/// <summary>
		/// Checkout the cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="checkout"></param>
		/// <returns></returns>
		[HttpPost("checkout")]
		public async ValueTask<PurchaseAndAction?> Checkout(Context context, [FromBody] CheckoutInfo checkout)
		{
			var cart = await (_service as ShoppingCartService).Get(context, checkout.ShoppingCartId);

			if (cart == null)
			{
				return null;
			}

			return await (_service as ShoppingCartService)
				.Checkout(
					context,
					cart,
					checkout.PaymentMethod
				);
		}

	}

	/// <summary>
	/// Checking out a cart.
	/// </summary>
	public struct CheckoutInfo
	{
		/// <summary>
		/// The cart ID.
		/// </summary>
		public uint ShoppingCartId;

		/// <summary>
		/// Delivery address ID if necessary.
		/// </summary>
		public uint DeliveryAddressId;

		/// <summary>
		/// Billing address if necessary.
		/// </summary>
		public uint BillingAddressId;

		/// <summary>
		/// If using a saved payment method, the ID of it or the details for a one off payment use.
		/// This can be null if it's a free or buy now pay later order. 
		/// Buy now pay later orders must be available to the user.
		/// </summary>
		public JToken PaymentMethod;
	}

	/// <summary>
	/// Represents quantity or quantity change in a cart. Only one or the other - not both.
	/// </summary>
	public struct CartQuantity
    {
		/// <summary>
		/// The delta quantity. Use this or Total.
		/// </summary>
		public int? Delta;

		/// <summary>
		/// The total desired quantity. Use this or Delta.
		/// </summary>
		public uint? Total;
	}

    /// <summary>
    /// A set of items to change within a cart.
    /// </summary>
    public struct CartItemChanges
    {
		/// <summary>
		/// The shopping cart, which can be zero if you need a new cart object.
		/// </summary>
		public uint ShoppingCartId;

		/// <summary>
		/// Items to add/ remove.
		/// </summary>
		public List<CartItemChange> Items;
    }
    
    /// <summary>
    /// Changing the coupon on a cart.
    /// </summary>
    public struct RemoveCoupon
	{
		/// <summary>
		/// The shopping cart
		/// </summary>
		public uint ShoppingCartId;
	}
	
    /// <summary>
    /// Changing the coupon on a cart.
    /// </summary>
    public struct CartCoupon
	{
		/// <summary>
		/// The coupon to apply. If this is null, nothing happens.
		/// </summary>
		public string Code;

		/// <summary>
		/// The shopping cart
		/// </summary>
		public uint ShoppingCartId;
	}

	/// <summary>
	/// Changing (usually an addition) the quantity of an item in a cart.
	/// </summary>
	public struct CartItemChange
    {   
        /// <summary>
        /// The product to change.
        /// </summary>
        public uint ProductId;

		/// <summary>
		/// The delta quantity. Use this or Quantity.
		/// </summary>
		public int? DeltaQuantity;

        /// <summary>
        /// The total desired quantity. Use this or DeltaQuantity.
        /// </summary>
        public uint? Quantity;
    }
}