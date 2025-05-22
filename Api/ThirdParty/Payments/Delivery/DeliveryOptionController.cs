using Api.Contexts;
using Api.Startup;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Payments;

/// <summary>Handles delivery option endpoints.</summary>
[Route("v1/deliveryoption")]
public partial class DeliveryOptionController : AutoController<DeliveryOption>
{
	private DeliveryService _deliveries;
	private DeliveryOptionService _options;
	private ShoppingCartService _carts;

	/// <summary>
	/// Instanced automatically (singleton).
	/// </summary>
	/// <param name="deliveries"></param>
	/// <param name="carts"></param>
	/// <param name="options"></param>
	public DeliveryOptionController(DeliveryService deliveries, ShoppingCartService carts, DeliveryOptionService options)
	{
		_deliveries = deliveries;
		_carts = carts;
		_options = options;
	}

	/// <summary>
	/// Estimate the given shopping cart. The contextual user must be able to load the cart of course!
	/// If estimates are already generated, this will return the existing set.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="shoppingCartId"></param>
	/// <returns></returns>
	[HttpGet("estimate/cart/{shoppingCartId}")]
	public async ValueTask<ContentStream<DeliveryOption, uint>> Estimate(Context context, [FromRoute] uint shoppingCartId)
	{
		var cart = await _carts.Get(context, shoppingCartId);
		var estimates = await _deliveries.EstimateDelivery(context, cart);

		return new ContentStream<DeliveryOption, uint>(estimates, _options);
	}

}