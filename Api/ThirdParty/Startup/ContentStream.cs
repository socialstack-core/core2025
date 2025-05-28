using Api.Contexts;
using Api.Database;
using Api.Permissions;
using Api.SocketServerLibrary;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Startup;

/// <summary>
/// A source for a content stream.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="ID"></typeparam>
public interface ContentStreamSource<T, ID>
where T: Content<ID>, new()
where ID : struct, IEquatable<ID>, IComparable<ID>, IConvertible
{
	/// <summary>
	/// Starts cycling results for the given filter with the given callback function. Usually use Where and then one if its convenience functions instead.
	/// </summary>
	ValueTask<int> GetResults(Context context, Filter<T, ID> filter, Func<Context, T, int, object, object, ValueTask> onResult, object srcA, object srcB);
}

/// <summary>
/// A list of objects in the API. This is only used for representation of the 
/// result set format: it is not actually instanced.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiList<T> {
	/// <summary>
	/// The result set.
	/// </summary>
	public List<T> Results { get; set; }
}

/// <summary>
/// A singular object from the API. This is only used for representation of the 
/// result format: it is not actually instanced.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiContent<T> {
	/// <summary>
	/// The singular result.
	/// </summary>
	public T Result { get; set; }
}

/// <summary>
/// Streams content from a service in a non-allocating way.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="ID"></typeparam>
public struct ContentStream<T, ID>
	where T: Content<ID>, new()
	where ID : struct, IEquatable<ID>, IComparable<ID>, IConvertible
{
	/// <summary>
	/// The source service.
	/// </summary>
	public ContentStreamSource<T, ID> Source;

	/// <summary>
	/// A filter to use.
	/// </summary>
	public Filter<T, ID> Filter;

	/// <summary>
	/// The autoservice for type T. Frequently == Source.
	/// </summary>
	public AutoService<T, ID> ServiceForType;

	/// <summary>
	/// True if the filter must be released after streaming. Happens internally.
	/// </summary>
	public bool ReleaseFilter;

	/// <summary>
	/// True if the total should be included.
	/// </summary>
	public bool IncludeTotal;

	/// <summary>
	/// A default content stream.
	/// </summary>
	public ContentStream()
	{
		
	}

	/// <summary>
	/// Convenience wrapper for creating a stream from a list.
	/// Not really ideal because the goal of a content stream is to avoid allocation of 
	/// lists, but there are plenty of times when a quick list is perfectly fine.
	/// </summary>
	/// <param name="list"></param>
	/// <param name="svc">Provide the AutoService for type T.</param>
	public ContentStream(List<T> list, AutoService<T, ID> svc)
	{
		Source = new ListStreamSource<T, ID>(list);
		ServiceForType = svc;
	}

	/// <summary>
	/// Starts streaming the results, invoking the given callback on each run.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="onResult"></param>
	/// <param name="srcA">An object that you can pass through to the callback without needing to heap allocate a delegate state.</param>
	/// <param name="srcB">A second object that you can pass through to the callback without needing to heap allocate a delegate state.</param>
	/// <returns></returns>
	public async ValueTask Stream(Context context, Func<Context, T, int, object, object, ValueTask> onResult, object srcA, object srcB)
	{
		await Source.GetResults(context, Filter, onResult, srcA, srcB);
		if (ReleaseFilter && Filter != null)
		{
			Filter.Release();
		}
	}

	/// <summary>
	/// Writes this content stream to a given actual stream as JSON.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="stream"></param>
	/// <param name="includes"></param>
	/// <returns></returns>
	public async ValueTask WriteJson(Context context, System.IO.Stream stream, string includes)
	{
		var writer = Writer.GetPooled();
		writer.Start(null);

		ContentStreamSource<T, ID> src = Source;

		await ServiceForType.ToJson(context, Filter, async (Context ctx, Filter<T, ID> filt, Func<T, int, ValueTask> onResult) => {

			return await src.GetResults(ctx, filt, async (Context ctx2, T result, int index, object srcA, object srcB) => {
				var _onResult = srcA as Func<T, int, ValueTask>;
				await _onResult(result, index);
			}, onResult, null);

		}, writer, stream, includes, Filter == null ? false : Filter.IncludeTotal);

		if (ReleaseFilter && Filter != null)
		{
			Filter.Release();
		}

		writer.Release();
	}

}

/// <summary>
/// A convenience wrapper for streaming lists of content types.
/// Best avoided - use the mainline service output instead as it avoids allocating the list itself.
/// For quick wins or minorly used endpoints though, this is completely fine.
/// </summary>
public class ListStreamSource<T, ID> : ContentStreamSource<T, ID>
	where T: Content<ID>, new()
	where ID : struct, IEquatable<ID>, IComparable<ID>, IConvertible
{
	/// <summary>
	/// The list in this source.
	/// </summary>
	public List<T> Content;
	
	/// <summary>
	/// Creates a new list source.
	/// </summary>
	/// <param name="content"></param>
	public ListStreamSource(List<T> content)
	{
		Content = content;
	}

	/// <summary>
	/// Starts streaming the results, invoking the given callback on each one.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="filter"></param>
	/// <param name="onResult"></param>
	/// <param name="srcA"></param>
	/// <param name="srcB"></param>
	/// <returns></returns>
	public async ValueTask<int> GetResults(Context context, Filter<T, ID> filter, Func<Context, T, int, object, object, ValueTask> onResult, object srcA, object srcB)
	{
		if(Content == null)
		{
			return 0;
		}
		
		for(var i=0;i<Content.Count;i++)
		{
			await onResult(context, Content[i], i, srcA, srcB);
		}

		return Content.Count;
	}

}
