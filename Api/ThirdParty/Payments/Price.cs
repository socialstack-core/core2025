using System;
using Api.AutoForms;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{

	/// <summary>
	/// A Price
	/// </summary>
	[ListAs("PriceTiers", Explicit = true)]
	[ImplicitFor("PriceTiers", typeof(Product))]
	public partial class Price : VersionedContent<uint>
	{
		/// <summary>
		/// The amount in the target currency.
		/// </summary>
		[Data("help", "A whole number in the smallest unit of the currency (pence/ cents).")]
		public Localized<uint> Amount;
		
		/// <summary>
		/// Min quantity required for this price.
		/// </summary>
		public uint MinimumQuantity;
	}

}