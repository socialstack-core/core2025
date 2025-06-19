﻿using Api.Contexts;
using Api.Database;
using Api.Permissions;
using Api.Startup;
using Api.Users;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Api.SocketServerLibrary;
using System.IO;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json.Linq;

/// <summary>
/// A general use service which manipulates an entity type. In the global namespace due to its common use.
/// Deletes, creates, lists and updates them whilst also firing off a series of events.
/// Note that you don't have to inherit this to create a service - it's just for convenience for common functionality.
/// Services are actually detected purely by name.
/// </summary>
public partial class AutoService<T, ID>
{

	/// <summary>
	/// {"result": 
	/// </summary>
	private static readonly byte[] ResultHeader = new byte[] { (byte)'{', (byte)'"', (byte)'r', (byte)'e', (byte)'s', (byte)'u', (byte)'l', (byte)'t', (byte)'"', (byte)':' };

	/// <summary>
	/// {"total": 
	/// </summary>
	private static readonly byte[] TotalHeader = new byte[] { (byte)']', (byte)',', (byte)'"', (byte)'t', (byte)'o', (byte)'t', (byte)'a', (byte)'l', (byte)'"', (byte)':' };

	/// <summary>
	/// ,"results": (comes after total)
	/// </summary>
	private static readonly byte[] ResultsHeaderAfterTotal = new byte[] { (byte)',', (byte)'"', (byte)'r', (byte)'e', (byte)'s', (byte)'u', (byte)'l', (byte)'t', (byte)'s', (byte)'"', (byte)':', (byte)'[' };

	/// <summary>
	/// {"results": (no total)
	/// </summary>
	private static readonly byte[] ResultsHeader = new byte[] { (byte)'{', (byte)'"', (byte)'r', (byte)'e', (byte)'s', (byte)'u', (byte)'l', (byte)'t', (byte)'s', (byte)'"', (byte)':', (byte)'[' };

	/// <summary>
	/// ]}
	/// </summary>
	private static readonly byte[] ResultsFooter = new byte[] { (byte)']', (byte)'}' };

	/// <summary>
	/// "null"
	/// </summary>
	private static readonly byte[] NullText = new byte[] { (byte)'n', (byte)'u', (byte)'l', (byte)'l' };

	/// <summary>
	/// ,"includes":[ 
	/// </summary>
	private static readonly byte[] IncludesHeader = new byte[] {
		(byte)',', (byte)'"', (byte)'i', (byte)'n', (byte)'c', (byte)'l', (byte)'u', (byte)'d', (byte)'e', (byte)'s', (byte)'"', (byte)':', (byte)'['
	};

	private static readonly byte[] IncludesFooter = new byte[] { (byte)']', (byte)'}' };

	/// <summary>
	/// End of include block. ]}.
	/// </summary>
	private static readonly byte[] IncludesValueFooter = new byte[] { (byte)']', (byte)'}' };
	
	/// <summary>
	/// End of dynamic include block. }}.
	/// </summary>
	private static readonly byte[] IncludesDynamicValueFooter = new byte[] { (byte)'}', (byte)'}' };

	/// <summary>
	/// Serialises results from this service with the requested filter. 
	/// Allocates the result as a string. Use sparingly to avoid unnecessary allocations.
	/// </summary>
	public async ValueTask<string> ToJson(Context context, Filter<T,ID> filter, string includes = null)
	{
		var writer = Writer.GetPooled();
		writer.Start(null);
		await ToJson(context, filter, async (Context ctx, Filter<T, ID> filt, Func<T, int, ValueTask> onResult) => {

			return await GetResults(ctx, filt, async (Context ctx2, T result, int index, object src, object srcB) => {
				var _onResult = src as Func<T, int, ValueTask>;
				await _onResult(result, index);
			}, onResult, null);

		}, writer, null, includes, filter.IncludeTotal);
		var jsonString = writer.ToUTF8String();
		writer.Release();
		return jsonString;
	}

	/// <summary>
	/// Loads the given stored JSON string as an entity of this type. 
	/// Does not currently handle any list included fields, 
	/// but note that as it's stored JSON it can and does include private field values.
	/// </summary>
	/// <param name="json"></param>
	/// <returns></returns>
	public T FromStoredJson(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			return null;
		}
		
