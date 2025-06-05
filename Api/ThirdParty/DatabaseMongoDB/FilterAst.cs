using Api.Configuration;
using Api.Contexts;
using Api.Database;
using Api.SocketServerLibrary;
using Api.Startup;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
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
		/// <param name="collectors"></param>
		/// <param name="filter"></param>
		/// <param name="localeCode"></param>
		/// <param name="context"></param>
		public FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
			where INSTANCE_TYPE : T
		{
			if (Root == null)
			{
				return null;
			}

			return Root.ToMongo<INSTANCE_TYPE>(ref collectors, localeCode, filter, context);
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
		/// <param name="currentCollector"></param>
		/// <param name="context"></param>
		/// <param name="filterA"></param>
		/// <param name="localeCode">Optional localeCode used when a request is for e.g. French fields instead. 
		/// It would be e.g. "fr" and just matches whatever your Locale.Code is.</param>
		public FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(IDCollector currentCollector, string localeCode, Context context, FilterBase filterA)
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
			var cc = currentCollector;
			return Pool.Ast.ToMongo<INSTANCE_TYPE>(ref cc, localeCode, (Filter<T, ID>)filterA, context);
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this node to a value to send to Mongo. 
		/// E.g. constants in filters, arg values etc.
		/// </summary>
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Converts this node to a Mongo member name (usually a field name - can include dots for subdocuments). 
		/// E.g. constants in filters, arg values etc.
		/// </summary>
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public virtual string ToMongoMember(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			throw new NotImplementedException();
		}
	}

	public partial class MappingFilterTreeNode<T, ID> : FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Steps through this tree building a MongoDB filter.
		/// Note that if it encounters an array node, it will immediately resolve the value using values stored in the given filter instance.
		/// </summary>
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			if (TargetField != null)
			{
				// The same as just Field=x.
				var idNode = Id as ArgFilterTreeNode<T, ID>;
				var val = idNode.Binding.ConstructedField.GetValue(filter);

				var mem = TargetField.FieldInfo.Name;

				if (TargetField.Localised && localeCode != null)
				{
					mem += "." + localeCode;
				}
				
				return Builders<INSTANCE_TYPE>.Filter.Eq(mem, val);
			}
			
			var idVal = Id.ToMongoValue(ref collectors, localeCode, filter, context);

			// Checking the mappings column for a singular entry.
			return Builders<INSTANCE_TYPE>.Filter.AnyEq("mappings." + MapName, idVal);
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override FilterDefinition<INSTANCE_TYPE> ToMongo<INSTANCE_TYPE>(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			if (Operation == "not")
			{
				return Builders<INSTANCE_TYPE>.Filter.Not(
					A.ToMongo<INSTANCE_TYPE>(ref collectors, localeCode, filter, context)
				);
			}

			if (A is MemberFilterTreeNode<T, ID> member && member.Collect)
			{
				// Use up a collector:
				var collector = collectors;
				collectors = collector.NextCollector;
				var bsonArray = collector.ToBsonArray();
				return Builders<INSTANCE_TYPE>.Filter.In("Id", bsonArray);
			}

			if (Operation == "and" || Operation == "&&")
			{
				return Builders<INSTANCE_TYPE>.Filter.And(
					A.ToMongo<INSTANCE_TYPE>(ref collectors, localeCode, filter, context),
					B.ToMongo<INSTANCE_TYPE>(ref collectors, localeCode, filter, context)
				);
			}
			else if (Operation == "or" || Operation == "||")
			{
				return Builders<INSTANCE_TYPE>.Filter.Or(
					A.ToMongo<INSTANCE_TYPE>(ref collectors, localeCode, filter, context),
					B.ToMongo<INSTANCE_TYPE>(ref collectors, localeCode, filter, context)
				);
			}
			else if (Operation == "=")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Eq(mem, val);
			}
			else if (Operation == "!=")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Not(
					Builders<INSTANCE_TYPE>.Filter.Eq(mem, val)
				);
			}
			else if (Operation == ">=")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Gte(mem, val);
			}
			else if (Operation == ">")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Gt(mem, val);
			}
			else if (Operation == "<")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Lt(mem, val);
			}
			else if (Operation == "<=")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				return Builders<INSTANCE_TYPE>.Filter.Lte(mem, val);
			}
			else if (Operation == "startswith")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				var escaped = Regex.Escape(val == null ? "" : val.ToString());
				return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression($"^{escaped}"));
			}
			else if (Operation == "endswith")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				var escaped = Regex.Escape(val == null ? "" : val.ToString());
				return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression($"{escaped}$"));
			}
			else if (Operation == "contains")
			{
				var mem = A.ToMongoMember(ref collectors, localeCode, filter, context);
				var val = B.ToMongoValue(ref collectors, localeCode, filter, context);
				var escaped = Regex.Escape(val == null ? "" : val.ToString());
				return Builders<INSTANCE_TYPE>.Filter.Regex(mem, new BsonRegularExpression(escaped));
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override string ToMongoMember(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			if (OnContext)
			{
				throw new PublicException("Filter syntax error: attempted to use a context field as a member of the table.", "filter/invalid_field_usage");
			}

			// Regular field.
			if (Field.Localised && localeCode != null)
			{
				return Field.FieldInfo.Name + "." + localeCode;
			}

			return Field.FieldInfo.Name;
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
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
		/// <param name="collectors"></param>
		/// <param name="localeCode"></param>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		public override object ToMongoValue(ref IDCollector collectors, string localeCode, Filter<T, ID> filter, Context context)
		{
			return Binding.ConstructedField.GetValue(filter);
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
		/// <exception cref="NotImplementedException"></exception>
		public virtual BsonArray ToBsonArray()
		{
			throw new NotImplementedException();
		}
	}

	public partial class IDCollector<T>
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
