using Api.Contexts;
using Api.Database;
using Api.Eventing;
using Api.Permissions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Api.CanvasRenderer;
using Newtonsoft.Json.Linq;
using Api.Templates;
using Api.Startup;

namespace Api.Templates
{
	/// <summary>
	/// Handles templates.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	[CustomCreatePage("Admin/Template/Create")]
	public partial class TemplateService : AutoService<Template>
    {
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public TemplateService() : base(Events.Template)
        {
			InstallAdminPages("Templates", "fa:fa-file-medical", new string[] { "id", "title", "key" });
			Cache();

			Events.Page.TransformCanvasNode.AddEventListener(async (Context context, CanvasNode node) => {

				if (node == null || node.Source == null)
				{
					return node;
				}

				if (node.Module == "Admin/Template")
				{
					// Sub in the template
					var templateIdJson = node.Source["tid"];

					if (templateIdJson != null && templateIdJson.Type == JTokenType.Integer)
					{
						var templateId = templateIdJson.Value<uint>();

						// Load the template:
						var template = await Get(context, templateId, DataOptions.IgnorePermissions);

						if (template == null)
						{
							// Can't load this node as the template was deleted.
							return null;
						}

						// Load the template body (which can cause further substition if needed):
						var templateInfo = await LoadTemplate(context, template, node);

						return templateInfo.LoadedTemplate;
					}
				}
				else if (node.Module == "Admin/Template/Slot")
				{
					// Replace the slot with the provided slot data from the template.
					if (node.Canvas == null || node.Canvas.Template == null || node.Data == null)
					{
						Log.Warn(LogTag, "A template slot node is present on a non-template canvas.");
						return null;
					}

					// The template info tells us 
					var templateInfo = node.Canvas.Template;

					// Locate the root replacement for this slot:
					if (templateInfo.Config.Roots == null || !node.Data.TryGetValue("name", out string rootName))
					{
						Log.Warn(LogTag, "Ignoring either a template slot which has no name, or no source config for the slot");
						return null;
					}

					if (!templateInfo.Config.Roots.TryGetValue(rootName, out CanvasNode root))
					{
						// This one is intentionally silent.
						// It'll happen if a slot was optional and the user simply didn't populate it.
						return null;
					}
					
					// Substitution time:
					return root;
				}

				return node;
			});

		}

		/// <summary>
		/// Loads a template.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="template"></param>
		/// <param name="templateConfig"></param>
		/// <returns></returns>
		public async ValueTask<TemplateDetails> LoadTemplate(Context context, Template template, CanvasNode templateConfig)
		{
			// Load the JSON.
			var json = Newtonsoft.Json.JsonConvert.DeserializeObject(template.BodyJson) as JToken;

			var details = new TemplateDetails()
			{
				Template = template,
				Config = templateConfig
			};

			var templateBody = await CanvasNode.LoadCanvasNode(context, json, new CanvasDetails()
			{
				Template = details
			});

			details.LoadedTemplate = templateBody;
			return details;
		}
		
	}

	/// <summary>
	/// Details for a template.
	/// </summary>
	public class TemplateDetails
	{
		/// <summary>
		/// The template node.
		/// </summary>
		public CanvasNode LoadedTemplate;
		
		/// <summary>
		/// The node providing the instance config.
		/// This is where the roots are that will be placed into slots.
		/// </summary>
		public CanvasNode Config;

		/// <summary>
		/// The template itself.
		/// </summary>
		public Template Template;

	}


}

namespace Api.CanvasRenderer {
	public partial class CanvasDetails
	{
		/// <summary>
		/// The template it is a part of.
		/// </summary>
		public TemplateDetails Template;
	}
}