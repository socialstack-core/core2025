using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Pages
{

	/// <summary>
	/// A Permalink. The most recent permalink for a given target URL is canonical.
	/// If there is no such source URL, the target is canonical.
	/// Unlike a redirect, a permalink is primarily a URL rewrite. Their purpose is to allow very complex URLs
	/// for e.g. products with deeply nested categories, whilst not breaking the URL when any of the products change.
	/// Instead older permalinks just become redirects to the latest, canonical one.
	/// Has an index which blocks creation of duplicates at the cluster level.
	/// </summary>
	[DatabaseIndex(Fields = new string[]{ "Url", "Target" }, Direction = "ASC", Unique = true)]
	public partial class Permalink : VersionedContent<uint>
	{
		/// <summary>
		/// The source URL. Always an absolute path ("/hello-world")
		/// </summary>
		public string Url;

		/// <summary>
		/// The target URL. Always an absolute path ("/hello-world")
		/// </summary>
		public string Target;
	}

	/// <summary>
	/// Just a URL and target for a permalink.
	/// </summary>
	public struct PermalinkUrlTarget
	{
		/// <summary>
		/// The source URL. Always an absolute path ("/hello-world")
		/// </summary>
		public string Url;

		/// <summary>
		/// The target URL. Always an absolute path ("/hello-world")
		/// </summary>
		public string Target;
	}

}