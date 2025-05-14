using Api.Contexts;
using Api.Eventing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Api.Startup;
using Api.CanvasRenderer;
using System.Reflection;
using System.Reflection.Metadata;
using Api.Startup.Routing;
using Api.Automations;

namespace Api.Pages
{
	/// <summary>
	/// Handles pages.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	[LoadPriority(9)]
	[HostType("web")]
	public partial class PageService : AutoService<Page>
	{
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public PageService() : base(Events.Page)
		{

			var config = GetConfig<PageServiceConfig>();

			if (config.InstallDefaultPages)
			{
				// If you don't have a homepage or admin area, this'll create them:
				Install(
					new Page()
					{
						Url = "/",
						Key = "home",
						Title = "Homepage",
						BodyJson = @"{
							""c"": {
								""t"": ""p"",
								""c"": {
									""s"": ""Welcome to your new SocialStack instance. This text comes from the pages table in your database in a format called canvas JSON - you can read more about this format in the documentation.""
								}
							}
						}"
					},
					new Page()
					{
						Url = "/en-admin/",
						Key = "admin",
						Title = "Welcome to the admin area",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Dashboard""
							}
						}"
					},
					new Page()
					{
						Url = "/en-admin/login",
						Key = "admin_login",
						Title = "Login to the admin area",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Landing"",
								""c"": {
									""t"": ""Admin/Tile"",
									""c"": {
										""t"": ""Admin/LoginForm"",
						                ""i"": 2
									},
									""i"": 3
								},
								""i"": 4
							},
							""i"": 5
						}"
					},
					new Page()
					{
						Url = "/en-admin/stdout",
						Key = "admin_stdout",
						Title = "Server log monitoring",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Default"",
								""c"": {
									""t"": ""Admin/Dashboards/Stdout""
								}
							}
						}"
					},
					new Page()
					{
						Url = "/en-admin/stress",
						Key = "admin_stress",
						Title = "Stress testing the API",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Default"",
								""c"": {
									""t"": ""Admin/Dashboards/StressTest""
								}
							}
						}"
					},
					new Page()
					{
						Url = "/en-admin/database",
						Key = "admin_database",
						Title = "Developer Database Access",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Default"",
								""c"": {
									""t"": ""Admin/Dashboards/Database""
								}
							}
						}"
					},
					new Page()
					{
						Url = "/en-admin/register",
						Key = "admin_register",
						Title = "Create a new account",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Landing"",
								""c"": {
									""t"": ""Admin/Tile"",
									""c"": {
										""t"": ""Admin/RegisterForm"",
						                ""i"": 2
									},
									""i"": 3
								},
								""i"": 4
							},
							""i"": 5
						}"
					},
					new Page()
					{
						Url = "/en-admin/permissions",
						Key = "admin_permissions",
						Title = "Permissions",
						BodyJson = @"{
							""c"": {
								""t"": ""Admin/Layouts/Default"",
								""c"": {
									""t"": ""Admin/PermissionGrid""
								}
							}
						}"
					},
					new Page()
					{
						Key = "404",
						Title = "Page not found",
						BodyJson = @"{
							""c"": {
								""t"": ""p"",
								""c"": {
									""c"": ""The page you were looking for wasn't found here."",
									""i"": 2
								}
							}
						}"
					}
				);
			}

			// Install the admin pages.
			InstallAdminPages("Pages", "fa:fa-paragraph", ["id", "url", "title"], null, "{\"requiredPermissions\": [\"page_list\", \"page_update\"]}");

			Events.Page.BeforeAdminPageInstall.AddEventListener((Context context, Page page, CanvasNode canvas, Type contentType, AdminPageType pageType) =>
			{
				// Note: Some sites are completely headless and don't have the pages module, so this can't go in upload module.
				// We use .Name here rather than typeof(Upload) to avoid coupling with uploads. Essentially, both modules are optional this way.
				if (contentType != null && contentType.Name == "Upload")
				{
					if (pageType == AdminPageType.List)
					{
						// Installing admin page for the list of uploads.
						// The create button is actually an uploader.
						canvas.Module = "Admin/Layouts/MediaCenter";
						canvas.Data.Clear();
					}
				} else if (contentType == typeof(Page) && pageType == AdminPageType.List)
				{
					// Installing the list of pages.
					// This will instead use the sitemap component.
					canvas.Module = "Admin/Layouts/Sitemap";
				}

				return new ValueTask<Page>(page);
			});

			Events.Page.BeforeAdminPageInstall.AddEventListener((context, page, canvasNode, contentType, pageType) => {

					if ((pageType == AdminPageType.Edit || pageType == AdminPageType.Add) && contentType == typeof(Page))
					{
						canvasNode.Module = "Admin/Page/Single";
					}

				return ValueTask.FromResult(page);
			});

			// Pages must always have the cache on for any release site.
			// That's because the HtmlService has a release only cache which depends on the sync messages for pages, as well as e.g. the url gen cache.
