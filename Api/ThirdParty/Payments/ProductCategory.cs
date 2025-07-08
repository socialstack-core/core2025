using Api.AutoForms;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;

namespace Api.Payments
{
	
	/// <summary>
	/// A ProductCategory
	/// </summary>
	 
	// A product category can have a single parent
	[HasVirtualField("ProductCategory", typeof(ProductCategory), "ParentId")]

    // products can be linked to one or more categories (normally the lowest child)
    [ListAs("ProductCategories")]
    [ImplicitFor("ProductCategories", typeof(Product))]

    public partial class ProductCategory : VersionedContent<uint>
	{
        /// <summary>
        /// The name of the product category
        /// </summary>
        [DatabaseField(Length = 200)]
        [Data("required", true)]
        [Data("validate", "Required")]
		public Localized<string> Name;

        /// <summary>
        /// The slug for product category
        /// </summary>
        [DatabaseField(Length = 1000)]
		[Data("readonly", true)]
		public string Slug;

        /// <summary>
        /// The description of this product category.
        /// </summary>
        [DatabaseField(Length = 200)]
        [Data("type", "canvas")]
        [Data("main", "false")]
        [Data("required", true)]
        [Data("validate", "CanvasRequired")]
        public Localized<string> Description;

        /// <summary>
        /// The target audience for the product category (0 = public web/1 = internal/2 = dev)
        /// </summary>
        [Module("Admin/Payments/TargetAudiences")]
        public uint TargetAudience;

        /// <summary>
        /// The category image ref
        /// </summary>
        [DatabaseField(Length = 300)]
        [Data("required", true)]
        [Data("validate", "Required")]
        public string FeatureRef;

        /// <summary>
        /// Optional icon to show with this item.
        /// </summary>
        [DatabaseField(Length = 300)]
        [Data("type", "icon")]
        [Data("required", true)]
        [Data("validate", "Required")]
        public string IconRef;

        /// <summary>
        /// The link to the parent of this product category 
        /// </summary>
        /// 
        [Module("Admin/ContentSelect")]
        [Data("contentType", "ProductCategory")]
        [Data("label", "Parent Product Category")]
        public uint? ParentId;
		
	}

}