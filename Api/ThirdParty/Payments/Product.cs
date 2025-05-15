using System;
using Api.AutoForms;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;

namespace Api.Payments
{
	
	/// <summary>
	/// A Product
	/// </summary>

	[HasVirtualField("Price", typeof(Price), "PriceId")]

    [ListAs("Tiers")]
	[ImplicitFor("Tiers", typeof(Product))]

    [ListAs("OptionalExtras", IsPrimary = false)]
    [ImplicitFor("OptionalExtras", typeof(Product))]

    [ListAs("Accessories", IsPrimary = false)]
    [ImplicitFor("Accessories", typeof(Product))]

    [ListAs("Suggestions", IsPrimary = false)]
    [ImplicitFor("Suggestions", typeof(Product))]

    public partial class Product : VersionedContent<uint>
	{
        /// <summary>
        /// The unique identifier for product
        /// </summary>
        [DatabaseField(Length = 200)]
        public string Sku;

        /// <summary>
        /// The name of the product
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;

        /// <summary>
        /// The slug for product
        /// </summary>
        [DatabaseField(Length = 1000)]
        public string Slug;

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
		/// Usually used by tiered products. This is the minimum purchase quantity of this product.
		/// </summary>
		public ulong MinQuantity;

		/// <summary>
		/// The content of this product.
		/// </summary>
		[Localized]
        [Data("type", "canvas")]
        [Data("main", "false")]
        public string DescriptionJson;

		/// <summary>
		/// The feature image ref
		/// </summary>
		[DatabaseField(Length = 300)]
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
		/// The price to use.
		/// </summary>
		[Localized]
		public uint PriceId;

		/// <summary>
		/// Available stock. Null indicates it is unlimited.
		/// </summary>
		public uint? Stock;

		/// <summary>
		/// Indicates if this is a variant product related to a parent base product
		/// </summary>
		public uint VariantOfId;
		
		/// <summary>
		/// Indicates if this is a tiered product related to a parent base product
		/// </summary>
		public uint TierOfId;
	}

}
