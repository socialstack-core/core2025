using Api.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Api.Startup.Routing;



/// <summary>
/// A node in the URL routing tree.
/// </summary>
public class RouteNode
{
	/// <summary>
	/// Locates a child node by its name specified in the given span.
	/// </summary>
	public virtual IntermediateNode FindChildNode(ReadOnlySpan<char> childName)
	{
		return null;
	}

	/// <summary>
	/// For each endpoint in the router, the given callback is executed.
	/// </summary>
	/// <param name="callback"></param>
	public virtual void ForEachEndpoint(Action<TerminalNode> callback)
	{
	}

}

/// <summary>
/// A request handler for a terminal. These are frequently generative.
/// </summary>
/// <param name="node"></param>
/// <param name="httpContext"></param>
/// <param name="basicContext"></param>
/// <param name="boundState"></param>
/// <param name="body"></param>
/// <returns></returns>
public delegate ValueTask<OutputType> TerminalMethod<State, OutputType, BodyType>(
	TerminalNode<State, OutputType, BodyType> node, 
	HttpContext httpContext, 
	Context basicContext, 
	State boundState,
	BodyType body
) where State : struct;

/// <summary>
/// A request handler for loading the body, if there is one.
/// </summary>
/// <param name="httpRequest"></param>
/// <returns></returns>
public delegate ValueTask<BodyType> TerminalBodyLoader<BodyType>(
	HttpRequest httpRequest
);

/// <summary>
/// A request handler for a terminal. These are frequently generative.
/// </summary>
/// <param name="node"></param>
/// <param name="httpContext"></param>
/// <param name="basicContext"></param>
/// <param name="boundState"></param>
/// <param name="body"></param>
/// <returns></returns>
public delegate ValueTask TerminalVoidMethod<State, BodyType>(
	TerminalVoidNode<State, BodyType> node, 
	HttpContext httpContext, 
	Context basicContext, 
	State boundState,
	BodyType body
) where State : struct;

/// <summary>
/// A request serialiser for a terminal. These are frequently generative.
/// </summary>
/// <param name="node"></param>
/// <param name="httpResponse"></param>
/// <param name="context"></param>
/// <param name="toSerialise"></param>
/// <returns></returns>
public delegate ValueTask TerminalSerialiser<State, OutputType, BodyType>(
	TerminalNode<State, OutputType, BodyType> node,
	HttpResponse httpResponse,
	Context context,
	OutputType toSerialise
) where State : struct;

/// <summary>
/// Binds the given terminal state from the given tokens.
/// </summary>
/// <typeparam name="State"></typeparam>
/// <param name="request"></param>
/// <param name="tokens"></param>
/// <returns></returns>
public delegate State TerminalBinderMethod<State>(HttpRequest request, ref Span<TokenMarker> tokens) where State : struct;

/// <summary>
/// A general use terminal node.
/// </summary>
public class TerminalNode : ArrayIntermediateNode
{
	/// <summary>
	/// Common JSON serialiser config.
	/// </summary>
	public static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
	{
		ContractResolver = new DefaultContractResolver
		{
			NamingStrategy = new CamelCaseNamingStrategy()
		},
		Formatting = Formatting.None
	};

	/// <summary>
	/// Used by the body loader when there is no body present. It usually gets optimised out completely.
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	public static ValueTask<EmptyTerminalState> ReadNothing(HttpRequest request)
	{
		return new ValueTask<EmptyTerminalState>(new EmptyTerminalState());
	}

	/// <summary>
	/// Deserialises the given type from the given http request.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="request"></param>
	/// <returns></returns>
	public static async ValueTask<T> ReadJsonBodyAsync<T>(HttpRequest request)
	{
		using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
		var body = await reader.ReadToEndAsync();
		return JsonConvert.DeserializeObject<T>(body);
	}
	
	/// <summary>
	/// Gets the common includes string from the query string.
	/// </summary>
	/// <param name="response"></param>
	/// <returns></returns>
	public static string GetIncludesString(HttpResponse response)
	{
		var request = response.HttpContext.Request;

		if (!request.Query.TryGetValue("includes", out StringValues sv))
		{
			return null;
		}

		return sv[sv.Count - 1];
	}

	/// <summary>
	/// The instance of the controller if there is one.
	/// </summary>
	public readonly object ControllerInstance;

	/// <summary>
	/// The complete original route to this node.
	/// </summary>
	public readonly string FullRoute;

	/// <summary>
	/// The node which constructed this one.
	/// </summary>
	public BuilderNode BuilderSource;

	/// <summary>
	/// Create a new terminal node.
	/// </summary>
	/// <param name="children"></param>
	/// <param name="exactMatch"></param>
	/// <param name="controllerInstance"></param>
	/// <param name="service"></param>
	/// <param name="fullRoute"></param>
	public TerminalNode(
		IntermediateNode[] children,
		string exactMatch,
		object controllerInstance,
		object service,
		string fullRoute
	) : base(children, exactMatch)
	{
		FullRoute = fullRoute;
		Service = service;
		ControllerInstance = controllerInstance;
		_ctxService = Services.Get<ContextService>();
	}

