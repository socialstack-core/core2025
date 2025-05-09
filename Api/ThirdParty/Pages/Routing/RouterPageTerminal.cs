using Api.CanvasRenderer;
using Api.Contexts;
using Api.Pages;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Api.Startup.Routing;


/// <summary>
/// A router node which represents a page.
/// </summary>
public class RouterPageTerminal : TerminalNode
{
	/// <summary>
	/// The page at this terminal.
	/// </summary>
	public readonly Page Page;

	/// <summary>
	/// True if this is an admin group page. This impacts the head as the page is rendered with e.g. the admin modules.
	/// </summary>
	public bool IsAdmin;

	/// <summary>
	/// Preformatted JSON array of the url token names. ["A", "B", ..]. will be the string "null" if it is null.
	/// </summary>
	public readonly string TokenNamesJson;

	/// <summary>
	/// The set of token metadata, if there were any in the canonical URL.
	/// </summary>
	public readonly PageUrlToken[] TokenNames;

	private readonly HtmlService _htmlService;

	/// <summary>
	/// The page generator.
	/// </summary>
	public readonly CanvasGenerator Generator;

	/// <summary>
	/// Creates a new page terminal.
	/// </summary>
	/// <param name="children"></param>
	/// <param name="page"></param>
	/// <param name="primaryContentType"></param>
	/// <param name="exactMatch"></param>
	/// <param name="fullRoute"></param>
	public RouterPageTerminal(IntermediateNode[] children, Type primaryContentType, Page page, string exactMatch, string fullRoute) 
		: base(children, exactMatch, null, null, fullRoute)
	{
		Page = page;
		_htmlService = Services.Get<HtmlService>();
		Generator = new CanvasGenerator(page.BodyJson, primaryContentType);
	}

	/// <summary>
	/// Execute this page node.
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="context"></param>
	/// <param name="tokenCount"></param>
	/// <param name="tokens"></param>
	/// <returns></returns>
	public override ValueTask<bool> Run(HttpContext httpContext, Context context, int tokenCount, ref Span<TokenMarker> tokens)
	{
		// Map to the non web specific page renderer:
		var pageWithTokens = new PageWithTokens()
		{
			PageTerminal = this,
			Host = httpContext.Request.Host
		};

		return _htmlService.RouteRequest(httpContext, context, pageWithTokens);
	}
}