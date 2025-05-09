using System;
using Api.AutoForms;
using Api.Database;
using Api.Translate;
using Api.Users;

namespace Api.Pages
{
	
	/// <summary>
	/// A page. Pages are accessed via associated permalink(s).
	/// </summary>
	public partial class Page : VersionedContent<uint>
	{
		/// <summary>
		/// The default title for this page.
		/// </summary>
		[Localized]
		public string Title;

		/// <summary>
		/// A key of the form e.g. "admin_user_list" which is used to keep track of 
		/// generated pages, enabling the URL to be edited without causing a page to regenerate.
		/// A page taking on the role of primary content for a given type has a key set to e.g. "primary:user".
		/// If it is the primary page for a specific piece of content, then it is e.g. "primary:product:42".
		/// Primary keys on the admin panel are prefixed with "admin_".
		/// </summary>
		public string Key;

		/// <summary>
		/// The pages content (as canvas JSON).
		/// </summary>
		[Localized]
		[Data("groups", "*")]
		[DatabaseField(Length = 9000000)]
		public string BodyJson;

		/// <summary>
		/// The default description for this page.
		/// </summary>
		[Localized]
		public string Description;

		/// <summary>
		/// Prevent this page from being indexed by search crawlers.
		/// </summary>
		[Data("hint", "Prevent search crawlers from indexing this page")]
		public bool NoIndex;

		/// <summary>
		/// Prevent links on this page from being followed by search crawlers.
		/// </summary>
		[Data("hint", "Prevent search crawlers from following links on this page")]
		public bool NoFollow;

		/// <summary>
		/// A disambiguation mechanism when the permission system returns multiple pages.
		/// Typically happens on the homepage.
		/// </summary>
		public bool PreferIfLoggedIn;
	}
	
}