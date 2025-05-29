using System;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{

	/// <summary>
	/// A ProductAttributeValue
	/// </summary>
	[HasVirtualField("attribute", typeof(ProductAttribute), "ProductAttributeId")]
	[ListAs("attributes", Explicit = true)]
	[ImplicitFor("attributes", typeof(Product))]
	public partial class ProductAttributeValue : VersionedContent<uint>
	{
		/// <summary>
		/// The attribute that this is a value for.
		/// </summary>
		public uint ProductAttributeId;

		/// <summary>
		/// The raw, unitless value.
		/// </summary>
		public string Value;

		/// <summary>
		/// The feature image ref to allow for colour swatches etc 
		/// </summary>
		[DatabaseField(Length = 300)]
		public string FeatureRef;

	}

}