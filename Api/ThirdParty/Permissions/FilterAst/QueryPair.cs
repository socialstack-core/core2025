using Api.Contexts;
using Api.Database;
using Api.SocketServerLibrary;
using Api.Startup;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api.Permissions
{

	/// <summary>
	/// A non-allocating mechanism for obtaining a list of things from a service.
	/// </summary>
	public struct QueryPair<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Primary user provided query. Will be effectively AND-ed with QueryB.
		/// </summary>
		public Filter<T,ID> QueryA;

		/// <summary>
		/// Secondary query, pre-parsed. Originates from the permission system, and is not null.
		/// </summary>
		public Filter<T,ID> QueryB;

		/// <summary>
		/// Total result count, when available.
		/// </summary>
		public int Total;

		/// <summary>
		/// True if anything has handled the request.
		/// </summary>
		public bool Handled;

		/// <summary>
		/// Callback when the queries get a result
		/// </summary>
		public Func<Context, T, int, object, object, ValueTask> OnResult;

		/// <summary>
		/// Source a object.
		/// </summary>
		public object SrcA;

		/// <summary>
		/// Source b object.
		/// </summary>
		public object SrcB;
	}
	
	/// <summary>
	/// Fast filter metadata.
	/// </summary>
	public class FilterMeta<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		private string _query;
		/// <summary>
		/// The service this filter is for
		/// </summary>
		public AutoService<T, ID> Service;
		private bool _allowConstants;
		/// <summary>
		/// The constructed filter type to use.
		/// </summary>
		private Type _constructedType;
		
		/// <summary>
		/// The AST for this filter.
		/// </summary>
		public FilterAst<T, ID> Ast;

		/// <summary>
		/// True if the filter has a rooted On(..) statement. It can only be a child of an AND statement.
		/// </summary>
		public bool HasRootedOn;
		
		/// <summary>
		/// Creates filter metadata for the given query pair.
		/// </summary>
		public FilterMeta(AutoService<T, ID> service, string query, bool allowConstants = false){
			_query = query;
			Service = service;
			_allowConstants = allowConstants;
		}

		/// <summary>
		/// The original query string.
		/// </summary>
		public string Query => _query;

		/// <summary>
		/// Type info for the args.
		/// </summary>
		public List<ArgBinding> ArgTypes;

		/// <summary>
		/// Parses the queries and constructs the filters now.
		/// </summary>
		public void Construct()
		{
			var tree = FilterAst.Parse(Service, _query, _allowConstants, !_allowConstants);
			Ast = tree;

			if (tree == null)
			{
				// No actual filter. It's effectively just a base "list everything".
				return;
			}

			// Build the type:
			_constructedType = tree.ConstructType();

			if (tree.Root != null)
			{
				HasRootedOn = tree.Root.HasRootedOnStatement();
			}
			
			ArgTypes = tree.Args;

			for (var i = 0; i < ArgTypes.Count; i++)
			{
				var argType = ArgTypes[i];
				var field = _constructedType.GetField("Arg_" + i);
				argType.ConstructedField = field;

				if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					argType.ArrayOf = field.FieldType.GetGenericArguments()[0];
				}
			}
		}

		/// <summary>
		/// Get a pooled filter instance.
		/// </summary>
		/// <returns></returns>
		public Filter<T,ID> GetPooled()
		{
			Filter<T, ID> f;

			if (_constructedType == null)
			{
				// Just a base Filter<T, ID> - it has no args etc.
				f = new Filter<T, ID>();
				f.Empty = true;
			}
			else
			{
				f = Activator.CreateInstance(_constructedType) as Filter<T, ID>;
				f.Empty = false;
			}

			f.IsIncluded = HasRootedOn;
			f.Pool = this;
			return f;
		}
		
	}

	/// <summary>
	/// A filter. Use the concrete-type variant as much as possible.
	/// </summary>
	public partial class FilterBase
	{
		/// <summary>
		/// Results per page. If 0, there's no limitation.
		/// </summary>
		public int PageSize;
		/// <summary>
		/// 0 based starting offset (In number of records).
		/// </summary>
		public int Offset;

		/// <summary>
		/// True if the total # of results should be included. Results in potentially large scans.
		/// </summary>
		public bool IncludeTotal = false;

		/// <summary>
		/// The field to sort by. Must be a field (can't be a virtual or property).
		/// </summary>
		public ContentField SortField;
		/// <summary>
		/// True if this should sort ascending.
		/// </summary>
		public bool SortAscending = true;

		/// <summary>
		/// Errors when a null is given for a non-nullable field.
		/// </summary>
		public void NullCheck(string s)
		{
			if (s == null)
			{
				throw new PublicException("Attempted to use a null as an arg for a non-nullable field. Did you mean to use something else?", "filter_invalid");
			}
		}

		/// <summary>
		/// Return to pool it came from
		/// </summary>
		public virtual void Release()
		{
			
		}

		/// <summary>
		/// Gets the set of argument types for this filter. Can be null if there are none.
		/// </summary>
		/// <returns></returns>
		public virtual List<ArgBinding> GetArgTypes()
		{
			return null;
		}

		/// <summary>
		/// Builds this filter node as a query string, writing it into the given string builder.
		/// If a variable is outputted then a value reader is pushed in the given arg set.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="localeCode">Optional localeCode used when a request is for e.g. French fields instead. 
		/// It would be e.g. "fr" and just matches whatever your Locale.Code is.</param>
		public void BuildOrderLimitQuery(Writer builder, string localeCode)
		{
			if (SortField != null)
			{
				builder.WriteS(" ORDER BY ");
				builder.Write((byte)'`');
				builder.WriteS(SortField.FieldInfo.Name);

				if (SortField.Localised && localeCode != null)
				{
					builder.WriteASCII("`.`");
					builder.WriteS(localeCode);
				}

				builder.WriteS("` ");
				builder.WriteS(SortAscending ? "asc" : "desc");
			}

			if (PageSize != 0)
			{
				builder.WriteS(" LIMIT ");
				builder.WriteS(Offset);
				builder.Write((byte)',');
				builder.WriteS(PageSize);
			}
			else if (Offset != 0)
			{
				// Just being used as an offset.
				builder.WriteS(" LIMIT ");
				builder.WriteS(Offset);
			}
		}

		/// <summary>
		/// use this to paginate (or restrict result counts) for large filters.
		/// </summary>
		/// <param name="pageIndex">0 based page index.</param>
		/// <param name="pageSize">The amount of results per page, or 50 if not specified. 
		/// If you specifically set this to 0, pageIndex acts like an offset (i.e. 10 meaning skip the first 10 results).</param>
		public FilterBase SetPage(int pageIndex, int pageSize = 50)
		{
			Offset = pageSize == 0 ? pageIndex : pageIndex * pageSize;
			PageSize = pageSize;
			return this;
		}

		/// <summary>
		/// Sort this filter by the given field name from the filters default type.
		/// </summary>
		/// <param name="fieldName">Name of field to sort by.</param>
		/// <param name="ascending">True if should sort in ascending order (true is default).</param>
		public FilterBase Sort(string fieldName, bool ascending = true)
		{
			SortAscending = ascending;
			SortField = GetField(fieldName);
			return this;
		}

		/// <summary>
		/// Gets a field in the type by its textual name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected virtual ContentField GetField(string name)
		{
			return null;
		}

		/// <summary>
		/// Test if the given object passes this filter.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="value"></param>
		/// <param name="isIncluded">True if the match is taking place within an inclusion context.</param>
		/// <returns></returns>
		public virtual bool Match(Context context, object value, bool isIncluded)
		{
			// No filter - pass by default.
			return true;
		}

		/// <summary>
		/// Gets the query for this filter.
		/// </summary>
		/// <returns></returns>
		public virtual string GetQuery()
		{
			return "";
		}

	}

	/// <summary>
	/// Fast precompiled non-allocating filter engine.
	/// </summary>
	public partial class Filter<T,ID> : FilterBase
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// True if we're in an inclusion context.
		/// </summary>
		public bool IsIncluded;
		/// <summary>
		/// The pool that the object came from.
		/// </summary>
		public FilterMeta<T, ID> Pool;

		/// <summary>
		/// Current arg offset.
		/// </summary>
		protected int _arg = 0;

		/// <summary>
		/// The type of the next arg to bind. Null if there are no more args.
		/// </summary>
		public Type NextBindType {
			get {
				if (_arg >= Pool.ArgTypes.Count)
				{
					return null;
				}
				return Pool.ArgTypes[_arg].ArgType;
			}
		}

		/// <summary>
		/// True if every arg has been bound.
		/// </summary>
		/// <returns></returns>
		public bool FullyBound()
		{
			if (Pool == null || Pool.ArgTypes == null)
			{
				return _arg == 0;
			}
			return _arg == Pool.ArgTypes.Count;
		}

		/// <summary>
		/// Return back to pool.
		/// </summary>
		public override void Release()
		{
			Reset();
		}

		/// <summary>
		/// True if this filter will always be true.
		/// </summary>
		public bool Empty;

		/// <summary>
		/// True if the given iterator has the given value in it
		/// </summary>
		/// <returns></returns>
		public static bool SetContains(List<ulong> values, ulong val)
		{
			foreach (var v in values)
			{
				if (val == v)
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// True if the given member value is present in the given enumerable.
		/// </summary>
		/// <returns></returns>
		public static bool MemberInSet<MEM_TYPE>(MEM_TYPE val, IEnumerable<MEM_TYPE> values)
			where MEM_TYPE : IEquatable<MEM_TYPE>
		{
			foreach (var v in values)
			{
				if (val.Equals(v))
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// True if the given iterator has any of the given values in it.
		/// </summary>
		/// <returns></returns>
		public static bool SetContainsAny(List<ulong> values, IEnumerable<ulong> anyOf)
		{
			foreach (var v in anyOf)
			{
				if (values != null && values.Contains(v))
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// True if the given iterator has any of the given values in it.
		/// </summary>
		/// <returns></returns>
		public static bool SetContainsAll(List<ulong> values, IEnumerable<ulong> anyOf)
		{
			foreach (var v in anyOf)
			{
				if (values == null || !values.Contains(v))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// True if the given iterator has the given value in it
		/// </summary>
		/// <returns></returns>
		public static bool SetEquals(List<ulong> values, IEnumerable<ulong> vals)
		{
			var max = values == null ? 0 : values.Count;
			var iterations = 0;

			foreach (var v in vals)
			{
				if (iterations >= max)
				{
					return false;
				}

				if (values[iterations] != v)
				{
					return false;
				}

				iterations++;
			}

			return iterations == max;
		}

		/// <summary>
		/// Gets the set of argument types for this filter. Can be null if there are none.
		/// </summary>
		/// <returns></returns>
		public override List<ArgBinding> GetArgTypes()
		{
			return Pool.ArgTypes;
		}
		
		/// <summary>
		/// Gets a field in the type by its textual name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected override ContentField GetField(string name)
		{
			if (Pool == null)
			{
				return null;
			}


			if (!Pool.Service.GetContentFields().NameMap.TryGetValue(name.ToLower(), out ContentField fld))
			{
				return null;
			}

			return fld;
		}

		/// <summary>
		/// Gets the query for this filter.
		/// </summary>
		/// <returns></returns>
		public override string GetQuery()
		{
			return Pool == null ? "" : Pool.Query;
		}

		/// <summary>
		/// Reset arg bind.
		/// </summary>
		public void Reset()
		{
			_arg = 0;
			Offset = 0;
			PageSize = 0;
			IncludeTotal = false;
			SortField = null;
			SortAscending = true;
		}

		/// <summary>
		/// Indicates a bind failure has happened.
		/// </summary>
		/// <param name="type"></param>
		public void Fail(Type type)
		{
			if (Pool == null)
			{
				throw new PublicException("Argument #" + _arg + " of this filter is not a '" + type.Name + "'.", "filter_invalid");
			}

			var max = Pool.ArgTypes == null ? 0 : Pool.ArgTypes.Count;

			if (_arg >= max)
			{
				throw new PublicException("Too many args being provided. This filter has " + max, "filter_invalid");
			}

			var arg = Pool.ArgTypes[_arg];

			var nullableBaseType = Nullable.GetUnderlyingType(arg.ArgType);

			string typeName;

			if (nullableBaseType == null)
			{
				typeName = arg.ArgType.Name;
			}
			else
			{
				typeName = "nullable: " + nullableBaseType.Name + "?";
			}

			throw new PublicException("Argument #" + _arg + " must be a '" + typeName + "', but you used Bind('" + type.Name + "') for it.", "filter_invalid");
		}

		/// <summary>
		/// Data options for this filter.
		/// </summary>
		public DataOptions DataOptions = DataOptions.Default;

		/// <summary>
		/// Execute this filter now, obtaining an allocated list of results. 
		/// Consider using the callback overload instead if you wish to avoid the list allocation.
		/// </summary>
		/// <returns></returns>
		public async ValueTask ListAll(Context context, Func<Context, T, int, object, object, ValueTask> cb, object srcA = null, object srcB = null)
		{
			await Pool.Service.GetResults(context, this, cb, srcA, srcB);
			Release();
		}

		/// <summary>
		/// Convenience function for getting a true if there are any results, or false if there were none.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async ValueTask<bool> Any(Context context)
		{
			var results = await ListAll(context);
			return results.Count > 0;
		}

		/// <summary>
		/// Convenience function for getting the first result, or null.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async ValueTask<T> First(Context context)
		{
			PageSize = 1;
			var results = await ListAll(context);
			return results.Count > 0 ? results[0] : null;
		}
		
		/// <summary>
		/// Convenience function for getting the last result, or null.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async ValueTask<T> Last(Context context)
		{
			var results = await ListAll(context);
			return results.Count > 0 ? results[results.Count - 1] : null;
		}

		/// <summary>
		/// Execute this filter now, obtaining a count using an allocated delegate.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<int> Count(Context context)
		{
			var count = 0;
			await ListAll(context, (Context ctx, T val, int index, object src, object src2) =>
			{
				count++;
				return new ValueTask();
			}, null);
			return count;
		}

		/// <summary>
		/// Execute this filter now, obtaining an allocated list of results. 
		/// Consider using the callback overload instead if you wish to avoid the list allocation.
		/// </summary>
		/// <returns></returns>
		public async ValueTask<List<T>> ListAll(Context context)
		{
			var results = new List<T>();
			await ListAll(context, (Context ctx, T val, int index, object src, object src2) =>
			{
				var list = src as List<T>;
				list.Add(val);
				return new ValueTask();
			}, results);
			return results;
		}
		
		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public Filter<T, ID> Bind(object v)
		{
			if (Pool.ArgTypes == null || _arg >= Pool.ArgTypes.Count)
			{
				Fail(v == null ? typeof(object) : v.GetType());
				return this;
			}

			var argInfo = Pool.ArgTypes[_arg];

			if (v == null)
			{
				// Is this field nullable?
				if (!argInfo.IsNullable)
				{
					throw new PublicException("Can't use null as arg #" + (_arg+1) + " because it's not nullable", "filter_invalid");
				}
			}
			else
			{
				var t = v.GetType();
				var argType = argInfo.ArgType;
				var nullableUnderlying = Nullable.GetUnderlyingType(argType);

				if (argInfo.ArrayOf != null)
				{
					if (argType.IsAssignableFrom(t))
					{
						// The value is directly usable as argTypes specific array type.
						argInfo.ConstructedField.SetValue(this, v);
						_arg++;
						return this;
					}
					else if (t == typeof(JArray))
					{
						var jArray = (JArray)v;

						if (argInfo.ArrayOf == typeof(string))
						{
							var nl = new List<string>();

							foreach (var item in jArray)
							{
								var token = item as JValue;

								if (token == null || token.Type == JTokenType.Null)
								{
									nl.Add(null);
								}
								else if (token.Type == JTokenType.String)
								{
									nl.Add(token.Value<string>());
								}
								else if (token.Type == JTokenType.Boolean)
								{
									nl.Add(token.Value<bool>() ? "true" : "false");
								}
								else if (token.Type == JTokenType.Float)
								{
									nl.Add(token.Value<float>().ToString());
								}
								else if (token.Type == JTokenType.Integer)
								{
									nl.Add(token.Value<long>().ToString());
								}
							}

							v = nl;
						}
						else if (argInfo.ArrayOf == typeof(uint))
						{
							v = ToLongIterator(jArray).Select(lon => (uint)lon);
						}
						else if (argInfo.ArrayOf == typeof(uint?))
						{
							v = ToLongIterator(jArray).Select(lon => (uint?)lon);
						}
						else if (argInfo.ArrayOf == typeof(long))
						{
							v = ToLongIterator(jArray);
						}
						else if (argInfo.ArrayOf == typeof(long?))
						{
							v = ToLongIterator(jArray).Select(lon => (long?)lon);
						}
						else
						{
							Fail(t);
						}
					}
					else if(typeof(IEnumerable).IsAssignableFrom(t))
					{
						// The value is some sort of enumerable - just not strictly the one we require.
						// Typically it's e.g. IEnumerable<ulong> being bound to a IEnumerable<uint> field.
						// That is specifically a very common part of the includes system.

						// We need to handle this more generically.
						if (typeof(IEnumerable<ulong>).IsAssignableFrom(t) && argType == typeof(IEnumerable<uint>))
						{
							var val = (IEnumerable<ulong>)v;
							argInfo.ConstructedField.SetValue(this, val.Select(n => (uint)n));
							_arg++;
							return this;
						}
						else
						{
							Fail(t);
						}

					}
				}
				// Common numeric conversion.
				// This happens because the JSON parser exclusively outputs long and double.
				else if (t == typeof(long))
				{
					if (nullableUnderlying == null)
					{
						ConvertFromLong((long)v, argType, argInfo.ConstructedField);
					}
					else
					{
						ConvertFromLongNullable((long)v, nullableUnderlying, argInfo.ConstructedField);
					}

					_arg++;
					return this;
				}
				else if (t == typeof(double))
				{
					if (nullableUnderlying == null)
					{
						ConvertFromDouble((double)v, argType, argInfo.ConstructedField);
					}
					else
					{
						ConvertFromDoubleNullable((double)v, nullableUnderlying, argInfo.ConstructedField);
					}

					_arg++;
					return this;
				}
				else if (!argType.IsAssignableFrom(t))
				{
					Fail(t);
				}
			}

			argInfo.ConstructedField.SetValue(this, v);
			_arg++;
			return this;
		}

		/// <summary>
		/// Treats a JArray like a long iterator.
		/// </summary>
		/// <param name="jArray"></param>
		/// <returns></returns>
		private IEnumerable<long> ToLongIterator(JArray jArray)
		{
			return jArray.Select(token => {

				if (token == null)
				{
					return 0;
				}

				switch (token.Type)
				{
					case JTokenType.Null:
						return 0;
					case JTokenType.String:
						if (long.TryParse(token.Value<string>(), out long ui))
						{
							return ui;
						}
						
						return 0;
					case JTokenType.Boolean:
						return token.Value<bool>() ? (long)1 : 0;
					case JTokenType.Float:
						return (long)token.Value<double>();
					case JTokenType.Integer:
						return token.Value<long>();
					default:
						return 0;
				}
			});
		}

		private void ConvertFromLong(long v, Type targetType, FieldInfo target)
		{
			switch (Type.GetTypeCode(targetType))
			{
				case TypeCode.Boolean:
					target.SetValue(this, (bool)(v != 0));
					break;
				case TypeCode.Char:
					target.SetValue(this, (char)(v));
					break;
				case TypeCode.SByte:
					target.SetValue(this, (sbyte)(v));
					break;
				case TypeCode.Byte:
					target.SetValue(this, (byte)(v));
					break;
				case TypeCode.Int16:
					target.SetValue(this, (short)(v));
					break;
				case TypeCode.UInt16:
					target.SetValue(this, (ushort)(v));
					break;
				case TypeCode.Int32:
					target.SetValue(this, (int)(v));
					break;
				case TypeCode.UInt32:
					target.SetValue(this, (uint)(v));
					break;
				case TypeCode.Int64:
					target.SetValue(this, v);
					break;
				case TypeCode.UInt64:
					target.SetValue(this, (ulong)(v));
					break;
				case TypeCode.Single:
					target.SetValue(this, (float)(v));
					break;
				case TypeCode.Double:
					target.SetValue(this, (double)(v));
					break;
				case TypeCode.String:
					target.SetValue(this, v.ToString());
					break;
				default:
					Fail(targetType);
					break;
			}
		}
		
		private void ConvertFromLongNullable(long v, Type targetType, FieldInfo target)
		{
			switch (Type.GetTypeCode(targetType))
			{
				case TypeCode.Boolean:
					target.SetValue(this, (bool?)(v != 0));
					break;
				case TypeCode.Char:
					target.SetValue(this, (char?)(v));
					break;
				case TypeCode.SByte:
					target.SetValue(this, (sbyte?)(v));
					break;
				case TypeCode.Byte:
					target.SetValue(this, (byte?)(v));
					break;
				case TypeCode.Int16:
					target.SetValue(this, (short?)(v));
					break;
				case TypeCode.UInt16:
					target.SetValue(this, (ushort?)(v));
					break;
				case TypeCode.Int32:
					target.SetValue(this, (int?)(v));
					break;
				case TypeCode.UInt32:
					target.SetValue(this, (uint?)(v));
					break;
				case TypeCode.Int64:
					target.SetValue(this, (long?)(v));
					break;
				case TypeCode.UInt64:
					target.SetValue(this, (ulong?)(v));
					break;
				case TypeCode.Single:
					target.SetValue(this, (float?)(v));
					break;
				case TypeCode.Double:
					target.SetValue(this, (double?)(v));
					break;
				case TypeCode.String:
					target.SetValue(this, v.ToString());
					break;
				default:
					Fail(targetType);
					break;
			}
		}
		
		private void ConvertFromDouble(double v, Type targetType, FieldInfo target)
		{
			switch (Type.GetTypeCode(targetType))
			{
				case TypeCode.Boolean:
					target.SetValue(this, (bool)(v != 0));
					break;
				case TypeCode.Char:
					target.SetValue(this, (char)(v));
					break;
				case TypeCode.SByte:
					target.SetValue(this, (sbyte)(v));
					break;
				case TypeCode.Byte:
					target.SetValue(this, (byte)(v));
					break;
				case TypeCode.Int16:
					target.SetValue(this, (short)(v));
					break;
				case TypeCode.UInt16:
					target.SetValue(this, (ushort)(v));
					break;
				case TypeCode.Int32:
					target.SetValue(this, (int)(v));
					break;
				case TypeCode.UInt32:
					target.SetValue(this, (uint)(v));
					break;
				case TypeCode.Int64:
					target.SetValue(this, v);
					break;
				case TypeCode.UInt64:
					target.SetValue(this, (ulong)(v));
					break;
				case TypeCode.Single:
					target.SetValue(this, (float)(v));
					break;
				case TypeCode.Double:
					target.SetValue(this, (double)(v));
					break;
				case TypeCode.String:
					target.SetValue(this, v.ToString());
					break;
				default:
					Fail(targetType);
					break;
			}
		}
		
		private void ConvertFromDoubleNullable(double v, Type targetType, FieldInfo target)
		{
			switch (Type.GetTypeCode(targetType))
			{
				case TypeCode.Boolean:
					target.SetValue(this, (bool?)(v != 0));
					break;
				case TypeCode.Char:
					target.SetValue(this, (char?)(v));
					break;
				case TypeCode.SByte:
					target.SetValue(this, (sbyte?)(v));
					break;
				case TypeCode.Byte:
					target.SetValue(this, (byte?)(v));
					break;
				case TypeCode.Int16:
					target.SetValue(this, (short?)(v));
					break;
				case TypeCode.UInt16:
					target.SetValue(this, (ushort?)(v));
					break;
				case TypeCode.Int32:
					target.SetValue(this, (int?)(v));
					break;
				case TypeCode.UInt32:
					target.SetValue(this, (uint?)(v));
					break;
				case TypeCode.Int64:
					target.SetValue(this, (long?)(v));
					break;
				case TypeCode.UInt64:
					target.SetValue(this, (ulong?)(v));
					break;
				case TypeCode.Single:
					target.SetValue(this, (float?)(v));
					break;
				case TypeCode.Double:
					target.SetValue(this, (double?)(v));
					break;
				case TypeCode.String:
					target.SetValue(this, v.ToString());
					break;
				default:
					Fail(targetType);
					break;
			}
		}

		/// <summary>
		/// Binds the current arg using the given textual representation.
		/// </summary>
		/// <param name="str"></param>
		public virtual Filter<T, ID> BindFromString(string str)
		{
			Fail(typeof(string));
			return this;
		}

		/// <summary>
		/// A convenience variant of SetPage which returns a stronger typed filter.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public Filter<T, ID> Page(int offset, int size = 50)
		{
			SetPage(offset, size);
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(string v)
		{
			Fail(typeof(string));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(double v)
		{
			Fail(typeof(double));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(float v)
		{
			Fail(typeof(float));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(decimal v)
		{
			Fail(typeof(decimal));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(DateTime v)
		{
			Fail(typeof(DateTime));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(bool v)
		{
			Fail(typeof(bool));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(ulong v)
		{
			Fail(typeof(ulong));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(long v)
		{
			Fail(typeof(long));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(uint v)
		{
			Fail(typeof(uint));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(int v)
		{
			Fail(typeof(int));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(ushort v)
		{
			Fail(typeof(ushort));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(short v)
		{
			Fail(typeof(short));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(byte v)
		{
			Fail(typeof(byte));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(sbyte v)
		{
			Fail(typeof(sbyte));
			return this;
		}

		// Nullables

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(double? v)
		{
			Fail(typeof(double?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(float? v)
		{
			Fail(typeof(float?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(decimal? v)
		{
			Fail(typeof(decimal?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(DateTime? v)
		{
			Fail(typeof(DateTime?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(bool? v)
		{
			Fail(typeof(bool?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(ulong? v)
		{
			Fail(typeof(ulong?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(long? v)
		{
			Fail(typeof(long?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(uint? v)
		{
			Fail(typeof(uint?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(int? v)
		{
			Fail(typeof(int?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(ushort? v)
		{
			Fail(typeof(ushort?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(short? v)
		{
			Fail(typeof(short?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(byte? v)
		{
			Fail(typeof(byte?));
			return this;
		}

		/// <summary>
		/// Binds given value to current argument.
		/// </summary>
		public virtual Filter<T, ID> Bind(sbyte? v)
		{
			Fail(typeof(sbyte?));
			return this;
		}

	}
	
}