	/// <summary>
	/// For each endpoint in the router, the given callback is executed.
	/// </summary>
	/// <param name="callback"></param>
	public override void ForEachEndpoint(Action<TerminalNode> callback)
	{
		callback(this);
		base.ForEachEndpoint(callback);
	}

	/// <summary>
	/// If a content object is serialised, this is the service reference.
	/// </summary>
	public readonly object Service;

	/// <summary>
	/// Context service used
	/// </summary>
	private readonly ContextService _ctxService;

	/// <summary>
	/// Outputs a context.
	/// </summary>
	/// <param name="response"></param>
	/// <param name="context"></param>
	/// <returns></returns>
	public async ValueTask OutputContext(HttpResponse response, Context context)
	{
		// Regenerate the contextual token:
		context.SendToken(response);

		response.ContentType = "application/json";
		await _ctxService.ToJson(context, response.Body);
	}
	
	/// <summary>
	/// Run this route node.
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="basicContext"></param>
	/// <param name="tokenCount"></param>
	/// <param name="tokens"></param>
	/// <returns></returns>
	public virtual ValueTask<bool> Run(HttpContext httpContext, Context basicContext, int tokenCount, ref Span<TokenMarker> tokens)
	{
		return new ValueTask<bool>(false);
	}
}

/// <summary>
/// A node at the end of a route.
/// </summary>
public class TerminalNode<State, OutputType, BodyType> : TerminalNode
	where State : struct
{
	/// <summary>
	/// The binder method, which binds URL args.
	/// </summary>
	public readonly TerminalBinderMethod<State> Binder;

	/// <summary>
	/// The method to run.
	/// </summary>
	public readonly TerminalMethod<State, OutputType, BodyType> Method;
	
	/// <summary>
	/// The body loader.
	/// </summary>
	public readonly TerminalBodyLoader<BodyType> BodyLoader;
	
	/// <summary>
	/// Serialises the output.
	/// </summary>
	public readonly TerminalSerialiser<State, OutputType, BodyType> Serialise;

	/// <summary>
	/// True if the full context is required.
	/// </summary>
	public readonly bool RequireFullContext;

	/// <summary>
	/// Create a new terminal node.
	/// </summary>
	/// <param name="children"></param>
	/// <param name="exactMatch"></param>
	/// <param name="bodyLoader"></param>
	/// <param name="binder">Binds values from the src.</param>
	/// <param name="method">Runs the actual endpoint using any bound values.</param>
	/// <param name="serialiser">Serialises the output.</param>
	/// <param name="controllerInstance"></param>
	/// <param name="svc"></param>
	/// <param name="requireFullContext"></param>
	/// <param name="fullRoute"></param>
	public TerminalNode(
		IntermediateNode[] children, 
		string exactMatch,
		TerminalBinderMethod<State> binder, 
		TerminalMethod<State, OutputType, BodyType> method,
		TerminalBodyLoader<BodyType> bodyLoader,
		TerminalSerialiser<State, OutputType, BodyType> serialiser, 
		object controllerInstance = null, 
		object svc = null,
		bool requireFullContext = false,
		string fullRoute = null
	) : base(children, exactMatch, controllerInstance, svc, fullRoute)
	{
		Method = method;
		BodyLoader = bodyLoader;
		Binder = binder;
		Serialise = serialiser;
		RequireFullContext = requireFullContext;
	}

	/// <summary>
	/// Run this route node.
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="basicContext"></param>
	/// <param name="tokenCount"></param>
	/// <param name="tokens"></param>
	/// <returns></returns>
	public override ValueTask<bool> Run(HttpContext httpContext, Context basicContext, int tokenCount, ref Span<TokenMarker> tokens)
	{
		State state = Binder(httpContext.Request, ref tokens);
		return InternalRunMethod(httpContext, basicContext, state);
	}

	private async ValueTask<bool> InternalRunMethod(HttpContext httpContext, Context basicContext, State state)
	{
		if (RequireFullContext)
		{
			basicContext = await httpContext.Request.GetContext(basicContext);
		}

		// Load the body:
		BodyType body = await BodyLoader(httpContext.Request);

		// Run the endpoint:
		var output = await Method(this, httpContext, basicContext, state, body);

		// Serialise the output:
		await Serialise(this, httpContext.Response, basicContext, output);

		return true;
	}

}

