using Microsoft.AspNetCore.Mvc;

namespace Api.ContentTemplates
{
    /// <summary>Handles contentTemplate endpoints.</summary>
    [Route("v1/contentTemplate")]
	public partial class ContentTemplateController : AutoController<ContentTemplate>
    {
    }
}