#if !DEBUG
			Cache();
#endif

		}

		/// <summary>
		/// Used as a temporary piece of JSON when setting up admin pages to help avoid people setting the bodyJson field incorrectly.
		/// </summary>
		private readonly string TemporaryBodyJson = "{\"content\":\"Don't set this field - its about to be overwritten by the contents of the Canvas object that you've been given.\"}";

		/// <summary>
		/// Installs a singular general use admin panel page. The url given is relative to the admin home (usually /en-admin/).
		/// </summary>
		/// <returns></returns>
		public async ValueTask InstallAdminPage(string relativeUrl, string title, CanvasNode pageCanvas, Type contentObjectType = null, AdminPageType pageType = null)
		{
			if (relativeUrl == null)
			{
				relativeUrl = "";
			}

			if (pageType == null)
			{
				pageType = AdminPageType.Generic;
			}

			var context = new Context();

			if (!relativeUrl.StartsWith("/"))
			{
				relativeUrl = "/" + relativeUrl;
			}

			var adminPage = new Page
			{
				Url = "/en-admin" + relativeUrl,
				Key = "admin" + (relativeUrl == "/" ? "" :  relativeUrl.ToLower().Replace('/', '_')),
				BodyJson = "",
				Title = title
			};

			// Trigger an event to state that an admin page is being installed:
			// - Use this event to inject additional nodes into the page, or change it however you'd like.
			adminPage = await Events.Page.BeforeAdminPageInstall.Dispatch(context, adminPage, pageCanvas, contentObjectType, pageType);
			adminPage.BodyJson = pageCanvas.ToJson();

			await InstallInternal(context, adminPage);
		}

		/// <summary>
		/// Installs generic admin pages using the given fields to display on the list page.
		/// </summary>
		/// <param name="type">The content type that is being installed (Page, Blog etc)</param>
		/// <param name="fields"></param>
		/// <param name="childAdminPage">
		/// A shortcut for specifying that your type has some kind of sub-type.
		/// For example, the NavMenu admin page specifies a child type of NavMenuItem, meaning each NavMenu ends up with a list of NavMenuItems.
		/// Make sure you specify the fields that'll be visible from the child type in the list on the parent type.
		/// For example, if you'd like each child entry to show its Id and Title fields, specify new string[]{"id", "title"}.
		/// </param>
		public async ValueTask InstallAdminPages(Type type, string[] fields, ChildAdminPageOptions childAdminPage)
		{
			var typeName = type.Name;
			var typeNameLowercase = type.Name.ToLower();

			// "BlogPost" -> "Blog Post".
			var tidySingularName = Api.Startup.Pluralise.NiceName(type.Name);
			var tidyPluralName = Api.Startup.Pluralise.Apply(tidySingularName);
			
			var listPageCanvas = new CanvasNode("Admin/Layouts/List")
				.With("contentType", typeName)
				.With("fields", fields)
				.With("singular", tidySingularName)
				.With("plural", tidyPluralName);
			
			var listPage = new Page
			{
				Url = "/en-admin/" + typeNameLowercase,
				Key = "admin_" + typeNameLowercase,
				BodyJson = TemporaryBodyJson,
				Title = "Edit or create " + tidyPluralName
			};

			var context = new Context();

			// Trigger an event to state that an admin page is being installed:
			// - Use this event to inject additional nodes into the page, or change it however you'd like.
			listPage = await Events.Page.BeforeAdminPageInstall.Dispatch(context, listPage, listPageCanvas, type, AdminPageType.List);
			listPage.BodyJson = listPageCanvas.ToJson();

			// Future todo - If the admin page is "pure" (it's not been edited by an actual person) then compare BodyJson as well.
			// This is why we'll always generate the bodyJson with the event.
			var editPage = await CreateSinglePage(context, type, childAdminPage, true);
			var addPage = await CreateSinglePage(context, type, childAdminPage, false);

			await InstallInternal(
				context,
				listPage,
				editPage,
				addPage
			);
		}

		private async ValueTask<Page> CreateSinglePage(Context context, Type type, ChildAdminPageOptions childAdminPage, bool isEdit)
		{
			var typeName = type.Name;
			var typeNameLowercase = type.Name.ToLower();

			// "BlogPost" -> "Blog Post".
			var tidySingularName = Api.Startup.Pluralise.NiceName(type.Name);
			var tidyPluralName = Api.Startup.Pluralise.Apply(tidySingularName);

			var singlePageCanvas = new CanvasNode("Admin/Layouts/AutoEdit")
					.With("contentType", typeName)
					.With("singular", tidySingularName)
					.With("plural", tidyPluralName);

			if (isEdit)
			{
				singlePageCanvas.With("id", "${primary.id}");
			}

			if (childAdminPage != null && childAdminPage.ChildType != null)
			{
				// This is likely obsoleted: favour more customised pages instead.
				singlePageCanvas.AppendChild(
					new CanvasNode("Admin/AutoList")
					.With("contentType", childAdminPage.ChildType)
					.With("filterField", type.Name + "Id")
					.With("create", childAdminPage.CreateButton)
					.With("searchFields", childAdminPage.SearchFields)
					.With("filterValue", "${primary.id}")
					.With("fields", childAdminPage.Fields ?? (new string[] { "id" }))
				);
			}

			var singlePage = new Page
			{
				Url = "/en-admin/" + typeNameLowercase + "/" + (isEdit ? "${" + typeNameLowercase + ".id}" : "add"),
				Key = isEdit ? "admin_primary:" + typeNameLowercase : "admin_" + typeNameLowercase + "_add",
				BodyJson = TemporaryBodyJson,
				Title = isEdit ? "Editing " + tidySingularName.ToLower() + " #${" + typeNameLowercase + ".id}" : "Creating " + tidySingularName.ToLower()
			};

			// /${" + typeNameLowercase + ".id}

			// Trigger an event to state that an admin page is being installed:
			// - Use this event to inject additional nodes into the page, or change it however you'd like.
			singlePage = await Events.Page.BeforeAdminPageInstall.Dispatch(
				context, 
				singlePage, 
				singlePageCanvas, 
				type, 
				isEdit ? AdminPageType.Edit : AdminPageType.Add
			);

			// If it's an edit page, we'll now turn it in to a graph and feed the ID from the URL in to it.

			singlePage.BodyJson = singlePageCanvas.ToJson();

			return singlePage;
		}

		/// <summary>
		/// Installs the given page(s). It checks if they exist by their URL (or ID, if you provide that instead), and if not, creates them.
		/// </summary>
		/// <param name="pages"></param>
		public void Install(params Page[] pages)
		{
			if (Services.Started)
			{
				Task.Run(async () =>
				{
					await InstallInternal(new Context(), pages);
				});
			}
			else
			{
				Events.Service.AfterStart.AddEventListener(async (Context ctx, object src) =>
				{
					await InstallInternal(ctx, pages);
					return src;
				});
			}
		}
		
		private uint? _adminHomePageId;

		/// <summary>
		/// Gets the ID of the admin panel homepage.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private async ValueTask<uint?> GetAdminHomeId(Context context)
		{
			if (_adminHomePageId.HasValue)
			{
				return _adminHomePageId.Value;
			}

			var adminHome = await Where("Key=?", DataOptions.NoCacheIgnorePermissions).Bind("admin").First(context);

			if (adminHome != null)
			{
				_adminHomePageId = adminHome.Id;
			}

			return _adminHomePageId;
		}

		/// <summary>
		/// Installs the given page(s). It checks if they exist by their InstallKey, and if not, creates them.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="pages"></param>
		private async ValueTask InstallInternal(Context context, params Page[] pages)
		{
			foreach (var page in pages)
			{
				if (string.IsNullOrEmpty(page.Key))
				{
					throw new ArgumentException("A Key is required when installing a page.");
				}
			}

			// Get the pages by those keys:
			var existingPages = (await Where("Key=[?]", DataOptions.NoCacheIgnorePermissions)
					.Bind(pages.Select(page => page.Key))
					.ListAll(context));

			var existingPagesLookup = new Dictionary<string, Page>();

			foreach (var pg in existingPages)
			{
				existingPagesLookup[pg.Key] = pg;
			}

			// For each page to consider for install..
			foreach (var page in pages)
			{
				// If it doesn't already exist, create it.
				if (!existingPagesLookup.ContainsKey(page.Key))
				{
					await Create(context, page, DataOptions.IgnorePermissions);
				}
			}
		}

	}
}
