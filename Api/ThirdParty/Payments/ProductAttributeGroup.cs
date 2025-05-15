using System;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;


namespace Api.Payments
{

	/// <summary>
	/// A group of product attributes. For example, "Dimensions".
	/// Can be nested in a tree, and a group can be added to multiple parents.
	/// </summary>
	[ListAs("AttributeGroups", Explicit = true)]
	[ImplicitFor("AttributeGroups", typeof(ProductAttribute))]

	[ListAs("ChildGroups", Explicit = true, IsPrimary = false)]
	[ImplicitFor("ChildGroups", typeof(ProductAttributeGroup))]
	public partial class ProductAttributeGroup : VersionedContent<uint>
	{
        /// <summary>
        /// The name of the attribute group e.g. "Dimensions".
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;
	}

}