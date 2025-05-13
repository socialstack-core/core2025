using Api.Pages;
using System;

namespace Api.Startup.Routing;


/// <summary>
/// A terminal behaviour which is used to route to a Page.
/// </summary>
public class PageTerminalBehaviour : TerminalBehaviour
{
	/// <summary>
	/// The page itself.
	/// </summary>
	public Page Page;

	/// <summary>
	/// The primary content service for this page.
	/// </summary>
	public AutoService PrimaryService;

	/// <summary>
	/// An ID for a specific piece of primary content. If null and PrimaryService is set, this page is 
	/// the fallback primary content page.
	/// </summary>
	public string SpecificContentId;

	/// <summary>
	/// Creates a new terminal method.
	/// </summary>
	/// <param name="page"></param>
	/// <param name="primaryService"></param>
	/// <param name="specificContentId"></param>
	public PageTerminalBehaviour(Page page, AutoService primaryService, string specificContentId)
	{
		Page = page;
		PrimaryService = primaryService;
		SpecificContentId = specificContentId;
	}

	/// <summary>
	/// True if the behaviours are the same type and target.
	/// </summary>
	/// <returns></returns>
	public override bool Equals(TerminalBehaviour behaviour)
	{
		var node = behaviour as PageTerminalBehaviour;
		var otherId = node.Page?.Id;
		var selfId = Page?.Id;
		return node != null && otherId == selfId;
	}

	/// <summary>
	/// Clones this behaviour.
	/// </summary>
	/// <returns></returns>
	public override TerminalBehaviour Clone()
	{
		return new PageTerminalBehaviour(Page, PrimaryService, SpecificContentId);
	}

	/// <summary>
	/// Constructs this node in to a readonly tree node.
	/// </summary>
	/// <returns></returns>
	public override TerminalNode Build(BuilderNode node)
	{
		var tokens = node.GetAllTokens();

		if (PrimaryService == null)
		{
			// Normal page
			return new RouterPageTerminal(
				node.BuildChildren(),
				tokens,
				null,
				Page,
				node.IsToken ? null : node.Text,
				node.FullRoute
			);
		}

		// Page with primary content
		var rpt = typeof(RouterPageTerminal<,>).MakeGenericType(PrimaryService.ServicedType, PrimaryService.IdType);

		var result = Activator.CreateInstance(rpt, new object[] {
			node.BuildChildren(),
			tokens,
			PrimaryService,
			SpecificContentId,
			Page,
			node.IsToken ? null : node.Text,
			node.FullRoute
		});

		return result as RouterPageTerminal;
	}

}
