using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{
	
	/// <summary>
	/// A ProductAttribute
	/// </summary>
	public partial class ProductAttribute : VersionedContent<uint>
	{
        /// <summary>
        /// The name of the product attribute
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;
		
		/// <summary>
		/// The field type. 1=long, 2=double, 3=string, 4=image ref, 5=video ref, 6=file ref. 
		/// Don't store prices in attributes. You should instead create multiple product 
		/// variants and each one has its own potentially localised price.
		/// </summary>
		public int ProductAttributeType;
		
	}

}