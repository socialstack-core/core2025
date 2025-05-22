using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;
using Api.Startup;
using System;

namespace Api.Payments
{
	/// <summary>
	/// Handles shoppingCarts.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ShoppingCartService : AutoService<ShoppingCart>
    {
		private ProductQuantityService _productQuantities;
		private PurchaseService _purchases;
		private ProductService _products;

		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ShoppingCartService(ProductQuantityService productQuantities, PurchaseService purchases, ProductService products) : base(Events.ShoppingCart)
        {
			_productQuantities = productQuantities;
			_purchases = purchases;
			_products = products;
		}

		/// <summary>
		/// Checkout the cart using the given payment method. Intended to be called by actual user context.
		/// Returns a Purchase object which contains a clone of the objects in the cart.
		/// Will throw publicExceptions if the payment failed.
		/// You should however check the purchase.Status for immediate failures as well.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cart"></param>
		/// <param name="payment">
		/// The payment method to use. If the user does not want to save their method this can just be a "new PaymentMethod()" in memory only instance.
		/// </param>
		public async ValueTask<PurchaseAndAction> Checkout(Context context, ShoppingCart cart, PaymentMethod payment)
		{
			// Create a purchase (uses the user's locale from context):
			var purchase = await _purchases.Create(context, new Purchase() {
				ContentType = "ShoppingCart",
				ContentId = cart.Id,
				PaymentMethodId = payment.Id
			}, DataOptions.IgnorePermissions);

			// Copy the items from the shopping cart to the purchase.
			// This prevents any risk of someone manipulating their cart during the fulfilment.
			var inCart = await GetProducts(context, cart);
			await _purchases.AddProducts(context, purchase, inCart);

			// Attempt to fulfil the purchase now:
			return await _purchases.Execute(context, purchase, payment);
		}

		/// <summary>
		/// Gets the products in the given cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cart"></param>
		/// <returns></returns>
		public async ValueTask<List<ProductQuantity>> GetProducts(Context context, ShoppingCart cart)
		{
			return await _productQuantities.Where("ShoppingCartId=?", DataOptions.IgnorePermissions).Bind(cart.Id).ListAll(context);
		}

		/// <summary>
		/// Adds the given product to the cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cartId"></param>
		/// <param name="itemChanges"></param>
		/// <returns></returns>
		public async ValueTask<ShoppingCart> AddToCart(Context context, uint cartId, List<CartItemChange> itemChanges)
		{
			ShoppingCart cart;

			if (cartId == 0)
			{
				// Spawn a new cart now.
				cart = await Create(context, new ShoppingCart() { }, DataOptions.IgnorePermissions);
			}
			else
			{
				// Get the cart:
				cart = await Get(context, cartId);

				if (cart == null)
				{
					throw new PublicException("Cart was not found", "cart/not_found");
				}
			}

			foreach (var change in itemChanges)
			{
				// Get the product:
				var product = await _products.Get(context, change.ProductId);

				if (product == null)
				{
					throw new PublicException("Product '" + change.ProductId + "' was not found", "product/not_found");
				}

				await AddToCart(context, cart, product, new CartQuantity() {
					Total = change.Quantity,
					Delta = change.DeltaQuantity
				});
			}

			return cart;
		}
		
		/// <summary>
		/// Adds the given product to the cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cart"></param>
		/// <param name="productId"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public async ValueTask<ProductQuantity> AddToCart(Context context, ShoppingCart cart, uint productId, CartQuantity quantity)
		{
			// Get the product:
			var product = await _products.Get(context, productId, DataOptions.IgnorePermissions);

			if (product == null)
			{
				throw new PublicException("Product was not found", "product/not_found");
			}

			return await AddToCart(context, cart, productId, quantity);
		}

		/// <summary>
		/// Adds or removes N of the given product from the cart.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cart"></param>
		/// <param name="product"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public async ValueTask<ProductQuantity> AddToCart(Context context, ShoppingCart cart, Product product, CartQuantity quantity)
		{
			if (!quantity.Delta.HasValue && !quantity.Total.HasValue)
			{
				throw new PublicException("Please specify a quantity. It can either be a specific total quantity or a change in the quantity.", "quantity/missing");
			}

			if (cart.Status != 0)
			{
				throw new PublicException("Cannot modify a cart after it has been checked out.", "cart/closed");
			}

			// Check if this product is already in this cart:
			var pQuantity = await _productQuantities
				.Where("ProductId=? and ShoppingCartId=?", DataOptions.IgnorePermissions)
				.Bind(product.Id)
				.Bind(cart.Id)
			.First(context);

			if (pQuantity == null)
			{
				// New object.
				int currentQuantity = quantity.Delta.HasValue ? quantity.Delta.Value : (int)quantity.Total.Value;

				if (currentQuantity <= 0)
				{
					// No change.
					return null;
				}

				// Initial stock check. Note that a product can run out of stock whilst being in the cart.
				if (product.Stock != null)
				{
					if (currentQuantity > product.Stock.Value)
					{
						throw new PublicException("Unfortunately there's not enough in stock to place your order.", "stock/insufficient");
					}
				}

				// Create a new one:
				pQuantity = await _productQuantities.Create(context, new ProductQuantity()
				{
					ProductId = product.Id,
					ShoppingCartId = cart.Id,
					Quantity = (uint)currentQuantity
				}, DataOptions.IgnorePermissions);
			}
			else
			{
				// Add or subtract from the existing one.
				int newQty = quantity.Delta.HasValue ? (quantity.Delta.Value + (int)pQuantity.Quantity) : (int)quantity.Total.Value;

				if (newQty <= 0)
				{
					await _productQuantities.Delete(context, pQuantity, DataOptions.IgnorePermissions);
				}
				else
				{
					// Initial stock check. Note that a product can run out of stock whilst being in the cart.
					if (product.Stock != null)
					{
						if (newQty > product.Stock.Value)
						{
							throw new PublicException("Unfortunately there's not enough in stock to place your order.", "stock_insufficient");
						}
					}

					await _productQuantities.Update(context, pQuantity, (Context ctx, ProductQuantity toUpdate, ProductQuantity orig) =>
					{
						toUpdate.Quantity = (uint)newQty;
					});
				}
			}

			// Update the carts editedUtc such that other checkout features are aware that the cart has changed.
			await Update(context, cart, (Context ctx, ShoppingCart toUpdate, ShoppingCart orig) => {

				toUpdate.EditedUtc = DateTime.UtcNow;
				toUpdate.DeliveryOptionId = 0;

			}, DataOptions.IgnorePermissions);

			return pQuantity;
		}

	}
    
}
