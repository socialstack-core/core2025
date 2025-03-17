using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using Api.Contexts;
using Api.Eventing;

namespace Api.ContentTemplates
{
	/// <summary>
	/// Handles contentTemplates.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class ContentTemplateService : AutoService<ContentTemplate>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public ContentTemplateService() : base(Events.ContentTemplate)
        {
			// Example admin page install:
			// InstallAdminPages("ContentTemplates", "fa:fa-rocket", new string[] { "id", "name" });
		}
	}
    
}
