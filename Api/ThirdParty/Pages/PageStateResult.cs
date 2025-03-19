using Api.Configuration;
using System.Collections.Generic;

namespace Api.Pages;


/// <summary>
/// The response of a page/state call.
/// </summary>
public struct PageStateResult
{
	/// <summary>
	/// True if the request originated from an old version and the page needs to be reloaded.
	/// </summary>
	public bool OldVersion;

	/// <summary>
	/// Optional redirect target.
	/// </summary>
	public string Redirect;

	/// <summary>
	/// Configuration.
	/// </summary>
	public Dictionary<string, Config> Config;

	/// <summary>
	/// The page itself.
	/// </summary>
	public Page Page;
}