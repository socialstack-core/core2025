using Microsoft.AspNetCore.Mvc;

namespace Api.Regions
{
    /// <summary>Handles region endpoints.</summary>
    [Route("v1/region")]
	public partial class RegionController : AutoController<Region>
    {
    }
}