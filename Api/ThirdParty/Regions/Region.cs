using System;
using System.Collections.Generic;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Regions
{
	
	/// <summary>
	/// A Region
	/// </summary>
	public partial class Region : VersionedContent<uint>
	{
		/// <summary>
		/// The name of the region, examples include "header", "footer" etc...
		/// </summary>
		public string Name;

		/// <summary>
		/// The components that exist within the region
		/// </summary>
		public string ComponentsJson;
	}
}