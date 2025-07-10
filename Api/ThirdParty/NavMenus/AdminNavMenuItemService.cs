using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Api.AvailableEndpoints;
using Api.Eventing;
using Api.Contexts;
using Api.Pages;
using Api.Permissions;
using Api.Startup;

namespace Api.NavMenus
{
	/// <summary>
	/// Handles navigation menu items.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class AdminNavMenuItemService : AutoService<AdminNavMenuItem>
	{
		// <summary>
		// Cache to avoid repeated expensive lookups of content types by key.
		// Key is case-insensitive to be resilient to case variations in page keys.
		// </summary>
		// private readonly ConcurrentDictionary<string, Type> _keyContentTypes = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Constructor injecting dependencies.
		/// Calls base constructor with appropriate event topic.
		/// Calls example admin page install to bootstrap initial state.
		/// </summary>
		public AdminNavMenuItemService(
			PageService pageSvc
			) : base(Events.AdminNavMenuItem)
		{

			Events.Page.BeforePageInstall.AddEventListener(async(ctx, builder) => 
			{
				if (
					builder == null || 
					!builder.IsAdmin || 
					string.IsNullOrEmpty(builder.Url) ||
					builder.PageType != CommonPageType.AdminList
				)
				{
					// Non-admin list builder.
					return builder;
				}

				// Create an admin nav menu link to the target page.
				// You can disable this behaviour by ensuring both icon and title are blank.
				var title = builder.AdminNavMenuTitle;
				var icon = builder.AdminNavMenuIcon;

				if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(icon))
				{
					// Intentionally requesting no admin link
					return builder;
				}

				// Does this link exist? If yes, do nothing too.
				var existing = await Where("PageKey=?", DataOptions.IgnorePermissions)
					.Bind(builder.Key)
					.First(ctx);

				if (existing != null)
				{
					return builder;
				}

				var adminNavMenuItem = new AdminNavMenuItem()
				{
					Title = title,
					Url = builder.Url,
					IconRef = icon,
					PageKey = builder.Key
				};
				
				await Create(ctx, adminNavMenuItem, DataOptions.IgnorePermissions);
				return builder;
			});
			
			// Install example admin pages for demonstration / default setup.
			InstallAdminPages("Admin Nav Menu Items", "fa:fa-rocket", ["id", "title", "target"]);
			Cache();
		}
		
		/// <summary>
		/// Retrieves a list of admin navigation menu items that the current user is authorized to access,
		/// based on their granted capabilities for each item's content type.
		/// </summary>
		/// <param name="context">The execution context containing the user role and permission scope.</param>
		/// <returns>A list of <see cref="AdminNavMenuItem"/> the user has access to.</returns>
		/// <remarks>
		/// Only one matching granted capability is required per menu item. 
		/// Capability-to-content-type mapping is cached for performance.
		/// </remarks>
		public async ValueTask<List<AdminNavMenuItem>> ListUserAccessibleNavMenuItems(Context context)
		{
			// Menu items are cached during construction for fast access
			var allMenuItems = await Where(DataOptions.IgnorePermissions).ListAll(context);

			// Group the capabilities in a dictionary by their content type, saves the nested foreach lookups.
			var capabilitiesByType = Capabilities.GetAllCurrent()
				//  Grab associated **load** and **edit** capabilities for the targeted content type
				.Where(c => 
					c.Name.EndsWith("_load", StringComparison.OrdinalIgnoreCase) ||
					c.Name.EndsWith("_edit", StringComparison.OrdinalIgnoreCase))
				.GroupBy(c => c.ContentType)
				.ToDictionary(g => g.Key, g => g.AsEnumerable());

			var grantedMenuItems = new List<AdminNavMenuItem>(capacity: allMenuItems.Count); // Pre-allocate max capacity

			foreach (var item in allMenuItems)
			{
				var contentType = item.PageContentType;

				if (contentType is null)
				{
					continue;
				}
				
				if (!capabilitiesByType.TryGetValue(contentType, out var relevantCapabilities))
				{
					continue;
				}

				foreach (var capability in relevantCapabilities)
				{
					// Check if any single capability grants access — early exit on first match
					if (!context.Role.IsGranted(capability, context, null, false))
					{
						continue;
					}
					grantedMenuItems.Add(item);
					break; // Only one granted capability is needed — do not check the rest
				}
			}

			return grantedMenuItems;
		}


	}
}
