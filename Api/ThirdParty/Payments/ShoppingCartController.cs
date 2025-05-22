using Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Payments
{
    /// <summary>Handles shoppingCart endpoints.</summary>
    [Route("v1/shoppingCart")]
	public partial class ShoppingCartController : AutoController<ShoppingCart>
    {
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