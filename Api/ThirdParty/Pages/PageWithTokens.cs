using Api.CanvasRenderer;
using Api.Startup.Routing;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Api.Pages;


/// <summary>
/// A page and token values from the URL.
/// </summary>
public struct PageWithTokens
{
	/// <summary>
	/// The primary object for this page.
	/// </summary>
	public object PrimaryObject;
	/// <summary>
	/// The service for the primary object for this page.
	/// </summary>
	public AutoService PrimaryService;
	/// <summary>
	/// The host.
	/// </summary>
	public HostString Host;
	/// <summary>
	/// Any token values in the URL.
	/// </summary>
	public List<string> TokenValues;
	/// <summary>
	/// The page terminal in the router. Can be null.
	/// </summary>
	public RouterPageTerminal PageTerminal;
}
