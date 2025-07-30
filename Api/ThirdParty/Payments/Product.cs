using System;
using Api.AutoForms;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;
using Newtonsoft.Json;

namespace Api.Payments
{
	
	/// <summary>
	/// A Product. Price is specified via price tiers as a product can have bulk discounts.
	/// Typically though you would include calculatedPrice as that resolves both customer 
	/// specific pricing and provides with/without tax results.
	/// </summary>

    [ListAs("OptionalExtras", IsPrimary = false)]
    [ImplicitFor("OptionalExtras", typeof(Product))]

    [ListAs("Accessories", IsPrimary = false)]
    [ImplicitFor("Accessories", typeof(Product))]

    [ListAs("Suggestions", IsPrimary = false)]
    [ImplicitFor("Suggestions", typeof(Product))]

    [ListAs("Variants", IsPrimary = false)]
    [ImplicitFor("Variants", typeof(Product))]

    [HasVirtualField("primaryCategory", typeof(ProductCategory), "PrimaryCategoryId")]

	[HasSecondaryResult("attributeValueFacets", typeof(AttributeValueFacet))]
	[HasSecondaryResult("productCategoryFacets", typeof(ProductCategoryFacet))]
	public partial class Product : VersionedContent<uint>
	{
        /// <summary>
        /// The unique identifier for product
        /// </summary>
        [DatabaseField(Length = 200)]
        public string Sku;

		/// <summary>
		/// 0 = Physical
		/// 1 = Digital (no delivery method required)
		/// </summary>
		[Module("Admin/Payments/ProductTypes")]
		public uint ProductType;

        /// <summary>
        /// The name of the product
        /// </summary>
        [DatabaseField(Length = 200)]
        [Data("required", "true")]
        [Data("validate", "Required")]
		public Localized<string> Name;

        /// <summary>
        /// The slug for product
        /// </summary>
        [DatabaseField(Length = 1000)]
		[Data("readonly", true)]
        public string Slug;

		/// <summary>
		/// In the atomic currency unit (pence), the nominal value of free samples used for tax purposes.
		/// This must be set if the configured price is zero. It is not triggered in the event 
		/// that an order's value is discounted to zero through coupons or other promotions.
		/// </summary>
		public uint? FreeSampleNominalValue;

        /// <summary>
        /// True if this product is billed by usage.
        /// </summary>
        [Data("help", "Tick this if this product is billed after it has been used based on the amount of usage it has had.")]
		public bool IsBilledByUsage;
		
		/// <summary>
		/// Used to indicate if this product recurs and if so, the frequency.
		/// 0 = One off
		/// 1 = Weekly
		/// 2 = Monthly
		/// 3 = Quarterly
		/// 4 = Yearly
		/// </summary>
		[Module("Admin/Payments/BillingFrequencies")]
		public uint BillingFrequency;

        /// <summary>
        /// Used to indicate if this product is tax exempt
        /// 0 = No
        /// 1 = Yes
        /// 2 = Eligibility required
        /// </summary>
        [Module("Admin/Payments/TaxExempt")]
        [Data("help", "Identify if the product is tax exempt")]
        public uint TaxExempt;

        /// <summary>
        /// Used to indicate the availability of the product
        /// 0 = Yes
        /// 1 = Pre order
        /// 2 = No (permanently)
        /// 3 = No (awaiting stock)
        /// </summary>
        [Module("Admin/Payments/Availability")]
        [Data("help", "Identify if the product is currently available")]
        public uint Availability;

        /// <summary>
		/// The content of this product.
		/// </summary>
		[Data("type", "canvas")]
        [Data("main", "false")]
        public Localized<JsonString> DescriptionJson;

        /// <summary>
        /// The raw metadata/content of this product, used for free text search
        /// </summary>
        [Data("readonly", true)]
		[JsonIgnore]
        public string DescriptionRaw;

		/// <summary>
		/// The feature image ref
		/// </summary>
		[DatabaseField(Length = 300)]
		[Data("required", "true")]
		[Data("validate", "Required")]
		public string FeatureRef;

		/// <summary>
		/// Indicates how the price is computed based on the quantity of the product. See "Pricing strategy" on the wiki for more info.
		/// 0 = Standard strategy. Select tier (or base product) based on minQuantity, then perform quantity * tier.
		/// 1 = Step once strategy. maxQuantity * tierPrice for base tier, then standard.
		/// 2 = Step always. maxQuantity * tierPrice per tier always.
		/// </summary>
		[Module("Admin/Payments/PriceStrategies")]
        [Data("help", "If you're using tiers, this defines the calculation used for the final price.")]
        public uint PriceStrategy;

		/// <summary>
		/// Available stock. Null indicates it is unlimited.
		/// </summary>
		public uint? Stock;

		/// <summary>
		/// Indicates if this is a variant product related to a parent base product
		/// </summary>
		public uint? VariantOfId;
		
		/// <summary>
		/// Continue selling when there is no stock
		/// usually useful for products that can be
		/// back-ordered. It's false by default.
		/// </summary>
		[Data("help", "Can this continue to be fulfilled even when there is no physical stock?")]
		public bool ContinueSellingWithNoStock = false;

		/// <summary>
		/// The ID of the primary product category. This is just a convenience field for 
		/// being the equiv of the first mapping entry, and exists to make it easily includable.
		/// </summary>
		[JsonIgnore]
		public uint PrimaryCategoryId => (uint)Mappings.GetFirst("ProductCategories");
	}

}
