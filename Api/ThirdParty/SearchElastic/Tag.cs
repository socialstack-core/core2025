using Api.AutoForms;

namespace Api.Tags
{
    public partial class Tag
    {
        /// <summary>
        /// The sequence in which to list/display the tag
        /// </summary>
        [Data("hint", "The sequence in which to list/display the tag")]
        public int Order;

        /// <summary>
        /// Should this tag be retrieved via currency locale?
        /// </summary>
        public bool IsPrice;
    }
}