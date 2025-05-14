using Api.CanvasRenderer;
using Api.Pages;
using Api.Permissions;
using System.Collections.Generic;

namespace Api.Eventing
{
	/// <summary>
	/// Events are instanced automatically. 
	/// You can however specify a custom type or instance them yourself if you'd like to do so.
	/// </summary>
	public partial class Events
	{
		/// <summary>
		/// All page entity events.
		/// </summary>
		public static PageEventGroup Page;

		/// <summary>
		/// Set of events for a permalink.
		/// </summary>
		public static EventGroup<Permalink> Permalink;
	}

	/// <summary>
	/// Page entity specific extensions to events.
	/// </summary>
	public class PageEventGroup : EventGroup<Page>
	{
		/// <summary>
		/// Canvas node transformation.
		/// </summary>
		public EventHandler<CanvasNode> TransformCanvasNode;
		
		/// <summary>
		/// Before a user is about to navigate to a page (the server is generating either just the state or the html for them).
		/// </summary>
		public EventHandler<PageWithTokens> BeforeNavigate;

		/// <summary>
		/// On admin page install.
		/// </summary>
		public EventHandler<Page, CanvasRenderer.CanvasNode, System.Type, AdminPageType> BeforeAdminPageInstall;
		
	}
}