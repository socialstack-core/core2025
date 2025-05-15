using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{
	
	/// <summary>
	/// A ProductAttributeValue
	/// </summary>
	public partial class ProductAttributeValue : VersionedContent<uint>
	{
		// Example fields. None are required:
		/*
        /// <summary>
        /// The name of the productAttributeValue
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;
		
		/// <summary>
		/// The content of this productAttributeValue.
		/// </summary>
		[Localized]
		public string BodyJson;

		/// <summary>
		/// The feature image ref
		/// </summary>
		[DatabaseField(Length = 80)]
		public string FeatureRef;
		*/
		
	}

}