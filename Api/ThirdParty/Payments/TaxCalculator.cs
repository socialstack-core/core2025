using System;

namespace Api.Payments;


/// <summary>
/// Calculates tax for a jurisdiction.
/// </summary>
public class TaxCalculator
{
	
	/// <summary>
	/// Creates a new tax calculator from the given config.
	/// </summary>
	public TaxCalculator(TaxConfiguration config)
	{
		Multiplier = (config.Multiplier / 100d) + 1d;
	}
	
	/// <summary>
	/// Overall multiplier.
	/// </summary>
	public double Multiplier;
	
	
	/// <summary>
	/// Applies this tax calculator to the given pence/cents value.
	/// </summary>
	public uint Apply(uint amount)
	{
		// Naturally floors.
		return (uint)(amount * Multiplier);
	}
	
}