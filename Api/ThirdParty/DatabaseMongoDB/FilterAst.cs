using Api.Configuration;
using Api.Contexts;
using Api.Database;
using Api.SocketServerLibrary;
using Api.Startup;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace Api.Permissions{

	/// <summary>
	/// A tree of parsed filter nodes.
	/// </summary>
	public partial class FilterAst<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="localeCode"></param>
		/// <param name="context"></param>
		public FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(string localeCode, Filter<T, ID> filter, Context context)
			where INSTANCE_TYPE : T
		{
			if (Root == null)
			{
				return null;
			}

			return Root.ToMongo<INSTANCE_TYPE>(localeCode, filter, context);
		}
	}
	
	/// <summary>
	/// Fast precompiled non-allocating filter engine.
	/// </summary>
	public partial class Filter<T,ID> : FilterBase
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{

		/// <summary>Builds a MongoDB filter.</summary>
		/// <param name="context"></param>
		/// <param name="filterA"></param>
		/// <param name="localeCode">Optional localeCode used when a request is for e.g. French fields instead. 
		/// It would be e.g. "fr" and just matches whatever your Locale.Code is.</param>
		public FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(string localeCode, Context context, FilterBase filterA)
			where INSTANCE_TYPE : T
		{
			if (Pool == null || Pool.Ast == null)
			{
				// There isn't one!
				return null;
			}
			
			// Don't cache this.
			// Both queryA and queryB might output the same text, however they will use different arg numbers, meaning only filterA can be cached (because it's first).
			// For simplicity, therefore, cache neither.

			// Only filterA is permitted to have args. This is also important for checking the state of any On(..) calls.
			return Pool.Ast.ToMongo<INSTANCE_TYPE>(localeCode, (Filter<T, ID>)filterA, context);
		}
		
	}
	
	/// <summary>
	/// A filter. Use the concrete-type variant as much as possible.
	/// </summary>
	public partial class FilterBase
	{
		
	}
	
	/// <summary>
	/// Base tree node
	/// </summary>
	public partial class FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Constructs a typed mongo .In filter.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual FilterDefinition<INSTANCE_TYPE> ToMongoIn<INSTANCE_TYPE>(string member, string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Constructs a typed mongo .AnyEq filter.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual FilterDefinition<INSTANCE_TYPE> ToMongoAny<INSTANCE_TYPE>(string member, string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Constructs a typed Mongo .All filter.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual FilterDefinition<INSTANCE_TYPE> ToMongoAll<INSTANCE_TYPE>(string member, string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this node to a value to send to Mongo. 
		/// E.g. constants in filters, arg values etc.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this node to a Mongo member name (usually a field name - can include dots for subdocuments). 
		/// E.g. constants in filters, arg values etc.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual string ToMongoMember(string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class OpFilterTreeNode<T, ID> : FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(string localeCode, Filter<T, ID> filter, Context context)
		{
			var operation = Operation == null ? "" : Operation.ToLower().Trim();

			if (operation == "not")
			{
				return Builders<INSTANCE_TYPE>.Filter.Not(
					A.ToMongo<INSTANCE_TYPE>(localeCode, filter, context)
				);
			}

			if (operation == "and" || operation == "&&")
			{
				return Builders<INSTANCE_TYPE>.Filter.And(
					A.ToMongo<INSTANCE_TYPE>(localeCode, filter, context),
					B.ToMongo<INSTANCE_TYPE>(localeCode, filter, context)
				);
			}
			else if (operation == "or" || operation == "||")
			{
				return Builders<INSTANCE_TYPE>.Filter.Or(
					A.ToMongo<INSTANCE_TYPE>(localeCode, filter, context),
					B.ToMongo<INSTANCE_TYPE>(localeCode, filter, context)
				);
			}
			else if (operation == "=")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);

				// If A is a mapping field then always use Eq.
				// Using Eq with a singular value naturally checks any matching element.
				// Using Eq with an array is an exact match (in order), matching what the cache check does.

				// Otherwise if it's an array being given to an ordinary field match, use In instead.
				if (B is ArgFilterTreeNode<T, ID> argNode && argNode.Array && !A.IsListField)
				{
					return B.ToMongoIn<INSTANCE_TYPE>(mem, localeCode, filter, context);
				}
				else
				{
					var val = B.ToMongoValue(localeCode, filter, context);
					return Builders<INSTANCE_TYPE>.Filter.Eq(mem, val);
				}
			}
			else if (operation == "!=")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);

				// If it's an array, use In instead.
				if (B is ArgFilterTreeNode<T, ID> argNode && argNode.Array && !A.IsListField)
				{
					return Builders<INSTANCE_TYPE>.Filter.Not(
						B.ToMongoIn<INSTANCE_TYPE>(mem, localeCode, filter, context)
					);
				}
				else
				{
					var val = B.ToMongoValue(localeCode, filter, context);

					return Builders<INSTANCE_TYPE>.Filter.Not(
						Builders<INSTANCE_TYPE>.Filter.Eq(mem, val)
					);
				}
			}
			else if (operation == ">=")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);
				var val = B.ToMongoValue(localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Gte(mem, val);
			}
			else if (operation == ">")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);
				var val = B.ToMongoValue(localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Gt(mem, val);
			}
			else if (operation == "<")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);
				var val = B.ToMongoValue(localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Lt(mem, val);
			}
			else if (operation == "<=")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);
				var val = B.ToMongoValue(localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Lte(mem, val);
			}
			else if (operation == "startswith")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);
				var val = B.ToMongoValue(localeCode, filter, context);
				var escaped = Regex.Escape(val == null ? "" : val.ToString());
				return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression($"^{escaped}"));
			}
			else if (operation == "endswith")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);
				var val = B.ToMongoValue(localeCode, filter, context);
				var escaped = Regex.Escape(val == null ? "" : val.ToString());
				return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression($"{escaped}$"));
			}
			else if (operation == "contains")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);

				if (A.IsListField)
				{
					if (B is ArgFilterTreeNode<T, ID> argNode && argNode.Array)
					{
						// Contains all of these things
						return B.ToMongoAll<INSTANCE_TYPE>(mem, localeCode, filter, context);
					}
					else
					{
						// Contains the given value (can have other stuff in there). Same as Eq:
						var val = B.ToMongoValue(localeCode, filter, context);
						return Builders<INSTANCE_TYPE>.Filter.Eq(mem, val);
					}
				}
				else
				{
					var val = B.ToMongoValue(localeCode, filter, context);
					var escaped = Regex.Escape(val == null ? "" : val.ToString());
					return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression(escaped));
				}
			}
			else if (operation == "containsany")
			{
				var mem = A.ToMongoMember(localeCode, filter, context);

				if (A.IsListField)
				{
					if (B is ArgFilterTreeNode<T, ID> argNode && argNode.Array)
					{
						// Contains *any* of these things
						return B.ToMongoAny<INSTANCE_TYPE>(mem, localeCode, filter, context);
					}
					else
					{
						// Contains the given value (can have other stuff in there). Same as Eq:
						var val = B.ToMongoValue(localeCode, filter, context);
						return Builders<INSTANCE_TYPE>.Filter.Eq(mem, val);
					}
				}
				else
				{
					// Unchanged from contains for strings
					var val = B.ToMongoValue(localeCode, filter, context);
					var escaped = Regex.Escape(val == null ? "" : val.ToString());
					return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression(escaped));
				}
			}
			
			throw new Exception("Operation not supported via MongoDB: " + Operation);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class MemberFilterTreeNode<T, ID> : FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			if (OnContext)
			{
				return ContextField.PrivateFieldInfo.GetValue(context);
			}

			throw new PublicException("Filter syntax error: attempted to use a field as a value.", "filter/invalid_field_usage");
		}

		/// <summary>
		/// Steps through this tree building a MongoDB member name.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override string ToMongoMember(string localeCode, Filter<T, ID> filter, Context context)
		{
			if (OnContext)
			{
				throw new PublicException("Filter syntax error: attempted to use a context field as a member of the table.", "filter/invalid_field_usage");
			}

			if (Field.VirtualInfo != null)
			{
				// These are never localised.
				return "mappings." + Field.Name;
			}
			else if (Field.FieldInfo != null)
			{
				// Regular field.
				var baseName = Field.FieldInfo.Name;

				if (Field.Localised && localeCode != null)
				{
					return baseName + "." + localeCode;
				}

				return baseName;
			}
			else
			{
				// (it's a property actually - properties are unusable in filters as they don't exist in the data store.
				// We don't call it a property as it leaks a little bit of internal structural information).
				throw new PublicException("Filter syntax error: Can't use the field '" + Field.Name + "' in a filter.", "filter/invalid_field_usage");
			}
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public partial class IsIncludedFilterTreeNode<T, ID> : FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			return filter.IsIncluded;
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	public partial class StringFilterTreeNode<T, ID> : ConstFilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			return Value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class NumberFilterTreeNode<T, ID> : ConstFilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			return Value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class DecimalFilterTreeNode<T, ID> : ConstFilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			return Value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class BoolFilterTreeNode<T, ID> : ConstFilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			return Value;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class NullFilterTreeNode<T, ID> : ConstFilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			return null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class ArgFilterTreeNode<T, ID> : ConstFilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(string localeCode, Filter<T, ID> filter, Context context)
		{
			var arg = filter.Arguments[Binding.Index];
			var val = arg.BoxedValue;

			// Convert Linq selectors to lists (unfortunately the mongo driver doesn't know how to treat it as an arbritrary IEnumerable)
			var iEnum = val as IEnumerable;

			if (iEnum != null && !(val is string))
			{
				var type = val.GetType();

				if (type.IsArray || type == typeof(LongIDCollector) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
				{
					// It's a supported enumerable anyway.
					return val;
				}

				var elementType = FilterAst.GetEnumerableType(type);

				if (elementType == null)
				{
					throw new Exception(
						"Cannot bind '" + type + "' on mongoDB as it is an enumerable type with no apparent element type. " +
						"Consider making a custom BSON serializer for this type."
					);
				}

				// Convert it to a list:
				var listType = typeof(List<>).MakeGenericType(elementType);
				return Activator.CreateInstance(listType, iEnum);
			}

			return val;
		}

		/// <summary>
		/// Converts specifically to a typed mongo AnyEq(..) statement.
		/// </summary>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <param name="member"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public override FilterDefinition<INSTANCE_TYPE> ToMongoAny<INSTANCE_TYPE>(string member, string localeCode, Filter<T, ID> filter, Context context)
		{
			var arg = filter.Arguments[Binding.Index];
			var val = arg.BoxedValue;

			if (val == null)
			{
				// Key in () - matches nothing.
				// We don't know what the IEnumerable type would have been but it
				// doesn't matter anyway as this will just emit an empty bson array.
				return Builders<INSTANCE_TYPE>.Filter.AnyEq(member, System.Array.Empty<int>());
			}

			// Unfortunately reflection is required to bind this here.
			var elementType = FilterAst.GetEnumerableType(val.GetType());
			var genericMethod = typeof(ArgFilterTreeNode<T, ID>)
				.GetMethod(nameof(ToTypedMongoAny))
				.MakeGenericMethod(new Type[] {
					typeof(INSTANCE_TYPE),
					elementType
				});

			return (FilterDefinition<INSTANCE_TYPE>)genericMethod.Invoke(this, new object[] {
				member,
				val
			});
		}

		/// <summary>
		/// Converts specifically to a typed mongo All(..) statement.
		/// </summary>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <param name="member"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public override FilterDefinition<INSTANCE_TYPE> ToMongoAll<INSTANCE_TYPE>(string member, string localeCode, Filter<T, ID> filter, Context context)
		{
			var arg = filter.Arguments[Binding.Index];
			var val = arg.BoxedValue;

			if (val == null)
			{
				// Key in () - matches nothing.
				// We don't know what the IEnumerable type would have been but it
				// doesn't matter anyway as this will just emit an empty bson array.
				return Builders<INSTANCE_TYPE>.Filter.All(member, System.Array.Empty<int>());
			}

			// Unfortunately reflection is required to bind this here.
			var elementType = FilterAst.GetEnumerableType(val.GetType());
			var genericMethod = typeof(ArgFilterTreeNode<T, ID>)
				.GetMethod(nameof(ToTypedMongoAll))
				.MakeGenericMethod(new Type[] {
					typeof(INSTANCE_TYPE),
					elementType
				});

			return (FilterDefinition<INSTANCE_TYPE>)genericMethod.Invoke(this, new object[] {
				member,
				val
			});
		}

		/// <summary>
		/// Converts specifically to a typed mongo In(..) statement.
		/// </summary>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <param name="member"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public override FilterDefinition<INSTANCE_TYPE> ToMongoIn<INSTANCE_TYPE>(string member, string localeCode, Filter<T, ID> filter, Context context)
		{
			var arg = filter.Arguments[Binding.Index];
			var val = arg.BoxedValue;

			if (val == null)
			{
				// Key in () - matches nothing.
				// We don't know what the IEnumerable type would have been but it
				// doesn't matter anyway as this will just emit an empty bson array.
				return Builders<INSTANCE_TYPE>.Filter.In(member, System.Array.Empty<int>());
			}

			// Unfortunately reflection is required to bind this here.
			var elementType = FilterAst.GetEnumerableType(val.GetType());
			var genericMethod = typeof(ArgFilterTreeNode<T, ID>)
				.GetMethod(nameof(ToTypedMongoIn))
				.MakeGenericMethod(new Type[] { 
					typeof(INSTANCE_TYPE),
					elementType
				});

			return (FilterDefinition<INSTANCE_TYPE>)genericMethod.Invoke(this, new object[] {
				member,
				val
			});
		}

		/// <summary>
		/// Constructs a typed mongoDB .In filter.
		/// </summary>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <typeparam name="ENUM_T"></typeparam>
		/// <param name="member"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public FilterDefinition<INSTANCE_TYPE> ToTypedMongoIn<INSTANCE_TYPE, ENUM_T>(string member, IEnumerable<ENUM_T> val)
		{
			// Unlike the other mechanism though this does *not* require
			// conversion to List from various Linq internal types.
			return Builders<INSTANCE_TYPE>.Filter.In(member, val);
		}

		/// <summary>
		/// Constructs a typed mongoDB .All filter.
		/// </summary>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <typeparam name="ENUM_T"></typeparam>
		/// <param name="member"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public FilterDefinition<INSTANCE_TYPE> ToTypedMongoAll<INSTANCE_TYPE, ENUM_T>(string member, IEnumerable<ENUM_T> val)
		{
			// Unlike the other mechanism though this does *not* require
			// conversion to List from various Linq internal types.
			return Builders<INSTANCE_TYPE>.Filter.All(member, val);
		}

		/// <summary>
		/// Constructs a typed mongoDB .AnyEq filter.
		/// </summary>
		/// <typeparam name="INSTANCE_TYPE"></typeparam>
		/// <typeparam name="ENUM_T"></typeparam>
		/// <param name="member"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public FilterDefinition<INSTANCE_TYPE> ToTypedMongoAny<INSTANCE_TYPE, ENUM_T>(string member, IEnumerable<ENUM_T> val)
		{
			// Unlike the other mechanism though this does *not* require
			// conversion to List from various Linq internal types.
			return Builders<INSTANCE_TYPE>.Filter.AnyEq(member, val);
		}
	}
}


namespace Api.Startup {

	public partial class IDCollector
	{
		/// <summary>
		/// Gets this ID collector content as a Bson array for MongoDB.
		/// </summary>
		/// <returns></returns>
		public virtual BsonArray ToBsonArray()
		{
			throw new NotImplementedException();
		}
	}

	public partial class LongIDCollector
	{

		/// <summary>
		/// Gets this ID collector content as a Bson array for MongoDB.
		/// </summary>
		/// <returns></returns>
		public override BsonArray ToBsonArray()
		{
			var result = new BsonArray(Count);
			
			foreach (var item in this)
			{
				result.Add(BsonValue.Create(item));
			}

			return result;
		}

	}
}
