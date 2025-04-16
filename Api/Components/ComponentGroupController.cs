using Microsoft.AspNetCore.Mvc;

namespace Api.Components
{
    /// <summary>Handles componentGroup endpoints.</summary>
    [Route("v1/componentGroup")]
	public partial class ComponentGroupController : AutoController<ComponentGroup>
    {
    }
}