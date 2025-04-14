using Api.Contexts;
using System.Threading.Tasks;

namespace Api.Payments;

/// <summary>
/// A generic payment gateway.
/// </summary>
public class PaymentGateway
{
	/// <summary>
	/// The ID of this gateway. Stripe is gateway = 1.
	/// </summary>
	public uint Id;

	/// <summary>
	/// Request a payment to occur.
	/// </summary>
	/// <param name="purchase"></param>
	/// <param name="totalCost"></param>
	/// <param name="paymentMethod"></param>
	/// <param name="coupon"></param>
	/// <returns></returns>
	public virtual ValueTask<PurchaseAndAction> ExecutePurchase(Purchase purchase, ProductCost totalCost, PaymentMethod paymentMethod, Coupon coupon = null)
	{
		return new ValueTask<PurchaseAndAction>(new PurchaseAndAction() { });
	}

	/// <summary>
	/// Request a card authorisation for the given purchase. The purchase itself will enter the success state if the authorisation occurs.
	/// As a result this should only be used on purchases which are either free or discounted to free with a coupon.
	/// </summary>
	/// <param name="purchase"></param>
	/// <param name="totalCost"></param>
	/// <param name="paymentMethod"></param>
	/// <param name="coupon"></param>
	/// <returns></returns>
	public virtual ValueTask<PurchaseAndAction> AuthorisePurchase(Purchase purchase, ProductCost totalCost, PaymentMethod paymentMethod, Coupon coupon = null)
	{
		return new ValueTask<PurchaseAndAction>(new PurchaseAndAction() { });
	}

	/// <summary>
	/// Checks if a token is valid and if so, returns its info. *Do not* send this full information to the frontend. It is API only.
	/// You *may* send the Name and ExpiryUtc fields only.
	/// </summary>
	/// <param name="gatewayToken"></param>
	/// <returns></returns>
	public virtual ValueTask<TokenInformation> GetTokenDetails(string gatewayToken)
	{
		return new ValueTask<TokenInformation>(new TokenInformation() { Valid = false });
	}

	/// <summary>
	/// Prepares the given token. This can be used to, for example, convert a single use token into a multi-use one depending on the gateway.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="gatewayToken"></param>
	/// <returns></returns>
	public virtual ValueTask<string> PrepareToken(Context context, string gatewayToken)
	{
		return new ValueTask<string>(gatewayToken);
	}
}