		var baseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);

		if (baseObj == null)
		{
			return null;
		}

		// Get the field set:
		var fields = GetContentFields();

		var newResult = (T)Activator.CreateInstance(InstanceType);

		foreach (var val in baseObj.Properties())
		{
			if(!fields.TryGetValue(val.Name, out var field))
			{
				continue;
			}

			if (field.FieldInfo == null)
			{
				// Fields only.
				continue;
			}

			// Set field from val.Value
			#warning loading stored documents such as revisions is incomplete here
			throw new Exception("properties are not yet loaded from stored docs (revisions)");
		}

		return newResult;
	}

	private TypeDocumentReaderWriter<T> _storedJsonIO;

	/// <summary>
	/// Serialises an object to a JSON string equiv to what may be stored in a database.
	/// I.e. fields only and excludes the "result" wrapper. Used when e.g. creating revisions.
	/// </summary>
	/// <param name="entity"></param>
	/// <returns></returns>
	public ValueTask<string> ToStoredJson(T entity)
	{
		// Get the field set:
		var fields = GetContentFields();
		Writer writer = Writer.GetPooled();
		writer.Start(null);

		var io = _storedJsonIO;

		if (io == null || io.Fields != fields)
		{
			io = TypeIOEngine.GenerateDocumentReaderWriter<T, ID>(fields);
			_storedJsonIO = io;
		}

		io.WriteStoredJson(entity, writer);

		var str = writer.ToUTF8String();
		writer.Release();
		return new ValueTask<string>(str);
	}

	/// <summary>
	/// Serialises an object from this service to a JSON string.
	/// Allocates the result as a string. Use sparingly to avoid unnecessary allocations.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="entity"></param>
	/// <param name="includes"></param>
	/// <returns></returns>
	public async ValueTask<string> ToJson(Context context, T entity, string includes = null)
	{
		var writer = Writer.GetPooled();
		writer.Start(null);
		await ToJson(context, entity, writer, null, includes, false);
		var jsonString = writer.ToUTF8String();
		writer.Release();
		return jsonString;
	}

	/// <summary>
	/// Serialises the given object into the given stream (usually a response stream). By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// dataSource is often a filter.
	/// </summary>
	public async ValueTask ToJson<ANY>(
		Context context, ANY dataSource,
		Func<Context, ANY, Func<T, int, ValueTask>, ValueTask<int>> onGetData,
		Writer writer,
		Stream targetStream = null, string includes = null, bool includeTotal = false, bool leaveOpen = false)
	{
		// Get the json structure:
		var jsonStructure = await GetTypedJsonStructure(context);

		if (jsonStructure.TypeIO == null)
		{
			jsonStructure.TypeIO = TypeIOEngine.Generate(jsonStructure, this, context);
		}

		// Get the include set (can be null). Must happen first such that if it errors, nothing was written out to the stream.
		var includeSet = GetContentFields().GetIncludeSet(includes);

		writer.Write(ResultsHeader, 0, 12);

		// Obtain ID collectors, and then collect the IDs.
		IDCollector firstCollector;
		FunctionalInclusionNode[] functionalIncludes;

		if (includeSet == null)
		{
			firstCollector = null;
			functionalIncludes = null;
		}
		else
		{
			firstCollector = includeSet.RootInclude.GetCollectors();
			functionalIncludes = includeSet.RootInclude.FunctionalIncludes;
		}

		var total = await onGetData(context, dataSource, async (T entity, int index) =>
		{
			if (index != 0)
			{
				writer.Write((byte)',');
			}

			if (entity == null)
			{
				writer.Write(NullText, 0, 4);
			}
			else
			{
				jsonStructure.TypeIO.WriteJsonUnclosed(entity, writer, context, false);

				// Execute any functional includes on the root node.
				if (functionalIncludes != null)
				{
					for (var i = 0; i < functionalIncludes.Length; i++)
					{
						var fi = functionalIncludes[i];
						var valueGen = fi.ValueGenerator as VirtualFieldValueGenerator<T, ID>;

						if (valueGen != null)
						{
							// ,"propertyName":
							writer.Write(fi._jsonPropertyHeader, 0, fi._jsonPropertyHeader.Length);

							// value:
							await valueGen.GetValue(context, entity, writer);
						}
					}
				}

				// Collect IDs from it:
				var current = firstCollector;

				while (current != null)
				{
					current.WriteAndCollect(context, writer, entity);
					current = current.NextCollector;
				}

				writer.Write((byte)'}');
			}

			if (targetStream != null)
			{
				// Flush after each one:
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}
		});

		if (includeSet == null)
		{
			if (includeTotal)
			{
				writer.Write(TotalHeader, 0, 10);
				writer.WriteS(total);

				if (!leaveOpen)
				{
					writer.Write((byte)'}');
				}
			}
			else if (leaveOpen)
			{
				writer.Write((byte)']');
			}
			else
			{
				// ]}
				writer.Write(ResultsFooter, 0, 2);
			}

			if (targetStream != null)
			{
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}
		}
		else
		{
			// We've got some includes to add.
			// Write the includes header, then write out the data so far.

			if (includeTotal)
			{
				writer.Write(TotalHeader, 0, 10);
				writer.WriteS(total);
			}
			else
			{
				writer.Write((byte)']');
			}

			// Starts with a ,
			writer.Write(IncludesHeader, 0, 13);

			if (targetStream != null)
			{
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}

			// Execute all inclusions (internally releases the collectors):
			await ExecuteIncludes(context, targetStream, writer, firstCollector, includeSet.RootInclude);

			if (leaveOpen)
			{
				writer.Write((byte)']');
			}
			else
			{
				// ]}
				writer.Write(IncludesFooter, 0, 2);
			}

			if (targetStream != null)
			{
				// Copy remaining bits:
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}
		}
	}

	/// <summary>
	/// Outputs the given object (an entity from this service) to JSON in the given writer.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="entity"></param>
	/// <param name="writer"></param>
	/// <param name="targetStream"></param>
	/// <param name="includes"></param>
	/// <returns></returns>
	public override ValueTask ObjectToJson(Context context, object entity, Writer writer, Stream targetStream = null, string includes = null)
	{
		return ToJson(context, (T)entity, writer, targetStream, includes);
	}

	/// <summary>
	/// Outputs the given object (an entity from this service) to JSON in the given writer.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="entity"></param>
	/// <param name="writer"></param>
	/// <returns></returns>
	public override async ValueTask ObjectToTypeAndIdJson(Context context, object entity, Writer writer)
	{
		// Get the json structure:
		var jsonStructure = await GetTypedJsonStructure(context);

		if (jsonStructure.TypeIO == null)
		{
			jsonStructure.TypeIO = TypeIOEngine.Generate(jsonStructure, this, context);
		}

		jsonStructure.TypeIO.WriteJsonPartial((T)entity, writer);
	}

	/// <summary>
	/// Serialises the given object list into the given stream (usually a response stream). By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// addResultWrap will wrap the object with {"result":...}. It is assumed true if includes is not null.
	/// </summary>
	public async ValueTask ToJson(Context context, IEnumerable<T> entities, Writer writer, Stream targetStream = null, string includes = null, bool includeTotal = false)
	{
		await ToJson(context, null, async (Context ctx, Filter<T, ID> filt, Func<T, int, ValueTask> onResult) => {

			var index = 0;

			if (entities == null)
			{
				return 0;
			}

			foreach (var entity in entities)
			{
				await onResult(entity, index);
				index++;
			}

			return index;

		}, writer, targetStream, includes, includeTotal);
	}

	/// <summary>
	/// Serialises the given object into the given stream (usually a response stream). By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// </summary>
	public async ValueTask ToJson(Context context, T entity, Stream targetStream, string includes)
	{
		var writer = Writer.GetPooled();
		writer.Start(null);
		await ToJson(context, entity, writer, targetStream, includes, true);
		writer.Release();
	}

	/// <summary>
	/// Serialises the given object into the given stream (usually a response stream). By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// addResultWrap will wrap the object with {"result":...}. It is assumed true if includes is not null.
	/// </summary>
	public async ValueTask ToJson(Context context, T entity, Writer writer, Stream targetStream = null, string includes = null, bool addResultWrap = true)
	{
		// Get the json structure:
		var jsonStructure = await GetTypedJsonStructure(context);

		if (jsonStructure.TypeIO == null)
		{
			jsonStructure.TypeIO = TypeIOEngine.Generate(jsonStructure, this, context);
		}

		// Get the include set (can be null):
		var includeSet = GetContentFields().GetIncludeSet(includes);

		if (addResultWrap)
		{
			writer.Write(ResultHeader, 0, 10);
		}

		IDCollector firstCollector = null;

		if (entity == null)
		{
			writer.Write(NullText, 0, 4);
		}
		else
		{
			jsonStructure.TypeIO.WriteJsonUnclosed(entity, writer, context, false);

			// Execute any functional includes on the root node.
			FunctionalInclusionNode[] functionalIncludes = (includeSet == null) ? null : includeSet.RootInclude.FunctionalIncludes;

			if (functionalIncludes != null)
			{
				for (var i = 0; i < functionalIncludes.Length; i++)
				{
					var fi = functionalIncludes[i];
					var valueGen = fi.ValueGenerator as VirtualFieldValueGenerator<T, ID>;

					if (valueGen != null)
					{
						// ,"propertyName":
						writer.Write(fi._jsonPropertyHeader, 0, fi._jsonPropertyHeader.Length);

						// value:
						await valueGen.GetValue(context, entity, writer);
					}
				}
			}

			if (includeSet != null)
			{
				// First we need to obtain ID collectors, and then collect the IDs.
				firstCollector = includeSet.RootInclude.GetCollectors();

				var current = firstCollector;

				while (current != null)
				{
					current.WriteAndCollect(context, writer, entity);
					current = current.NextCollector;
				}
			}

			writer.Write((byte)'}');
		}

		if (includeSet == null)
		{
			if (addResultWrap)
			{
				writer.Write((byte)'}');
			}

			if (targetStream != null)
			{
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}
		}
		else
		{
			// We've got some includes to add.
			// Write the includes header, then write out the data so far.
			writer.Write(IncludesHeader, 0, 13);

			if (targetStream != null)
			{
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}

			if (firstCollector != null)
			{
				await ExecuteIncludes(context, targetStream, writer, firstCollector, includeSet.RootInclude);
			}

			if (addResultWrap)
			{
				writer.Write(IncludesFooter, 0, 2);
			}
			else
			{
				writer.Write((byte)']');
			}

			if (targetStream != null)
			{
				// Copy remaining bits:
				await writer.CopyToAsync(targetStream);
				writer.Reset(null);
			}
		}
	}

	/// <summary>
	/// Used to execute includes.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="targetStream"></param>
	/// <param name="writer"></param>
	/// <param name="firstCollector"></param>
	/// <param name="includeNode"></param>
	/// <returns></returns>
	public async ValueTask ExecuteIncludes(Context context, Stream targetStream, Writer writer, IDCollector firstCollector, InclusionNode includeNode)
	{
		// Now all IDs that are needed have been collected,
		// go through the inclusions and perform the include.
		var includesToExecute = includeNode.ChildNodes;

		for (var i = 0; i < includesToExecute.Length; i++)
		{
			var toExecute = includesToExecute[i];

			if (toExecute.InclusionOutputIndex != 0)
			{
				// Comma between includes. Exists for all nodes except the very first include (output index 0).
				writer.Write((byte)',');
			}

			// Write the inclusion node header:
			var h = toExecute.IncludeHeader;
			writer.Write(h, 0, h.Length);

			// Get ID collector:
			var collector = firstCollector;
			var curIndex = 0;

			// A linked list is by far the best structure here - the set is usually tiny and it avoids allocating.
			while (curIndex < toExecute.CollectorIndex)
			{
				curIndex++;
				collector = collector.NextCollector;
			}

			// Spawn child collectors now, if we need any.
			var childCollectors = toExecute.GetCollectors();

			if (toExecute.TypeSource != null)
			{
				// The collector in this case is a MultiIdCollector.
				// Ask each service in it (which can be none) to output a JSON list.
				var multiCollector = collector as MultiIdCollector;

				if (multiCollector != null)
				{
					for (var n = 0; n < multiCollector.CollectorFill; n++)
					{
						var cbt = multiCollector.CollectorsByType[n];
						if (n == 0)
						{
							writer.Write((byte)'\"');
						}
						else
						{
							writer.WriteASCII(",\"");
						}
						writer.WriteASCII(cbt.Service.EntityName);

						writer.WriteASCII("\":{\"results\":[");

						// Load the sub-include set:
						if (toExecute.DynamicChildIncludes != null)
						{
							var childIncludeSet = cbt.Service.GetContentFields().GetIncludeSet(toExecute.DynamicChildIncludes);

							if (childIncludeSet != null)
							{
								// We've got some includes to add.

								// First we need to obtain ID collectors, and then collect the IDs.
								var childRoot = childIncludeSet.RootInclude;

								var firstChildCollector = childRoot.GetCollectors();

								await cbt.Service.OutputJsonList(
									context,
									firstChildCollector,
									cbt.Collector,
									writer, true,
									childIncludeSet.RootInclude.FunctionalIncludes
								);

								writer.Write((byte)']');
								// Write the includes header, then write out the data so far.
								writer.Write(IncludesHeader, 0, 13);

								if (childRoot.ChildNodes != null && childRoot.ChildNodes.Length > 0)
								{
									// NB: This will release the child collectors for us.
									await ExecuteIncludes(context, targetStream, writer, firstChildCollector, childRoot);
								}

								writer.Write(IncludesFooter, 0, 2);
							}
							else
							{
								await cbt.Service.OutputJsonList(context, null, cbt.Collector, writer, true, null);
								writer.Write(IncludesValueFooter, 0, 2);
							}
						}
						else
						{
							await cbt.Service.OutputJsonList(context, null, cbt.Collector, writer, true, null);
							writer.Write(IncludesValueFooter, 0, 2);
						}
					}   
				}
				
				// End of this include.
				writer.Write(IncludesDynamicValueFooter, 0, 2);

			}
			else
			{
				// Directly use IDs in collector with the service.
				await toExecute.Service.OutputJsonList(context, childCollectors, collector, writer, true, toExecute.FunctionalIncludes);

				// End of this include.
				writer.Write(IncludesValueFooter, 0, 2);
			}

			// Did it have any child nodes? If so, execute those as well.
			// Above we will have collected the IDs that the children need.
			if (toExecute.ChildNodes != null && toExecute.ChildNodes.Length > 0)
			{
				// NB: This will release the child collectors for us.
				await ExecuteIncludes(context, targetStream, writer, childCollectors, toExecute);
			}
		}

		// Release the collectors:
		var current = firstCollector;

		while (current != null)
		{
			var next = current.NextCollector;
			current.Release();
			current = next;
		}
	}

	/// <summary>
	/// Serialises the given object into the given stream (usually a response stream). By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// addResultWrap will wrap the object with {"result":...}. It is assumed true if includes is not null.
	/// </summary>
	public async ValueTask ToJson(Context context, T entity, Writer writer, InclusionNode node)
	{
		// Get the json structure:
		var jsonStructure = await GetTypedJsonStructure(context);

		if (jsonStructure.TypeIO == null)
		{
			jsonStructure.TypeIO = TypeIOEngine.Generate(jsonStructure, this, context);
		}

		// Write the object out:
		if (entity == null)
		{
			writer.Write(NullText, 0, 4);
		}
		else
		{
			jsonStructure.TypeIO.WriteJsonUnclosed(entity, writer, context, false);

			FunctionalInclusionNode[] functionalIncludes = (node == null) ? null : node.FunctionalIncludes;

			if (functionalIncludes != null)
			{
				for (var i = 0; i < functionalIncludes.Length; i++)
				{
					var fi = functionalIncludes[i];
					var valueGen = fi.ValueGenerator as VirtualFieldValueGenerator<T, ID>;

					if (valueGen != null)
					{
						// ,"propertyName":
						writer.Write(fi._jsonPropertyHeader, 0, fi._jsonPropertyHeader.Length);

						// value:
						await valueGen.GetValue(context, entity, writer);
					}
				}
			}

			writer.Write((byte)'}');
		}
	}

	/// <summary>
	/// Outputs a single object from this service as JSON into the given writer. Acts like include * was specified.
	/// Executes the given collector(s) whilst it happens, which can also be null.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="id"></param>
	/// <param name="writer"></param>
	/// <param name="includes">The includes to use when outputting the JSON</param>
	/// <returns></returns>
	public override async ValueTask OutputById(Context context, ulong id, Writer writer, string includes = "*")
	{
		// Get the object:
		var content = await Get(context, ConvertId(id), DataOptions.Default);

		// Output it:
		await ToJson(context, content, writer, null, includes);
	}
		
	/// <summary>
	/// Outputs a list of things from this service as JSON into the given writer.
	/// Executes the given collector(s) whilst it happens, which can also be null.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="collectors"></param>
	/// <param name="idSet"></param>
	/// <param name="writer"></param>
	/// <param name="viaIncludes"></param>
	/// <param name="functionalIncludes"></param>
	/// <returns></returns>
	public override async ValueTask OutputJsonList(Context context, IDCollector collectors, IDCollector idSet, Writer writer, bool viaIncludes,
		FunctionalInclusionNode[] functionalIncludes = null)
	{
		var collectedIds = idSet as LongIDCollector;

		var cache = GetCache();

		// If cached, directly enumerate over the IDs via the cache.
		if (cache != null)
		{
			var idIndex = cache.GetIdIndex();

			// This mapping type is cached.
			var _enum = collectedIds.GetNonAllocEnumerator();
			var first = true;
			while (_enum.HasMore())
			{
				// Get current value:
				var valID = _enum.Current();
				// Read that ID set from the cache:
				if (idIndex.TryGetValue(ConvertId(valID), out T entity))
				{
					if (first)
					{
						first = false;
					}
					else
					{
						writer.Write((byte)',');
					}

					// Output the object:
					await ToJsonWithIncludes(context, entity, writer, functionalIncludes, collectors);
				}
			}

		}
		else if (collectedIds.Count > 0)
		{
			// DB hit.
			var f = Where("Id=[?]", DataOptions.IgnorePermissions)
			.Bind(collectedIds);
			f.IsIncluded = viaIncludes;

			await f.ListAll(
				context,
				async (Context ctx, T entity, int index, object src, object src2) =>
				{

					// Passing these in avoids a delegate frame allocation.
					// The casts are free because they're reference types.
					var _writer = (Writer)src;
					var _cols = (IDCollector)src2;

					if (index != 0)
					{
						_writer.Write((byte)',');
					}

					// Output the object:
					await ToJsonWithIncludes(context, entity, _writer, functionalIncludes, _cols);
				},
				writer,
				collectors
			);
		}
	}

	/// <summary>
	/// Serialises the given object into the given writer. By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// </summary>
	public async ValueTask ToJson(Context context, T entity, Writer writer)
	{
		// Get the json structure:
		var jsonStructure = await GetTypedJsonStructure(context);

		if (jsonStructure.TypeIO == null)
		{
			jsonStructure.TypeIO = TypeIOEngine.Generate(jsonStructure, this, context);
		}

		if (entity == null)
		{
			writer.Write(NullText, 0, 4);
		}
		else
		{
			jsonStructure.TypeIO.WriteJsonUnclosed(entity, writer, context, false);
			writer.Write((byte)'}');
		}
	}

	/// <summary>
	/// Serialises the given object into the given writer. By using this method, it will consider the fields a user is permitted to see (based on the role in the context)
	/// and also may use a per-object cache which contains string segments.
	/// </summary>
	public async ValueTask ToJsonWithIncludes(Context context, T entity, Writer writer, FunctionalInclusionNode[] functionalIncludes, IDCollector collectors)
	{
		// Get the json structure:
		var jsonStructure = await GetTypedJsonStructure(context);

		if (jsonStructure.TypeIO == null)
		{
			jsonStructure.TypeIO = TypeIOEngine.Generate(jsonStructure, this, context);
		}

		if (entity == null)
		{
			writer.Write(NullText, 0, 4);
			return;
		}

		jsonStructure.TypeIO.WriteJsonUnclosed(entity, writer, context, false);

		if (functionalIncludes != null)
		{
			for (var i = 0; i < functionalIncludes.Length; i++)
			{
				var fi = functionalIncludes[i];
				var valueGen = fi.ValueGenerator as VirtualFieldValueGenerator<T, ID>;

				if (valueGen != null)
				{
					// ,"propertyName":
					writer.Write(fi._jsonPropertyHeader, 0, fi._jsonPropertyHeader.Length);

					// value:
					await valueGen.GetValue(context, entity, writer);
				}
			}
		}

		// Collect:
		var col = collectors;

		while (col != null)
		{
			col.WriteAndCollect(context, writer, entity);
			col = col.NextCollector;
		}

		writer.Write((byte)'}');
	}
}