/// <summary>
/// A node at the end of a route.
/// </summary>
public class TerminalVoidNode<State, BodyType> : TerminalNode
	where State : struct
{
	/// <summary>
	/// The binder method, which binds URL args.
	/// </summary>
	public readonly TerminalBinderMethod<State> Binder;

	/// <summary>
	/// The method to run.
	/// </summary>
	public readonly TerminalVoidMethod<State, BodyType> Method;

	/// <summary>
	/// The body loader.
	/// </summary>
	public readonly TerminalBodyLoader<BodyType> BodyLoader;
	
	/// <summary>
	/// True if the full context is required.
	/// </summary>
	public readonly bool RequireFullContext;

	/// <summary>
	/// Create a new terminal node.
	/// </summary>
	/// <param name="children"></param>
	/// <param name="exactMatch"></param>
	/// <param name="bodyLoader"></param>
	/// <param name="binder">Binds values from the src.</param>
	/// <param name="method">Runs the actual endpoint using any bound values.</param>
	/// <param name="controllerInstance"></param>
	/// <param name="requireFullContext"></param>
	/// <param name="fullRoute"></param>
	public TerminalVoidNode(
		IntermediateNode[] children,
		string exactMatch,
		TerminalBinderMethod<State> binder,
		TerminalVoidMethod<State, BodyType> method,
		TerminalBodyLoader<BodyType> bodyLoader,
		object controllerInstance = null,
		bool requireFullContext = false,
		string fullRoute = null
	) : base(children, exactMatch, controllerInstance, null, fullRoute)
	{
		BodyLoader = bodyLoader;
		Method = method;
		Binder = binder;
		RequireFullContext = requireFullContext;
	}

	/// <summary>
	/// Run this route node.
	/// </summary>
	/// <param name="httpContext"></param>
	/// <param name="basicContext"></param>
	/// <param name="tokenCount"></param>
	/// <param name="tokens"></param>
	/// <returns></returns>
	public override ValueTask<bool> Run(HttpContext httpContext, Context basicContext, int tokenCount, ref Span<TokenMarker> tokens)
	{
		State state = Binder(httpContext.Request, ref tokens);
		return InternalRunMethod(httpContext, basicContext, state);
	}

	private async ValueTask<bool> InternalRunMethod(HttpContext httpContext, Context basicContext, State state)
	{
		if (RequireFullContext)
		{
			basicContext = await httpContext.Request.GetContext(basicContext);
		}

		BodyType body = await BodyLoader(httpContext.Request);
		await Method(this, httpContext, basicContext, state, body);
		return true;
	}
}

/// <summary>
/// Non-root nodes in the tree are all intermediates.
/// </summary>
public class IntermediateNode : RouteNode
{
	/// <summary>
	/// True if  the value of this node should be captured as a token.
	/// </summary>
	public readonly bool Capture;

	/// <summary>
	/// An exact match string for "this" node.
	/// </summary>
	public readonly string ExactMatch;

	/// <summary>
	/// Create a new intermediate node.
	/// </summary>
	/// <param name="exactMatch"></param>
	public IntermediateNode(string exactMatch)
	{
		if (exactMatch == null)
		{
			Capture = true;
		}
		else
		{
			ExactMatch = exactMatch;
		}
	}
}

/// <summary>
/// Used when the possible children is a small set.
/// </summary>
public class ArrayIntermediateNode : IntermediateNode
{
	/// <summary>
	/// The children set, sorted alphabetically with wildcards always last.
	/// </summary>
	private readonly IntermediateNode[] Children;
	
	/// <summary>
	/// </summary>
	/// <param name="childSet"></param>
	/// <param name="exactMatch"></param>
	public ArrayIntermediateNode(IntermediateNode[] childSet, string exactMatch) : base(exactMatch)
	{
		Children = childSet;
	}

	/// <summary>
	/// For each endpoint in the router, the given callback is executed.
	/// </summary>
	/// <param name="callback"></param>
	public override void ForEachEndpoint(Action<TerminalNode> callback)
	{
		for (var i = 0; i < Children.Length; i++)
		{
			Children[i].ForEachEndpoint(callback);
		}
	}

	/// <summary>
	/// Locates a child node by its name specified in the given span.
	/// </summary>
	public override IntermediateNode FindChildNode(ReadOnlySpan<char> childName)
	{
		for(var i=0;i<Children.Length;i++)
		{
			var child = Children[i];
			
			if(child.Capture)
			{
				// It's a wildcard node (ExactMatch is null).
				return child;
			}
			
			if(childName.SequenceEqual(child.ExactMatch))
			{
				return child;
			}
		}
		
		// No matches.
		return null;
	}
	
}

/// <summary>
/// The very root of the routing tree.
/// </summary>
public class RootNode : RouteNode
{
	/// <summary>
	/// The children set, sorted alphabetically with wildcards always last.
	/// </summary>
	private readonly IntermediateNode[] Children;
	
	/// <summary>
	/// </summary>
	/// <param name="childSet"></param>
	public RootNode(IntermediateNode[] childSet)
	{
		Children = childSet;
	}

	/// <summary>
	/// For each endpoint in the router, the given callback is executed.
	/// </summary>
	/// <param name="callback"></param>
	public override void ForEachEndpoint(Action<TerminalNode> callback)
	{
		for (var i = 0; i < Children.Length; i++)
		{
			Children[i].ForEachEndpoint(callback);
		}
	}

	/// <summary>
	/// Locates a child node by its name specified in the given span.
	/// </summary>
	public override IntermediateNode FindChildNode(ReadOnlySpan<char> childName)
	{
		for(var i=0;i<Children.Length;i++)
		{
			var child = Children[i];
			
			if(child.Capture)
			{
				// It's a wildcard node (ExactMatch is null).
				return child;
			}
			
			if(childName.SequenceEqual(child.ExactMatch))
			{
				return child;
			}
		}
		
		// No matches.
		return null;
	}
	
}