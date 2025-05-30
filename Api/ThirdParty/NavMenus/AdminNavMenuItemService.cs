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
	[AdminNav("fa:fa-child")]
	public partial class AdminNavMenuItemService : AutoService<AdminNavMenuItem>
	{
		private readonly AvailableEndpointService _endpointService;
		
		private readonly PageService _pageService;
		
		/// <summary>
		/// Cache to avoid repeated expensive lookups of content types by key.
		/// Key is case-insensitive to be resilient to case variations in page keys.
		/// </summary>
		private readonly ConcurrentDictionary<string, Type> _keyContentTypes = new(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Constructor injecting dependencies.
		/// Calls base constructor with appropriate event topic.
		/// Calls example admin page install to bootstrap initial state.
		/// </summary>
		public AdminNavMenuItemService(
			AvailableEndpointService aes,
			PageService pageSvc
			) : base(Events.AdminNavMenuItem)
		{
			_endpointService = aes;
			_pageService = pageSvc;
			
			Events.Permalink.BeforeCreate.AddEventListener(async (ctx, permalink) =>
			{
				var adminContext = new Context(1, 1, 1);
				if (permalink.Target.StartsWith("page:"))
				{
					var id = permalink.Target.Split(':')[1];

					if (string.IsNullOrEmpty(id))
					{
						throw new PublicException("Permalink target is missing appended ID", "permalink/bad-format");
					}

					if (uint.TryParse(id, out var pageId))
					{
						var page = await pageSvc.Get(adminContext, pageId, DataOptions.IgnorePermissions);

						if (page == null)
						{
							throw new PublicException(
								$"Permalink target is invalid, no page found with the ID {pageId}",
								"permalink/invalid-target"
							);
						}

						var pageKey = page.Key.Split(':');

						if (pageKey.Length != 2)
						{
							return permalink;
						}

						var pageType = pageKey[0];
						var entity   = pageKey[1];

						if (pageType != "admin_list")
						{
							return permalink;
						}

						var svc = Services.Get(entity + "Service");

						if (svc == null)
						{
							return permalink;
						}

						var adminNavAttr = svc.GetType().GetCustomAttribute<AdminNavAttribute>();
						var icon = adminNavAttr?.Icon ?? "fa:fa-rocket";
						
						// no we have the page & permalink, construct the link.
						
						var tidyPluralName = Pluralise.PluraliseWords(Pluralise.NiceName(svc.EntityName));

						var adminNavMenuItem = new AdminNavMenuItem()
						{
							Title = adminNavAttr?.Label ?? char.ToUpper(tidyPluralName[0]) + tidyPluralName[1..],
							Url = permalink.Url,
							IconRef = icon,
							PageKey = page.Key
						};
						
						await Create(adminContext, adminNavMenuItem);
					}
				}

				return permalink;
			});
			
			// Install example admin pages for demonstration / default setup.
			InstallAdminPages(["id", "title", "target"]);
			Cache();
		}
		
		/// <summary>
		/// Returns a list of admin navigation menu items accessible to the current user,
		/// based on their granted capabilities and the content type of each item.
		/// </summary>
		/// <param name="context">Execution context containing role and user permission information.</param>
		/// <returns>A list of <see cref="AdminNavMenuItem"/> objects that the user has access to.</returns>
		/// <remarks>
		/// This method assumes that capability data is cached and cheap to access. It optimizes permission checks
		/// by grouping capabilities by content type for faster lookups.
		/// </remarks>
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
					if (!await context.Role.IsGranted(capability, context, null, false))
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
