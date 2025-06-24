namespace Api.Translate;


public partial class Locale
{
	
	/// <summary>
	/// The default tax jurisdiction when this locale is in use.
	/// Often ISO 3166-1 alpha 2, unless you do know the 3166-2 code for a particular US state for example.
	/// </summary>
	public string DefaultTaxJurisdiction;
	
}