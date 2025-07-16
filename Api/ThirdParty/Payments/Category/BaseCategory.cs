using Api.AutoForms;
using Api.Database;
using Api.Translate;
using Api.Users;

namespace Api.Payments
{
    ///summary
    ///Base class for content categories.
    /// </summary>
    public abstract partial class BaseCategory : VersionedContent<uint>
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
        /// The optional parent id
        /// </summary>
        public uint? ParentId;
    }
}
