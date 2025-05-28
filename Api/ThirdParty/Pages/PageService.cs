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
					new PageBuilder()
					{
						Url = "/",
						Key = "home",
						Title = "Homepage",
						BuildBody = (PageBuilder builder) => {
							return builder.AddTemplate(
								new CanvasNode("p")
								.AppendChild(new CanvasNode()
								{
									StringContent = "Welcome to your new SocialStack instance. This text comes from the pages table in your database in a format called canvas JSON - you can read more about this format in the documentation."
								})
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/",
						Key = "admin",
						Title = "Welcome to the admin area",
						BuildBody = (PageBuilder builder) => {
							return builder.AddTemplate(
								new CanvasNode("Admin/Layouts/Dashboard")
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/login",
						Key = "admin_login",
						Title = "Login to the admin area",
						BuildBody = (PageBuilder builder) => {
							return new CanvasNode("Admin/Layouts/Landing")
							.AppendChild(
								new CanvasNode("Admin/Tile")
								.AppendChild(
									new CanvasNode("Admin/LoginForm")
								)
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/stdout",
						Key = "admin_stdout",
						Title = "Server log monitoring",
						BuildBody = (PageBuilder builder) =>
						{
							return builder.AddTemplate(
								new CanvasNode("Admin/Dashboards/Stdout")
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/stress",
						Key = "admin_stress",
						Title = "Stress testing the API",
						BuildBody = (PageBuilder builder) =>
						{
							return builder.AddTemplate(
								new CanvasNode("Admin/Dashboards/StressTest")
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/database",
						Key = "admin_database",
						Title = "Developer Database Access",
						BuildBody = (PageBuilder builder) =>
						{
							return builder.AddTemplate(
								new CanvasNode("Admin/Dashboards/Database")
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/register",
						Key = "admin_register",
						Title = "Create a new account",
						BuildBody = (PageBuilder builder) =>
						{
							return new CanvasNode("Admin/Layouts/Landing")
							.AppendChild(
								new CanvasNode("Admin/Tile")
								.AppendChild(
									new CanvasNode("Admin/RegisterForm")
								)
							);
						}
					},
					new PageBuilder()
					{
						Url = "/en-admin/permissions",
						Key = "admin_permissions",
						Title = "Permissions",
						BuildBody = (PageBuilder builder) =>
						{
							return builder.AddTemplate(
								new CanvasNode("Admin/PermissionGrid")
							);
						}
					},
					new PageBuilder()
					{
						Key = "404",
						Title = "Page not found",
						BuildBody = (PageBuilder builder) =>
						{
							return builder.AddTemplate(
								new CanvasNode("p").AppendChild(
									new CanvasNode() {
										StringContent = "The page you were looking for wasn't found here."
									}
								)
							);
						}
					}
				);
			}

			// Install the admin pages.
			InstallAdminPages("Pages", "fa:fa-paragraph", ["id", "url", "title"], null, "{\"requiredPermissions\": [\"page_list\", \"page_update\"]}");

			Events.Page.BeforePageInstall.AddEventListener((context, builder) => {

				if (builder == null || builder.ContentType != typeof(Page))
				{
					return ValueTask.FromResult(builder);
				}

				if (builder.PageType == CommonPageType.AdminEdit || builder.PageType == CommonPageType.AdminAdd)
				{
					builder.GetContentRoot()
						.Empty()
						.AppendChild(new CanvasNode("Admin/Page/Single"));
				}
				else if (builder.PageType == CommonPageType.AdminList)
				{
					builder.GetContentRoot()
						.Empty()
						.AppendChild(new CanvasNode("Admin/Layouts/Sitemap"));
				}

				return ValueTask.FromResult(builder);
			}, 5);

			// Pages must always have the cache on for any release site.
			// That's because the HtmlService has a release only cache which depends on the sync messages for pages, as well as e.g. the url gen cache.
#if !DEBUG
			Cache();
#endif

		}

		/// <summary>
		/// Installs generic admin pages using the given fields to display on the list page.
		/// </summary>
		/// <param name="type">The content type that is being installed (Page, Blog etc)</param>
		/// <param name="fields"></param>
		/// <param name="options">
		/// Additional config options for the page.
		/// </param>
		public void InstallAdminPages(Type type, string[] fields, AdminPageOptions options)
		{
			var typeName = type.Name;
			var typeNameLowercase = type.Name.ToLower();

			// "BlogPost" -> "Blog Post".
			var tidySingularName = Api.Startup.Pluralise.NiceName(type.Name);
			var tidyPluralName = Api.Startup.Pluralise.Apply(tidySingularName);
			
			Install(new PageBuilder
			{
				ContentType = type,
				PageType = CommonPageType.AdminList,
				Url = "/en-admin/" + typeNameLowercase,
				Key = "admin_list:" + typeNameLowercase,
				Title = "Edit or create " + tidyPluralName,
				BuildBody = (PageBuilder builder) => {
					return builder.AddTemplate(
						new CanvasNode("Admin/Layouts/List")
						.With("contentType", typeName)
						.With("fields", fields)
						.With("singular", tidySingularName)
						.With("plural", tidyPluralName)
					);
				}
			});

			var incl = options == null || options.EditIncludes == null ? "*" : options.EditIncludes;
			InstallSingleAdminPage(type, true, incl);
			InstallSingleAdminPage(type, false, null);
		}

		private void InstallSingleAdminPage(Type type, bool isEdit, string includes)
		{
			var typeName = type.Name;
			var typeNameLowercase = type.Name.ToLower();

			// "BlogPost" -> "Blog Post".
			var tidySingularName = Api.Startup.Pluralise.NiceName(type.Name);
			var tidyPluralName = Api.Startup.Pluralise.Apply(tidySingularName);

			var singlePage = new PageBuilder
			{
				ContentType = type,
				PageType = isEdit ? CommonPageType.AdminEdit : CommonPageType.AdminAdd,
				PrimaryContentIncludes = includes,
				Url = "/en-admin/" + typeNameLowercase + "/" + (isEdit ? "${" + typeNameLowercase + ".id}" : "add"),
				Key = isEdit ? ("admin_primary:" + typeNameLowercase) : "admin_" + typeNameLowercase + "_add",
				Title = isEdit ? "Editing " + tidySingularName.ToLower() + " #${" + typeNameLowercase + ".id}" : "Creating " + tidySingularName.ToLower(),
				BuildBody = (PageBuilder builder) =>
				{
					var singlePageCanvas = new CanvasNode("Admin/AutoForm")
						.With("contentType", typeName)
						.With("singular", tidySingularName)
						.With("plural", tidyPluralName);

					if (isEdit)
					{
						singlePageCanvas.WithPrimaryLink("content");
					}

					return builder.AddTemplate(
						singlePageCanvas
					);
				}
			};

			Install(singlePage);
		}

		/// <summary>
		/// The built up list of pages to install when services have started.
		/// </summary>
		private List<PageBuilder> _toInstall;
		private object _installLocker = new object();

		/// <summary>
		/// Installs the given page. It checks if they exist by their URL (or ID, if you provide that instead), and if not, creates them.
		/// </summary>
		/// <param name="builders">
		/// Constructs the base page content.
		/// This will then be passed through the InstallPage function where other modules can manipulate it if needed.
		/// You can ask the provided PageInstaller for a templated root node too, and it will 
		/// generate one based on if your page is an admin one or not (established from its key starting with "admin_").
		/// </param>
		public void Install(params PageBuilder[] builders)
		{
			bool scheduleStart = false;

			lock (_installLocker)
			{
				if (_toInstall == null)
				{
					_toInstall = new List<PageBuilder>();
					scheduleStart = true;
				}

				_toInstall.AddRange(builders);
			}

			if (scheduleStart)
			{
				if (Services.Started)
				{
					Task.Run(async () =>
					{
						List<PageBuilder> set;

						lock (_installLocker)
						{
							set = _toInstall;
							_toInstall = null;
						}
						await InstallInternal(new Context(), set);
					});
				}
				else
				{
					Events.Service.AfterStart.AddEventListener(async (Context ctx, object src) =>
					{
						List<PageBuilder> set;

						lock (_installLocker)
						{
							set = _toInstall;
							_toInstall = null;
						}
						await InstallInternal(ctx, set);
						return src;
					});
				}
			}
		}
		
		private uint? _adminHomePageId;

		/// <summary>
		/// Installs the given page(s). It checks if they exist by their InstallKey, and if not, creates them.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="builders"></param>
		private async ValueTask InstallInternal(Context context, List<PageBuilder> builders)
		{
			if (builders == null)
			{
				return;
			}

			foreach (var builder in builders)
			{
				if (string.IsNullOrEmpty(builder.Key))
				{
					throw new ArgumentException("A Key is required when installing a page.");
				}
			}

			// Get the pages by those keys:
			var existingPages = (await Where("Key=[?]", DataOptions.NoCacheIgnorePermissions)
					.Bind(builders.Select(page => page.Key))
					.ListAll(context));

			var existingPagesLookup = new Dictionary<string, Page>();

			foreach (var pg in existingPages)
			{
				existingPagesLookup[pg.Key] = pg;
			}

			// For each page to consider for install..
			foreach (var builder in builders)
			{
				// If it doesn't already exist, create it.
				if (existingPagesLookup.ContainsKey(builder.Key))
				{
					continue;
				}

				// Start building:
				builder.Build();

				await Events.Page.BeforePageInstall.Dispatch(context, builder);
				builder.Page.BodyJson = builder.Body.ToJson();

				await Create(context, builder.Page, DataOptions.IgnorePermissions);
			}
		}

	}
}
