using Api.Database;
using System.Threading.Tasks;
using System.Collections.Generic;
using Api.Permissions;
using System.Linq;
using Api.Eventing;
using Api.Contexts;
using Api.Startup;
using Api.CanvasRenderer;
using System;
using Api.Pages;

namespace Api.NavMenus
{
	/// <summary>
	/// Handles navigation menus.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class NavMenuService : AutoService<NavMenu>
	{
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public NavMenuService() : base(Events.NavMenu)
        {
			InstallAdminPages(
				"Nav Menus", "fa:fa-map-signs", new string[] { "id", "name", "key" }
			);

			Events.Page.BeforeAdminPageInstall.AddEventListener((Context context, Page page, CanvasNode node, Type type, AdminPageType pageType) => {

				if (page == null)
				{
					return new ValueTask<Page>(page);
				}

				if (pageType == AdminPageType.Edit)
				{
					// This is likely obsoleted: favour more customised pages instead.
					node.AppendChild(
						new CanvasNode("Admin/AutoList")
						.With("contentType", "NavMenuItem")
						.With("filterField", "NavMenuId")
						.With("create", true)
						.With("searchFields", null)
						.With("filterValue", "${primary.id}")
						.With("fields", new string[] { "bodyJson" })
					);
				}

				return new ValueTask<Page>(page);
			});
		}
	}
    
}
