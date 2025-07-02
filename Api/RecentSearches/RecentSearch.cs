using System;
using Api.Database;
using Api.Payments;
using Api.Startup;
using Api.Translate;
using Api.Users;


namespace Api.RecentSearches
{
	
	/// <summary>
	/// A RecentSearch
	/// </summary>
	[HasVirtualField("Product", typeof(Product), "ProductId")]
	public partial class RecentSearch : UserCreatedContent<uint>
	{
		/// <summary>
		/// Reference to the product ID
		/// </summary>
		public uint ProductId;
	}

}