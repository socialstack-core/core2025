using Microsoft.AspNetCore.Mvc;

namespace Api.RecentSearches
{
    /// <summary>Handles recentSearch endpoints.</summary>
    [Route("v1/recentSearch")]
	public partial class RecentSearchController : AutoController<RecentSearch>
    {
    }
}