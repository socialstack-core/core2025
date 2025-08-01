namespace Api.Payments;


public struct DeliveryPricingInfo
{
	public double TaxApportion;
	public ulong ValueUntaxed;
	public ulong ValueTaxed;
}