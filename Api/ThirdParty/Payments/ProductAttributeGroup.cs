using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{
	
	/// <summary>
	/// A ProductAttributeGroup
	/// </summary>
	public partial class ProductAttributeGroup : VersionedContent<uint>
	{
		// Example fields. None are required:
		/*
        /// <summary>
        /// The name of the productAttributeGroup
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;
		
		/// <summary>
		/// The content of this productAttributeGroup.
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