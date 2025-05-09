using Api.Contexts;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Api.Startup.Routing;


/// <summary>
/// The root of the URL router. It has multiple trees - one per http verb.
/// Routers are static structures. To change routes, you need to create a new router.
/// </summary>
public class Router
{
	/// <summary>
	/// The current router.
	/// </summary>
	public static Router CurrentRouter;

	/// <summary>
	/// True if the given router is not the current one.
	/// </summary>
	/// <param name="router"></param>
	/// <returns></returns>
	public static bool IsStale(Router router)
	{
		return router != CurrentRouter;
	}

	/// <summary>
	/// Max number of tokens allowed in a URL.
	/// </summary>
	public static readonly int MaxTokenCount = 4;

	/// <summary>
	/// The set of router trees by HTTP verb.
	/// </summary>
	public readonly RouteNode[] TreesByVerb;

	private readonly PathString Slash = new PathString("/");

	/// <summary>
	/// Create a new router. Usually done via a RouterBuilder.
	/// </summary>
	/// <param name="treesByVerb"></param>
	public Router(RouteNode[] treesByVerb)
	{
		TreesByVerb = treesByVerb;
	}

	/// <summary>
	/// For each endpoint in the router, the given callback is executed.
	/// </summary>
	/// <param name="callback"></param>
	public void ForEachEndpoint(Action<TerminalNode> callback)
	{
		for (var i = 0; i < TreesByVerb.Length; i++)
		{
			TreesByVerb[i].ForEachEndpoint(callback);
		}
	}

	/// <summary>
	/// Handle the given request.
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="basicContext"></param>
	/// <returns></returns>
	public ValueTask<bool> HandleRequest(HttpContext httpContext, Context basicContext)
	{
		var req = httpContext.Request;
		var method = req.Method;

		int verbIndex;

		switch (method)
		{
			case "GET":
				verbIndex = 0;
				break;
			case "POST":
				verbIndex = 1;
				break;
			case "DELETE":
				verbIndex = 2;
				break;
			case "PUT":
				verbIndex = 3;
				break;
			default:
				return new ValueTask<bool>(false);
		}

		// The requested path is..
		ReadOnlySpan<char> path;
		int pathStartIndex;
		int pathMax = 0;

		if (req.Path.HasValue)
		{
			// If it starts with a /, ignore it.
			path = req.Path.Value.AsSpan();
			pathMax = path.Length;
			var startsWithSlash = (path[0] == '/');
			var endsWithSlash = (pathMax > 1 && path[pathMax - 1] == '/');

			if (startsWithSlash)
			{
				pathStartIndex = 1;

				if (endsWithSlash)
				{
					pathMax--; // Yes this is correct! The max only moves down 1 as the paths actual true end is only moving 1 place.
					path = path.Slice(1, pathMax - 1); // The subpath however gets 2 chars shorter.
				}
				else
				{
					path = path.Slice(1);
				}
			}
			else if (endsWithSlash)
			{
				pathMax--;
				path = path.Slice(0, pathMax);
				pathStartIndex = 0;
			}
			else
			{
				pathStartIndex = 0;
			}
		}
		else
		{
			path = ReadOnlySpan<char>.Empty;
			pathStartIndex = 0;
		}

		// Max of N tokens (4 is the default).
		var tokenCount = 0;
		Span<TokenMarker> tokenSet = stackalloc TokenMarker[MaxTokenCount];

		// Initial node is..
		var current = TreesByVerb[verbIndex];

		while (true)
		{
			int index = path.IndexOf('/');

			if (index == -1)
			{
				// It's the whole remaining segment.
				var finalTerminal = current.FindChildNode(path);

				// Is current a capture token?
				if (finalTerminal != null)
				{
					if (finalTerminal.IsRewrite)
					{
						// It's a rewrite. Tokens are reset to the contents of the rewrite node.
						var rewrite = (TerminalRewriteNode)finalTerminal;
						finalTerminal = rewrite.GoTo as IntermediateNode;
						tokenCount = rewrite.ReplaceTokens(ref tokenSet);
					}
					else if (finalTerminal.Capture)
					{
						// Yes - store the child segment as a token.
						// It is assumed that URLs that have too many tokens are never added to the tree
						// to minimise logic on the hot path as much as possible.
						tokenSet[tokenCount] = new TokenMarker(pathStartIndex, pathMax - pathStartIndex);
						tokenCount++;
					}
				}

				var terminal = finalTerminal as TerminalNode;

				if (terminal == null)
				{
					return new ValueTask<bool>(false);
				}

				return terminal.Run(httpContext, basicContext, tokenCount, ref tokenSet);
			}

			ReadOnlySpan<char> childSegment;

			if (index == 0)
			{
				childSegment = ReadOnlySpan<char>.Empty;
			}
			else
			{
				childSegment = path.Slice(0, index);
			}

			var next = current.FindChildNode(childSegment);

			if (next == null)
			{
				// 404
				return new ValueTask<bool>(false);
			}

			if (next.IsRewrite)
			{
				// It's a rewrite. Tokens are reset to the contents of the rewrite node.
				var rewrite = (TerminalRewriteNode)next;
				next = rewrite.GoTo as IntermediateNode;
				tokenCount = rewrite.ReplaceTokens(ref tokenSet);
			}
			else if (next.Capture) 
			{
				// It's a capture token - store the child segment as a token.
				// It is assumed that URLs that have too many tokens are never added to the tree
				// to minimise logic on the hot path as much as possible.
				tokenSet[tokenCount] = new TokenMarker(pathStartIndex, index);
				tokenCount++;
			}

			// Move along the path:
			path = path.Slice(index + 1);
			pathStartIndex += index + 1;
			current = next;
		}
	}

}

/// <summary>
/// A marked region in a path where a token is located.
/// </summary>
public struct TokenMarker
{
	/// <summary>
	/// Index in the lookup, if this token is a static one.
	/// </summary>
	public readonly int LookupIndex;
	/// <summary>
	/// The start of the token region
	/// </summary>
	public readonly int Start;
	/// <summary>
	/// The end of the token region
	/// </summary>
	public readonly int Length;


	/// <summary>
	/// Creates a new token marker.
	/// </summary>
	/// <param name="start"></param>
	/// <param name="length"></param>
	/// <param name="lookupIndex"></param>
	public TokenMarker(int start, int length, int lookupIndex = -1)
	{
		Start = start;
		Length = length;
		LookupIndex = lookupIndex;
	}
}