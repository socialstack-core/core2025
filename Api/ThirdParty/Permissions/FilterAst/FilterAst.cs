using Api.Contexts;
using Api.Database;
using Api.Startup;
using Api.Translate;
using Api.Users;
using Karambolo.PO;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Api.Permissions{

	/// <summary>
	/// Functions that can be used in a filter. They are invoked during compilation and can output type specific things.
	/// </summary>
	public static partial class FilterFunctions
	{
		/// <summary>
		/// The available methods
		/// </summary>
		private static ConcurrentDictionary<string, MethodInfo> _methods;

		/// <summary>
		/// Gets a filter function by its given lowercase name, or null if it wasn't found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Func<MemberFilterTreeNode<T, ID>, FilterAst<T, ID>, FilterTreeNode<T, ID>> Get<T, ID>(string name)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (_methods == null)
			{
				_methods = new ConcurrentDictionary<string, MethodInfo>();

				// Get public static methods:
				var set = typeof(FilterFunctions).GetMethods(BindingFlags.Public | BindingFlags.Static);

				foreach (var method in set)
				{
					var mtdName = method.Name.ToLower();
					if (mtdName == "get")
					{
						continue;
					}
					_methods[mtdName] = method;
				}
			}

			if (!_methods.TryGetValue(name, out MethodInfo mtd))
			{
				return null;
			}

			mtd = mtd.MakeGenericMethod(typeof(T), typeof(ID));
			var result = mtd.CreateDelegate<Func<MemberFilterTreeNode<T, ID>, FilterAst<T, ID>, FilterTreeNode<T, ID>>>();
			return result;
		}

		/// <summary>
		/// True if the filter is being executed via an inclusion.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="node"></param>
		/// <param name="ast"></param>
		public static FilterTreeNode<T, ID> IsIncluded<T, ID>(MemberFilterTreeNode<T, ID> node, FilterAst<T, ID> ast)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (node.Args.Count != 0)
			{
				throw new PublicException("IsIncluded in a filter call takes 0 arguments", "filter_invalid");
			}

			// Use specialised IsIncluded node:
			return new IsIncludedFilterTreeNode<T,ID>();
		}

		/*
		/// <summary>
		/// True for rows that have a mapping to the given target object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="node"></param>
		/// <param name="ast"></param>
		public static FilterTreeNode<T, ID> On<T, ID>(MemberFilterTreeNode<T, ID> node, FilterAst<T, ID> ast)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (node.Args.Count < 2)
			{
				throw new PublicException("On in a filter call takes 2 or 3 arguments", "filter_invalid");
			}

			// Uses if mapping node:

			// Note that On() has custom arg parsing as it is permitted to contain constants always, so these casts are ok.
			return new MappingFilterTreeNode<T, ID>()
			{
				SourceMapping = true,
				IsOn = true,
				TypeName = (node.Args[0] as StringFilterTreeNode<T, ID>).Value,
				Id = node.Args[1] as ArgFilterTreeNode<T, ID>,
				MapName = node.Args.Count > 2 ? (node.Args[2] as StringFilterTreeNode<T, ID>).Value : null
			}.Add(ast);
		}
		*/

		/// <summary>
		/// True if the given role object is a direct match, or it has a field called Role and matches.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="node"></param>
		/// <param name="ast"></param>
		public static FilterTreeNode<T, ID> IsSelfRole<T, ID>(MemberFilterTreeNode<T, ID> node, FilterAst<T, ID> ast)
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (node.Args.Count != 0)
			{
				throw new PublicException("IsSelfRole in a filter call takes 0 arguments", "filter_invalid");
			}

			var op = new OpFilterTreeNode<T, ID>()
			{
				Operation = "="
			};

			// Context RoleId field:
			op.B = new MemberFilterTreeNode<T, ID>()
			{
				Name = "RoleId",
				OnContext = true
			}.Resolve(ast);

			if (typeof(T) == typeof(Api.Permissions.Role))
			{
				// Id=context.RoleId

				op.A = new MemberFilterTreeNode<T, ID>()
				{
					Name = "Id"
				}.Resolve(ast);
			}
			else
			{
				// Role=context.RoleId
				op.A = new MemberFilterTreeNode<T, ID>()
				{
					Name = "Role"
				}.Resolve(ast);
			}

			return op;
		}

		/// <summary>
		/// True if either the object is a User and is a direct match, or if this user is the creator user.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="node"></param>
		/// <param name="ast"></param>
		public static FilterTreeNode<T, ID> IsSelf<T, ID>(MemberFilterTreeNode<T, ID> node, FilterAst<T, ID> ast)
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (node.Args.Count != 0)
			{
				throw new PublicException("IsSelf in a filter call takes 0 arguments", "filter_invalid");
			}

			var op = new OpFilterTreeNode<T, ID>() {
				Operation = "="
			};

			// Context userId field:
			op.B = new MemberFilterTreeNode<T, ID>()
			{
				Name = "UserId",
				OnContext = true
			}.Resolve(ast);

			if (typeof(T) == typeof(Users.User))
			{
				// Id=context.UserId

				op.A = new MemberFilterTreeNode<T, ID>() {
					Name = "Id"
				}.Resolve(ast);
			}
			else
			{
				// If the object does not have a UserId field (e.g. it's just a regular Content<>), then return a const false.
				if (!typeof(UserCreatedContent<ID>).IsAssignableFrom(typeof(T)))
				{
					// A false:
					return new BoolFilterTreeNode<T, ID>();
				}

				// UserId=context.UserId
				op.A = new MemberFilterTreeNode<T, ID>()
				{
					Name = "UserId"
				}.Resolve(ast);
			}

			return op;
		}

		/// <summary>
		/// True if the contextual user is the creator, or there is a "UserPermits" mapping of {Thing}->{Context.User}
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="node"></param>
		/// <param name="ast"></param>
		public static FilterTreeNode<T, ID> HasUserPermit<T, ID>(MemberFilterTreeNode<T, ID> node, FilterAst<T, ID> ast)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (node.Args.Count != 0)
			{
				throw new PublicException("HasUserPermit in a filter call takes 0 arguments", "filter_invalid");
			}

			return new OpFilterTreeNode<T, ID>()
			{
				Operation = "contains",
				A = new MemberFilterTreeNode<T, ID>()
				{
					Name = "UserPermits"
				}.Resolve(ast),
				B = new MemberFilterTreeNode<T, ID>()
				{
					Name = "UserId",
					OnContext = true
				}.Resolve(ast)
			};
		}

		/// <summary>
		/// True if there is a "RolePermits" mapping of {Thing}->{Context.Role}
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="node"></param>
		/// <param name="ast"></param>
		public static FilterTreeNode<T, ID> HasRolePermit<T, ID>(MemberFilterTreeNode<T, ID> node, FilterAst<T, ID> ast)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (node.Args.Count != 0)
			{
				throw new PublicException("HasRolePermit in a filter call takes 0 arguments", "filter_invalid");
			}

			return new OpFilterTreeNode<T, ID>()
			{
				Operation = "contains",
				A = new MemberFilterTreeNode<T, ID>() {
					Name = "RolePermits"
				}.Resolve(ast),
				B = new MemberFilterTreeNode<T, ID>()
				{
					Name = "RoleId",
					OnContext = true
				}.Resolve(ast)
			};
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public static class FilterAst
	{
		/// <summary>
		/// FilterBase.NullCheck
		/// </summary>
		public static MethodInfo _baseNullCheck;
		
		/// <summary>
		/// FilterBase.CheckParseSuccess
		/// </summary>
		public static MethodInfo _checkParseSuccess;

		/// <summary>
		/// FilterAst.TryParseDate
		/// </summary>
		public static MethodInfo _tryParseDate;
		
		/// <summary>
		/// Performs a string.contains if both args are not null.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool GuardedContains(string a, string b)
		{
			if (a == null || b == null)
			{
				return false;
			}

			return a.Contains(b, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Date parsing. Supports numeric tick counts as well as actual date strings.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public static bool TryParseDate(string s, out DateTime field)
		{
			// If it's numeric, then use ticks:
			if (long.TryParse(s, out long ticks))
			{
				// Ticks to/ from the frontend are in ms. Scale to the ns used by dotnet:
				ticks *= 10000;
				field = new DateTime(ticks);
				return true;
			}

			return DateTime.TryParse(s, out field);
		}

		/// <summary>
		/// Checks if a TryParse was successful and if not emits a friendly error.
		/// Consumes a single bool from the stack.
		/// </summary>
		/// <param name="state"></param>
		public static void CheckParseSuccess(bool state)
		{
			if (!state)
			{
				throw new PublicException("Invalid format for one of your filter args. Make sure the value is correct for each one.", "filter_invalid");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <param name="q"></param>
		/// <param name="allowConstants"></param>
		/// <param name="allowArgs"></param>
		/// <returns></returns>
		public static FilterAst<T, ID> Parse<T, ID>(AutoService<T,ID> service, string q, bool allowConstants = true, bool allowArgs = true)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			if (string.IsNullOrEmpty(q))
			{
				return null;
			}
			var result = new FilterAst<T, ID>(q);
			result.Service = service;
			result.AllowConstants = allowConstants;
			result.AllowArgs = allowArgs;
			result.ConsumeWhitespace();
			result.Root = result.ParseAny(0);
			return result;
		}
	}

	/// <summary>
	/// Argument binding
	/// </summary>
	public class ArgBinding
	{
		/// <summary>
		/// True if this arg can be null. Either it is not a valuetype, or it is a generic Nullable
		/// </summary>
		public bool IsNullable;
		/// <summary>
		/// Field type
		/// </summary>
		public Type ArgType;
		/// <summary>
		/// Underlying builder
		/// </summary>
		public FieldBuilder Builder;
		/// <summary>
		/// If this is an array type, the element type inside it.
		/// </summary>
		public Type ArrayOf;
		/// <summary>
		/// Constructed field on target type
		/// </summary>
		public FieldInfo ConstructedField;
		/// <summary>
		/// The Bind() method for args of this same type.
		/// </summary>
		public ILGenerator BindMethod;
		/// <summary>
		/// True if this binding was the one that created the bind method. It'll be up to it to add the Ret.
		/// </summary>
		public bool FirstMethodUser;
	}

	/// <summary>
	/// A tree of parsed filter nodes.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="ID"></typeparam>
	public partial class FilterAst<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// Root node.
		/// </summary>
		public FilterTreeNode<T,ID> Root;
		/// <summary>
		/// True if constant values are permitted. They're disabled for frontend users to avoid 
		/// potentially vast quantities of apparent unique filters, heavily damaging our ability to optimise them.
		/// </summary>
		public bool AllowConstants;
		/// <summary>
		/// True if args ('?') are allowed in this filter.
		/// </summary>
		public bool AllowArgs;
		/// <summary>
		/// The query str
		/// </summary>
		public readonly string Query;
		/// <summary>
		/// Current index in the query
		/// </summary>
		public int Index;
		/// <summary>
		/// Current arg index.
		/// </summary>
		public int ArgIndex;
		/// <summary>
		/// The autoservice.
		/// </summary>
		public AutoService<T, ID> Service;
		/// <summary>
		/// The bound args, available after calling ConstructType.
		/// </summary>
		public List<ArgBinding> Args;

		/// <summary>
		/// Create an ast for the given query string.
		/// </summary>
		/// <param name="q"></param>
		public FilterAst(string q)
		{
			Query = q;
		}

		private TypeBuilder TypeBuilder;

		/// <summary>
		/// The BindFromString method
		/// </summary>
		private ILGenerator BindStringMethod;

		/// <summary>
		/// an array of just typeof(string)
		/// </summary>
		private static Type[] _strTypeInArray = new Type[] { typeof(string) };

		/// <summary>
		/// Reads the value from a member field. It can be localized or virtual. Nullables become default and Localized + JsonString are expanded.
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="member"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private Type ReadMemberValue(ILGenerator generator, MemberFilterTreeNode<T, ID> member)
		{
			if (member.ContextField != null)
			{
				// It's a context field.
				generator.Emit(OpCodes.Ldarg_1);
				var ctxGetMethod = member.ContextField.Property.GetGetMethod();
				generator.Emit(OpCodes.Callvirt, ctxGetMethod);
				return ctxGetMethod.ReturnType;
			}

			if (member.Field.VirtualInfo != null)
			{
				// Mapping fields are present on the .Mappings field.

				var mappingsField = typeof(Content<ID>)
					.GetField(
						nameof(Content<ID>.Mappings),
						BindingFlags.Public | BindingFlags.Instance
					);

				generator.Emit(OpCodes.Ldarg_2); // The object
				generator.Emit(OpCodes.Ldflda, mappingsField); // The mappings field (a struct)
				var getMethod = typeof(MappingData).GetMethod(nameof(MappingData.Get));
				generator.Emit(OpCodes.Ldstr, member.Field.Name.ToLower());
				generator.Emit(OpCodes.Callvirt, getMethod);
				return getMethod.ReturnType; // List<ulong>
			}
			else if (member.Field.FieldInfo != null)
			{
				return ReadMemberFieldValue(generator, member.Field.FieldInfo);
			}
			else
			{
				throw new Exception("Invalid filter syntax");
			}
		}

		/// <summary>
		/// Reads a member field. Expands Localized and JsonString as well.
		/// </summary>
		private Type ReadMemberFieldValue(ILGenerator generator, FieldInfo fieldInfo)
		{
			Type fieldType = fieldInfo.FieldType;

			var unwrapedType = fieldType;
			
			generator.Emit(OpCodes.Ldarg_2); // The object
			generator.Emit(OpCodes.Ldfld, fieldInfo);

			// Unwrap localized:
			if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Localized<>))
			{
				unwrapedType = fieldType.GetGenericArguments()[0];
				
				// .Get(context) => T
				var localized = generator.DeclareLocal(fieldType);
				generator.Emit(OpCodes.Stloc, localized);
				generator.Emit(OpCodes.Ldloca, localized);
				generator.Emit(OpCodes.Ldarg_1); // Context
				generator.Emit(OpCodes.Ldc_I4_1); // With fallback (true)
				generator.Emit(
					OpCodes.Callvirt,
					fieldType
						.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance, new Type[] {
							typeof(Context),
							typeof(bool)
						})
				);

				// Unwrap the localized attr:
				fieldType = unwrapedType;
			}

			if (fieldType == typeof(JsonString))
			{
				// Unwrap JsonString as a regular string:
				var jStr = generator.DeclareLocal(fieldType);
				var valMethod = fieldType.GetMethod(nameof(JsonString.ValueOf), BindingFlags.Public | BindingFlags.Instance);
				generator.Emit(OpCodes.Stloc, jStr);
				generator.Emit(OpCodes.Ldloca, jStr);
				generator.Emit(OpCodes.Callvirt, valMethod);
				fieldType = typeof(string);
			}

			var underlyingType = Nullable.GetUnderlyingType(fieldType);

			if (underlyingType != null)
			{
				// GetValueOrDefault call:
				var valOrDefault = fieldType
					.GetMethod("GetValueOrDefault", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);

				var nulb = generator.DeclareLocal(fieldType);
				generator.Emit(OpCodes.Stloc, nulb);
				generator.Emit(OpCodes.Ldloca, nulb);
				generator.Emit(OpCodes.Callvirt, valOrDefault);

				fieldType = underlyingType;
			}

			if (fieldType == typeof(DateTime))
			{
				var dateTimeLocal = generator.DeclareLocal(fieldType);
				generator.Emit(OpCodes.Stloc, dateTimeLocal);
				generator.Emit(OpCodes.Ldloca, dateTimeLocal);
				var ticksGetter = typeof(DateTime).GetProperty(nameof(DateTime.Ticks)).GetGetMethod();
				generator.Emit(OpCodes.Callvirt, ticksGetter);
			}

			return fieldType;
		}

		/// <summary>
		/// Emits a read field value for the given node into the given generator.
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="node"></param>
		/// <param name="idealType"></param>
		public Type EmitReadValue(ILGenerator generator, FilterTreeNode<T, ID> node, Type idealType)
		{
			if (node is MemberFilterTreeNode<T, ID> member)
			{
				// It's a member (e.g. CtxField = ObjectField)
				return ReadMemberValue(generator, member);
			}
			
			if (node is ConstFilterTreeNode<T, ID> value)
			{
				// The field's type dictates what we'll read the value as.

				// Emit the value. This will vary a little depending on what fieldType is.
				var argNode = value as ArgFilterTreeNode<T, ID>;

				if (argNode == null)
				{
					// Constant. Cooerce the constant to being of the same type as the field.

					if (idealType == typeof(string))
					{
						generator.Emit(OpCodes.Ldstr, value.AsString());
					}
					else if (idealType == typeof(float))
					{
						generator.Emit(OpCodes.Ldc_R4, (float)value.AsDecimal());
					}
					else if (idealType == typeof(double))
					{
						generator.Emit(OpCodes.Ldc_R8, (double)value.AsDecimal());
					}
					else if (idealType == typeof(uint) || idealType == typeof(int) || idealType == typeof(short) ||
						idealType == typeof(ushort) || idealType == typeof(byte) || idealType == typeof(sbyte))
					{
						generator.Emit(OpCodes.Ldc_I4, value.AsInt());
					}
					else if (idealType == typeof(ulong) || idealType == typeof(long))
					{
						generator.Emit(OpCodes.Ldc_I8, value.AsInt());
					}
					else if (idealType == typeof(bool))
					{
						generator.Emit(OpCodes.Ldc_I4, value.AsBool() ? 1 : 0);
					}

					return idealType;
				}

				// Create the arg now:
				var isEnumerable = argNode.Array;
				Type argElementType = idealType;

				if (argElementType.IsGenericType && argElementType.GetGenericTypeDefinition() == typeof(Localized<>))
				{
					// Unwrap localized fields:
					argElementType = argElementType.GetGenericArguments()[0];
				}

				// JsonString is an alias:
				if (argElementType == typeof(JsonString))
				{
					argElementType = typeof(string);
				}

				var argType = argElementType;

				if (argNode.Array)
				{
					argType = typeof(IEnumerable<>).MakeGenericType(argElementType);
				}

				var argField = DeclareArg(argNode.Id, argType);
				argNode.Binding = argField;
				generator.Emit(OpCodes.Ldarg_0);

				// If it's nullable, unwrap it. Not available on the enumerable types.
				var underlyingType = Nullable.GetUnderlyingType(argType);

				if (underlyingType != null)
				{
					// GetValueOrDefault call:
					var valOrDefault = argType.GetMethod("GetValueOrDefault", BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);

					if (valOrDefault != null)
					{
						generator.Emit(OpCodes.Ldflda, argField.Builder);
						generator.Emit(OpCodes.Call, valOrDefault);
					}
					else
					{
						generator.Emit(OpCodes.Ldfld, argField.Builder);
					}

					return underlyingType;
				}
				
				generator.Emit(OpCodes.Ldfld, argField.Builder);
				return argType;
			}
			
			throw new Exception("Invalid filter - can't convert this node to a readable value.");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="argType"></param>
		/// <returns></returns>
		public ArgBinding DeclareArg(int index, Type argType)
		{
			var builder = TypeBuilder.DefineField("Arg_" + index, argType, System.Reflection.FieldAttributes.Public);

			var nullableBaseType = Nullable.GetUnderlyingType(argType);

			var binding = new ArgBinding() {
				Builder = builder,
				ArgType = argType,
				IsNullable = !argType.IsValueType || nullableBaseType != null // Not a value type, OR is is a Nullable<>
			};

			for (var i = 0; i < Args.Count; i++)
			{
				var arg = Args[i];

				if (arg.ArgType == argType && arg.BindMethod != null)
				{
					binding.BindMethod = arg.BindMethod;
					break;
				}
			}

			Args.Add(binding);

			// If it's a valuetype or string, add a Bind method override plus also a string parse.
			var isString = argType == typeof(string);

			if (!argType.IsValueType && !isString)
			{
				// other objects, such as IEnumerables. These don't need special bind overrides.
				return binding;
			}

			var filt = typeof(Filter<T, ID>);
			var _argField = filt.GetField("_arg", BindingFlags.NonPublic | BindingFlags.Instance);

			if (BindStringMethod == null) {
				var bindMethod = TypeBuilder.DefineMethod("BindFromString", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, filt, _strTypeInArray);
				BindStringMethod = bindMethod.GetILGenerator();

				// Declare _arg as a local:
				BindStringMethod.DeclareLocal(typeof(int));
				BindStringMethod.Emit(OpCodes.Ldarg_0);
				BindStringMethod.Emit(OpCodes.Ldfld, _argField);
				BindStringMethod.Emit(OpCodes.Stloc_0);
			}

			if (binding.BindMethod == null)
			{
				// Create the bind method as well:
				binding.FirstMethodUser = true;
				var bindMethod = TypeBuilder.DefineMethod("Bind", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, filt, new Type[] { argType });
				binding.BindMethod = bindMethod.GetILGenerator();

				// Declare _arg as a local:
				binding.BindMethod.DeclareLocal(typeof(int));
				binding.BindMethod.Emit(OpCodes.Ldarg_0);
				binding.BindMethod.Emit(OpCodes.Ldfld, _argField);
				binding.BindMethod.Emit(OpCodes.Stloc_0);
			}

			// First, on the Bind method:

			// If(_arg == index) {
			//     Arg_INDEX = value;
			//     _arg++;
			// }

			var label = binding.BindMethod.DefineLabel();
			binding.BindMethod.Emit(OpCodes.Ldloc_0);
			binding.BindMethod.Emit(OpCodes.Ldc_I4, index);
			binding.BindMethod.Emit(OpCodes.Ceq);
			binding.BindMethod.Emit(OpCodes.Brfalse, label);
				// Set the value
				binding.BindMethod.Emit(OpCodes.Ldarg_0);
				binding.BindMethod.Emit(OpCodes.Ldarg_1);
				binding.BindMethod.Emit(OpCodes.Stfld, builder);
				// Increase _arg:
				binding.BindMethod.Emit(OpCodes.Ldarg_0);
				binding.BindMethod.Emit(OpCodes.Ldloc_0);
				binding.BindMethod.Emit(OpCodes.Ldc_I4_1);
				binding.BindMethod.Emit(OpCodes.Add);
				binding.BindMethod.Emit(OpCodes.Stfld, _argField);
			binding.BindMethod.Emit(OpCodes.Ldarg_0);
			binding.BindMethod.Emit(OpCodes.Ret);
			binding.BindMethod.MarkLabel(label);

			// Next, on the BindFromString method. These are only ever valuetypes, which should mean they have a Parse method that we can use.

			var parseType = (nullableBaseType != null ? nullableBaseType : argType);

			MethodInfo parseMethod;

			// Special case for date parsing, as we'd like to support either a date string or a numeric tick count.
			if (parseType == typeof(DateTime))
			{
				if (FilterAst._tryParseDate == null)
				{
					FilterAst._tryParseDate = typeof(FilterAst).GetMethod(nameof(FilterAst.TryParseDate), BindingFlags.Static | BindingFlags.Public);
				}

				parseMethod = FilterAst._tryParseDate;
			}
			else
			{
				parseMethod = isString ? null : parseType.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), parseType.MakeByRefType() }, null);
			}

			if (FilterAst._baseNullCheck == null)
			{
				FilterAst._baseNullCheck = typeof(FilterBase).GetMethod(nameof(FilterBase.NullCheck));
				FilterAst._checkParseSuccess = typeof(FilterAst)
					.GetMethod(nameof(FilterAst.CheckParseSuccess), BindingFlags.Static | BindingFlags.Public);
			}

			if (isString || parseMethod != null)
			{
				// We've got a parse method that can be used (or it just is a string anyway).

				label = BindStringMethod.DefineLabel();
				BindStringMethod.Emit(OpCodes.Ldloc_0);
				BindStringMethod.Emit(OpCodes.Ldc_I4, index);
				BindStringMethod.Emit(OpCodes.Ceq);
				BindStringMethod.Emit(OpCodes.Brfalse, label);
					
					if (argType.IsValueType)
					{
						if (nullableBaseType == null)
						{
							// Null is not permitted. If one is given, error:
							BindStringMethod.Emit(OpCodes.Ldarg_0);
							BindStringMethod.Emit(OpCodes.Ldarg_1);
							BindStringMethod.Emit(OpCodes.Callvirt, FilterAst._baseNullCheck);
						}
						else
						{
							// Check if user provided a null. If they did, set the target field to null.
							var nullLabel = BindStringMethod.DefineLabel();
							BindStringMethod.Emit(OpCodes.Ldarg_1);
							BindStringMethod.Emit(OpCodes.Ldnull);
							BindStringMethod.Emit(OpCodes.Ceq);
							BindStringMethod.Emit(OpCodes.Brfalse, nullLabel);
							// The given value is a null. Set a nullable representation of null to the field.
							BindStringMethod.Emit(OpCodes.Ldarg_0);
							BindStringMethod.Emit(OpCodes.Ldflda, builder);
							BindStringMethod.Emit(OpCodes.Initobj, argType);
							// Increase _arg:
							BindStringMethod.Emit(OpCodes.Ldarg_0);
							BindStringMethod.Emit(OpCodes.Ldloc_0);
							BindStringMethod.Emit(OpCodes.Ldc_I4_1);
							BindStringMethod.Emit(OpCodes.Add);
							BindStringMethod.Emit(OpCodes.Stfld, _argField);
							BindStringMethod.Emit(OpCodes.Ldarg_0);
							BindStringMethod.Emit(OpCodes.Ret);

							BindStringMethod.MarkLabel(nullLabel);
						}
					}

					// Set the value
					BindStringMethod.Emit(OpCodes.Ldarg_0);
					BindStringMethod.Emit(OpCodes.Ldarg_1);
					if (parseMethod != null)
					{
						// TryParse. The string is on the stack at the moment, so we just need to push a temp local ref:
						var tryParseScratchSpace = BindStringMethod.DeclareLocal(parseType);
						BindStringMethod.Emit(OpCodes.Ldloca, tryParseScratchSpace);
						BindStringMethod.Emit(OpCodes.Call, parseMethod);
						BindStringMethod.Emit(OpCodes.Call, FilterAst._checkParseSuccess);

						// CheckParse throws if it was invalid, so we can proceed to just put the parsed val straight onto the stack:
						BindStringMethod.Emit(OpCodes.Ldloc, tryParseScratchSpace);

						// If target type is nullable, the parseType isn't. Handle the new nullable struct:
						if (nullableBaseType != null)
						{
							var ctor = argType.GetConstructors();
							BindStringMethod.Emit(OpCodes.Newobj, ctor[0]);
						}
					}

					BindStringMethod.Emit(OpCodes.Stfld, builder);
					// Increase _arg:
					BindStringMethod.Emit(OpCodes.Ldarg_0);
					BindStringMethod.Emit(OpCodes.Ldloc_0);
					BindStringMethod.Emit(OpCodes.Ldc_I4_1);
					BindStringMethod.Emit(OpCodes.Add);
					BindStringMethod.Emit(OpCodes.Stfld, _argField);
				BindStringMethod.Emit(OpCodes.Ldarg_0);
				BindStringMethod.Emit(OpCodes.Ret);
				BindStringMethod.MarkLabel(label);
			}

			return binding;
		}

		/// <summary>
		/// Skips whitespaces
		/// </summary>
		public void ConsumeWhitespace()
		{
			while (Index < Query.Length && char.IsWhiteSpace(Query[Index]))
			{
				Index++;
			}
		}

		/// <summary>
		/// Peeks the next char
		/// </summary>
		/// <returns></returns>
		public char Peek()
		{
			if (Index >= Query.Length)
			{
				return '\0';
			}

			return Query[Index];
		}

		/// <summary>
		/// 
		/// </summary>
		public List<ContentField> GetIndexable()
		{
			// - Applies to function calls too -
			// (e.g. HasUserPermit() which is an indexable function as it uses the permit map OR the UserId one).

			// "indexable" state must be collected through to the root of a filter.
			// For example {IndexableNode} and {IndexableNode} => the "and" node is indexable, and uses the first one (it doesn't need both), or whichever is a shorter list.
			// {IndexableNode} or {non indexable} => the "or" node is _not_ indexable.
			// {IndexableNode} or {IndexableNode} => the "or" node is indexable, and does use both.

			// When multiple indices are returned to the root, it's effectively a union of all set results.

			// If the root is indexable at all, then some big performance gains can occur whilst resolving, particularly for live loops.
			// -> On lookup, it can use the map(s) as an index (or a content index if it's specifically marked as a DatabaseIndex field). 
			//    If there is more than one, it must use an ID collector to obtain and then uniquify the target IDs.
			//    This behaviour finally permits optimised lookups for both tags and categories simultaneously.

			// -> Whenever something is updated/ created with a mapping ref, 
			//    e.g. a message is created and it's mapped to video 4, then an inter-server broadcast marked as "video 4" is sent out.
			//  Any mappable filters are tuned directly in to a lookup, meaning there's a "video 4" index key with a linked list of listeners in it. 
			//  As maps are very specific and data safe by design, no filter or permission check needs to happen. It can simply send to all listeners on that map.

			if (Root == null)
			{
				// No root!
				return null;
			}

			return Root.GetIndexable();
		}

		private static int counter = 1;

		// Type.GetTypeFromHandle
		private static MethodInfo _getTypeFromHandle;

		/// <summary>
		/// True if there's any array args or Id collectors.
		/// </summary>
		public bool HasArrayNodes;

		/// <summary>
		/// 
		/// </summary>
		/// <returns>hasArrayNodes is true if there are any collector nodes or [?] args</returns>
		public Type ConstructType()
		{
			HasArrayNodes = false;
			Args = new List<ArgBinding>();
			AssemblyName assemblyName = new AssemblyName("GeneratedFilter_" + counter);
			counter++;
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

			// Create an inheriting type which reads the field as the key value:
			var filterT = typeof(Filter<T, ID>);
			TypeBuilder = moduleBuilder.DefineType("GeneratedFilter", TypeAttributes.Public, filterT);

			var matchMethod = TypeBuilder.DefineMethod(
				"Match",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
				typeof(bool), new Type[] { typeof(Context), typeof(object), typeof(bool) }
			);

			ILGenerator writerBody = matchMethod.GetILGenerator();

			if (Root == null)
			{
				// No filter - true:
				writerBody.Emit(OpCodes.Ldc_I4, 1);
			}
			else
			{
				Root.Emit(writerBody, this);
			}

			writerBody.Emit(OpCodes.Ret);

			var baseFail = filterT.GetMethod(nameof(Filter<T, ID>.Fail));

			if (_getTypeFromHandle == null)
			{
				_getTypeFromHandle = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle));
			}

			for (var i = 0; i < Args.Count; i++)
			{
				// Close them off:
				var arg = Args[i];

				if (arg.FirstMethodUser)
				{
					// At the bottom of the bind method is a fail method which indicates current arg X is not of the current type.
					arg.BindMethod.Emit(OpCodes.Ldarg_0);
					arg.BindMethod.Emit(OpCodes.Ldtoken, arg.ArgType);
					arg.BindMethod.Emit(OpCodes.Call, _getTypeFromHandle);
					arg.BindMethod.Emit(OpCodes.Call, baseFail);
					arg.BindMethod.Emit(OpCodes.Ldarg_0);
					arg.BindMethod.Emit(OpCodes.Ret);
				}
			}

			if (BindStringMethod != null)
			{
				// At the bottom of the bind string method is a fail method which indicates current arg X is not of the current type.
				BindStringMethod.Emit(OpCodes.Ldarg_0);
				BindStringMethod.Emit(OpCodes.Ldtoken, typeof(string));
				BindStringMethod.Emit(OpCodes.Call, _getTypeFromHandle);
				BindStringMethod.Emit(OpCodes.Call, baseFail);
				BindStringMethod.Emit(OpCodes.Ldarg_0);
				BindStringMethod.Emit(OpCodes.Ret);
			}

			// Bake the type:
			return TypeBuilder.CreateType();
		}

		/// <summary>
		/// True if there's more tokens
		/// </summary>
		/// <returns></returns>
		public bool More()
		{
			return Index < Query.Length;
		}

		/// <summary>
		/// Peeks the next char
		/// </summary>
		/// <returns></returns>
		public char Peek(int offset)
		{
			if ((Index + offset) >= Query.Length)
			{
				return '\0';
			}

			return Query[Index + offset];
		}

		/// <summary>
		/// Substring from given index to the current index (inclusive of the char at both start + index).
		/// </summary>
		/// <param name="start"></param>
		/// <returns></returns>
		public string SubstringFrom(int start)
		{
			return Query.Substring(start, Index - start);
		}

		/// <summary>
		/// Parse a token in the tree
		/// </summary>
		/// <returns></returns>
		public FilterTreeNode<T, ID> ParseAny(int groupState)
		{
			// Consume whitespace:
			ConsumeWhitespace();

			// Peek the token based on phase:
			var current = Peek();

			FilterTreeNode<T,ID> firstNode;

			// Entry - only a (, ! or a member are permitted.
			if (current == '(')
			{
				// Consume the bracket:
				Index++;
				ConsumeWhitespace();
				firstNode = ParseAny(2);

				if (Peek() != ')')
				{
					throw new PublicException(
						"Unexpected character in filter string. Expected a ) at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				Index++;
				ConsumeWhitespace();
			}
			else if (current == '!')
			{
				// Consume the !:
				Index++;
				ConsumeWhitespace();
				current = Peek();

				if (current != '(')
				{
					throw new PublicException(
						"! can only be followed immediately by a bracket in this context " + Index +" in " + Query,
						"filter_invalid"
					);
				}

				// Read the bracket:
				Index++;
				ConsumeWhitespace();
				firstNode = ParseAny(2);
				if (Peek() != ')')
				{
					throw new PublicException(
						"Unexpected character in filter string. Expected a ) at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				Index++;
				ConsumeWhitespace();

				firstNode = new OpFilterTreeNode<T, ID>() {
					A = firstNode,
					Operation = "not"
				};
			}
			else
			{
				// Can only be a member (either a field or a method call).
				if (!char.IsLetter(current))
				{
					throw new PublicException(
						"Unexpected character in filter string. Expected a letter but got '" + current + "' at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				// Read member:
				var member = new MemberFilterTreeNode<T, ID>();
				firstNode = member.Parse(this); // Consumes whitespace internally
			}

			// If the next char is ) or , or just EOF, return member.
			current = Peek();

			if (groupState == 0)
			{
				if (!More())
				{
					return firstNode;
				}
			}
			else if (groupState == 1) // Inside args like (a,b)
			{
				if (!More() || current == ',' || current == ')')
				{
					return firstNode;
				}
			}
			else if (groupState == 2) // Inside brackets (logic statement)
			{
				if (!More() || current == ')')
				{
					return firstNode;
				}
			}

			// There's more. Next must be an opcode, which means we're returning an Op node.
			var node = ParseOp();
			node.A = firstNode;
			ConsumeWhitespace();

			// An any node is acceptable only if the opcode is logical:
			if (node.IsLogic())
			{
				// A and {ANY}
				var val = ParseAny(groupState);
				node.B = val;
			}
			else
			{
				// Can only be a value here
				var val = ParseValue();
				node.B = val;
			}

			ConsumeWhitespace();
			current = Peek();

			// Check if we're terminating:
			if (groupState == 0)
			{
				if (!More())
				{
					return node;
				}
			}
			else if (groupState == 1) // Inside args like (a,b)
			{
				if (!More() || current == ',' || current == ')')
				{
					return node;
				}
			}
			else if (groupState == 2) // Inside (logic staetment)
			{
				if (!More() || current == ')')
				{
					return node;
				}
			}

			// There's more - it can only be an opcode though:
			var chain = ParseOp();

			if (chain == null || !chain.IsLogic())
			{
				throw new PublicException("Can only chain logic opcodes. Encountered an invalid sequence at " + Index + " in " + Query, "filter_invalid");
			}

			ConsumeWhitespace();
			var child = ParseAny(groupState);
			chain.A = node;
			chain.B = child;
			return chain;
		}

		/// <summary>
		/// Parse an operation
		/// </summary>
		/// <returns></returns>
		public OpFilterTreeNode<T, ID> ParseOp()
		{
			// Operator such as != or "and" or "startsWith" or "endsWith" or "contains":

			// If this char is an ascii letter, read operator until whitespace.
			var current = Peek();

			var opStart = Index;
			
			if (char.IsLetter(current))
			{
				Index++;

				while (Index < Query.Length && !char.IsWhiteSpace(Query[Index]))
				{
					Index++;
				}

			}
			else if (current == '<' || current == '>')
			{
				Index++;
				if (Peek() == '=')
				{
					Index++;
				}

			}
			else if (current == '!')
			{
				Index++;
				if (Peek() != '=')
				{
					// == is invalid
					throw new PublicException("Expected != but only saw the !. Was encountered at index " + Index + " in " + Query, "filter_invalid");
				}
				Index++;
			}
			else if (current == '=')
			{
				Index++;
				if (Peek() == '=')
				{
					// == is invalid
					throw new PublicException("Don't use == in a filter query string. Was encountered at index " + Index + " in " + Query, "filter_invalid");
				}
			}

			// OpCode:
			var opCode = SubstringFrom(opStart).ToLower();

			return new OpFilterTreeNode<T, ID>() {
				Operation = opCode
			};
		}

		private static readonly string NoConstants = "Constant values aren't permitted in this filter, for both performance and to make security easier for you. You'll need to use ? and pass the value as an arg instead.";

		/// <summary>
		/// Parses a constant-like value.
		/// </summary>
		/// <returns></returns>
		public FilterTreeNode<T, ID> ParseValue()
		{
			// Value - typically "?" but can be a const if const is permissable

			// Consume whitespace:
			ConsumeWhitespace();

			// Peek the token based on phase:
			var current = Peek();
			
			// Consume spaces:
			ConsumeWhitespace();

			if (current == '?')
			{
				// Arg
				if (!AllowArgs)
				{
					throw new PublicException(
						"Can't use an argument '?' in this filter. One was found at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				Index++;
				return new ArgFilterTreeNode<T, ID>() { Id = ArgIndex++ };
			}

			if (current == '[' && Peek(1) == '?' && Peek(2) == ']')
			{
				// Arg
				if (!AllowArgs)
				{
					throw new PublicException(
						"Can't use an argument '[?]' in this filter. One was found at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				Index+=3;
				HasArrayNodes = true;
				return new ArgFilterTreeNode<T, ID>() { Id = ArgIndex++, Array=true };
			}

			if (char.IsDigit(current) || current == '.')
			{
				// Either int/decimal
				var start = Index;
				var isDecimal = current == '.';
				Index++;

				while (More() && (char.IsDigit(Query[Index]) || Query[Index] =='.'))
				{
					if (Query[Index] == '.')
					{
						isDecimal = true;
					}
					Index++;
				}

				var str = Query.Substring(start, Index - start);

				if (!AllowConstants)
				{
					throw new PublicException(
						NoConstants + " Encountered '" + str + "' at position " + start + " in " + Query,
						"filter_invalid"
					);
				}

				if (isDecimal)
				{
					return new DecimalFilterTreeNode<T, ID>() { Value = Decimal.Parse(str) };
				}

				return new NumberFilterTreeNode<T, ID>() { Value = long.Parse(str) };
			}
			else if (current == 'T' || current == 't')
			{
				// "true"
				ExpectText("rue", "RUE");

				if (!AllowConstants)
				{
					throw new PublicException(
						NoConstants + " Encountered a use of true at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				Index += 4;

				return new BoolFilterTreeNode<T, ID>() { Value = true };
			}
			else if (current == 'F' || current == 'f')
			{
				// "false"
				ExpectText("alse", "ALSE");
				
				if (!AllowConstants)
				{
					throw new PublicException(
						NoConstants + " Encountered a use of false at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}

				Index += 5;

				return new BoolFilterTreeNode<T, ID>() { Value = false };
			}
			else if (current == 'N' || current == 'n')
			{
				// "null"
				ExpectText("ull", "ULL");

				if (!AllowConstants)
				{
					throw new PublicException(
						NoConstants + " Encountered a use of null at position " + Index + " in " + Query,
						"filter_invalid"
					);
				}
				
				Index += 4;
				return new NullFilterTreeNode<T, ID>();
			}
			else if (current == '"')
			{
				// a string. Only thing that requires escaping is "
				return ReadString('"', AllowConstants);
			}
			else if (current == '\'')
			{
				// a string. Only thing that requires escaping is '
				return ReadString('\'', AllowConstants);
			}

			throw new PublicException(
				"Unexpected character in filter string. Expected a value but got '" + current + "' at position " + Index + " in " + Query,
				"filter_invalid"
			);
		}

		private StringBuilder sb = new StringBuilder();

		/// <summary>
		/// Read a string from this AST
		/// </summary>
		/// <param name="terminal"></param>
		/// <param name="allowConstants"></param>
		/// <returns></returns>
		public StringFilterTreeNode<T, ID> ReadString(char terminal, bool allowConstants)
		{
			// Read off the first quote:
			var start = Index;
			Index++;
			bool escaped = false;
			while (Index < Query.Length)
			{
				var cur = Query[Index];
				Index++;

				if (escaped)
				{
					escaped = false;
					sb.Append(cur);
				}
				else if (cur == '\\')
				{
					escaped = true;
				}
				else if (cur == terminal)
				{
					break;
				}
				else
				{
					sb.Append(cur);
				}
			}

			var result = sb.ToString();
			sb.Clear();

			if (!allowConstants)
			{
				throw new PublicException(
					NoConstants + " Encountered the string '" + result + "' at position " + start + " in " + Query,
					"filter_invalid"
				);
			}

			return new StringFilterTreeNode<T, ID>() { Value = result };
		}

		private void ExpectText(string lc, string uc)
		{
			for (var i = 0; i < lc.Length; i++)
			{
				var current = Peek(i + 1);
				if (current != lc[i] && current != uc[i])
				{
					Fail(lc, Index + i + i);
				}
			}
		}

		private void Fail(string msg, int index)
		{
			throw new PublicException(
				"Unexpected character in filter string. Expected " + msg + " but got '" + Query[index] + "' at position " + index + " in " + Query,
				"filter_invalid"
			);
		}

		/// <summary>
		/// 
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();
			if (Root != null)
			{
				Root.ToString(builder);
			}
			return builder.ToString();
		}
	}

	/// <summary>
	/// Base tree node
	/// </summary>
	public partial class FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		/// <summary>
		/// True if the node has an on statement.
		/// Most nodes return false - only and will accept one as a child.
		/// </summary>
		/// <returns></returns>
		public virtual bool HasRootedOnStatement()
		{
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="ast"></param>
		public virtual void Emit(ILGenerator generator, FilterAst<T, ID> ast)
		{
			throw new NotImplementedException("Filter uses a " + GetType() + " node that can't be emitted: " + ast.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void ToString(StringBuilder builder)
		{
		}
		
		/// <summary>
		/// If not null, this node is indexable (using the given indices).
		/// When more than one is returned, it's because it is a union of all the id's in those indices.
		/// </summary>
		/// <returns></returns>
		public virtual List<ContentField> GetIndexable()
		{
			return null;
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
		/// string.Equals static method
		/// </summary>
		private static MethodInfo _strEquals;

		/// <summary>
		/// string.StartsWith(str) static method
		/// </summary>
		private static MethodInfo _strStartsWith;

		/// <summary>
		/// string.EndsWith(str) static method
		/// </summary>
		private static MethodInfo _strEndsWith;

		/// <summary>
		/// string.Contains(str) method
		/// </summary>
		private static MethodInfo _strContains;

		/// <summary>
		/// Node a
		/// </summary>
		public FilterTreeNode<T, ID> A;
		
		/// <summary>
		/// Node b
		/// </summary>
		public FilterTreeNode<T, ID> B;

		/// <summary>
		/// The actual operation (lowercase)
		/// </summary>
		public string Operation; // <, >, =, "or", etc

		/// <summary>
		/// True if this is a logic operation
		/// </summary>
		public bool IsLogic()
		{
			return Operation == "and" || Operation == "&&" ||
				Operation == "or" || Operation == "||" ||
				Operation == "not";
		}
		
		/// <summary>
		/// True if the node has an on statement.
		/// Most nodes return false - only and will accept one as a child.
		/// </summary>
		/// <returns></returns>
		public override bool HasRootedOnStatement()
		{
			if (Operation == "and" || Operation == "&&")
			{
				return A.HasRootedOnStatement() || B.HasRootedOnStatement();
			}

			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="ast"></param>
		public override void Emit(ILGenerator generator, FilterAst<T, ID> ast)
		{
			var operation = Operation == null ? "" : Operation.Trim().ToLower();

			if (operation == "and" || operation == "&&")
			{
				A.Emit(generator, ast);

				// If it was false, put a 0 on the stack. Don't evaluate B.
				var falseResult = generator.DefineLabel();
				var afterFalse = generator.DefineLabel();
				generator.Emit(OpCodes.Brfalse, falseResult);
				B.Emit(generator, ast);
				generator.Emit(OpCodes.Brfalse, falseResult);
				generator.Emit(OpCodes.Ldc_I4, 1);
				generator.Emit(OpCodes.Br, afterFalse);
				generator.MarkLabel(falseResult);
				generator.Emit(OpCodes.Ldc_I4, 0);
				generator.MarkLabel(afterFalse);
				return;
			}
			else if (operation == "or" || operation == "||")
			{
				var trueResult = generator.DefineLabel();
				var afterTrue = generator.DefineLabel();
				A.Emit(generator, ast);
				generator.Emit(OpCodes.Brtrue, trueResult);
				B.Emit(generator, ast);
				generator.Emit(OpCodes.Brtrue, trueResult);
				generator.Emit(OpCodes.Ldc_I4, 0);
				generator.Emit(OpCodes.Br, afterTrue);
				generator.MarkLabel(trueResult);
				generator.Emit(OpCodes.Ldc_I4, 1);
				generator.MarkLabel(afterTrue);
				return;
			}
			else if (operation == "not")
			{
				A.Emit(generator, ast);
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Ceq);
				return;
			}
			
			// All other operations require a member on the left hand side "Name=?" etc.

			// Field=VALUE, FIELD>=VALUE etc.
			var member = A as MemberFilterTreeNode<T, ID>;
			
			if (member == null || member.Field == null || (member.Field.VirtualInfo == null && member.Field.FieldInfo == null))
			{
				throw new Exception("Your filter has a syntax error - operation '" + Operation + "' must be performed as Member=Value, not Value=Member.");
			}

			if (operation == "=")
			{
				EmitEquals(generator, ast, member, B);
			}
			else if (operation == "!=")
			{
				EmitEquals(generator, ast, member, B);
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Ceq);
			}
			else if (operation == ">=")
			{
				// A>=B is the same as !A<B
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				// The inputs must be the same type here and they must be both numeric.
				if (memberType != valueType)
				{
					throw new Exception("Unsupported >= usage: Inputs must be the same type and not arrays.");
				}

				EmitLessThan(generator, valueType);

				// Must use == 0 to invert them.
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Ceq);
			}
			else if (operation == "<=")
			{
				// A<=B is the same as !A>B
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				// The inputs must be the same type here and they must be both numeric.
				if (memberType != valueType)
				{
					throw new Exception("Unsupported >= usage: Inputs must be the same type and not arrays.");
				}
				EmitGreaterThan(generator, valueType);

				// Must use == 0 to invert them.
				generator.Emit(OpCodes.Ldc_I4_0);
				generator.Emit(OpCodes.Ceq);
			}
			else if (operation == ">")
			{
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				// The inputs must be the same type here and they must be both numeric.
				if (memberType != valueType)
				{
					throw new Exception("Unsupported >= usage: Inputs must be the same type and not arrays.");
				}

				EmitGreaterThan(generator, valueType);
			}
			else if (operation == "<")
			{
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				// The inputs must be the same type here and they must be both numeric.
				if (memberType != valueType)
				{
					throw new Exception("Unsupported >= usage: Inputs must be the same type and not arrays.");
				}

				EmitLessThan(generator, valueType);
			}
			else if (operation == "contains")
			{
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);
				
				if (memberType == typeof(string))
				{
					if (valueType != typeof(string))
					{
						throw new PublicException("As the field is a string, you can only use contains with a string value.", "filter_invalid");
					}

					if (_strContains == null)
					{
						_strContains = typeof(FilterAst)
							.GetMethod(
								nameof(FilterAst.GuardedContains), 
								BindingFlags.Public | BindingFlags.Static, 
								null, 
								new Type[] { typeof(string), typeof(string) }, 
								null
							);
					}

					generator.Emit(OpCodes.Call, _strContains);
					return;
				}

				// Check if memberType is enumerable
				if (memberType != typeof(List<ulong>))
				{
					throw new PublicException("Contains can only be used on strings and array-like fields.", "filter_invalid");
				}

				if (IsEnumerable(valueType))
				{
					// Exact match. This assumes that valueArray is coerseable to IEnumerable of specifically ulong.
					var matchMethod = typeof(Filter<T, ID>).GetMethod(nameof(Filter<T, ID>.SetContainsAll));
					generator.Emit(OpCodes.Call, matchMethod);
				}
				else
				{
					// Contains (singular value).

					// Convert the singular value to a ulong:
					TypeIOEngine.CoerseToUlong(generator, valueType);

					var matchMethod = typeof(Filter<T, ID>).GetMethod(nameof(Filter<T, ID>.SetContains));
					generator.Emit(OpCodes.Call, matchMethod);
				}
			}
			else if (operation == "containsany")
			{
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				if (memberType == typeof(string))
				{
					if (valueType != typeof(string))
					{
						throw new PublicException("As the field is a string, you can only use ContainsAny with a string value.", "filter_invalid");
					}

					if (_strContains == null)
					{
						_strContains = typeof(FilterAst).GetMethod(nameof(FilterAst.GuardedContains), BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string) }, null);
					}

					generator.Emit(OpCodes.Call, _strContains);
					return;
				}

				// Check if memberType is enumerable
				if (memberType != typeof(List<ulong>))
				{
					throw new PublicException("Contains can only be used on strings and array-like fields.", "filter_invalid");
				}

				if (IsEnumerable(valueType))
				{
					// Exact match. This assumes that valueArray is coerseable to IEnumerable of specifically ulong.
					var matchMethod = typeof(Filter<T, ID>).GetMethod(nameof(Filter<T, ID>.SetContainsAny));
					generator.Emit(OpCodes.Call, matchMethod);
				}
				else
				{
					// Contains (singular value).

					// Convert the singular value to a ulong:
					TypeIOEngine.CoerseToUlong(generator, valueType);

					var matchMethod = typeof(Filter<T, ID>).GetMethod(nameof(Filter<T, ID>.SetContains));
					generator.Emit(OpCodes.Call, matchMethod);
				}
			}
			else if (operation == "startswith")
			{
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				if (valueType != typeof(string) || memberType != typeof(string))
				{
					throw new PublicException("StartsWith can only be used on strings.", "filter_invalid");
				}

				if (_strStartsWith == null)
				{
					_strStartsWith = typeof(string)
						.GetMethod("StartsWith", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
				}

				generator.Emit(OpCodes.Call, _strStartsWith);
			}
			else if (operation == "endswith")
			{
				var memberType = ast.EmitReadValue(generator, member, null);
				var valueType = ast.EmitReadValue(generator, B, memberType);

				if (valueType != typeof(string) || memberType != typeof(string))
				{
					throw new PublicException("EndsWith can only be used on strings.", "filter_invalid");
				}

				if (_strEndsWith == null)
				{
					_strEndsWith = typeof(string).GetMethod("EndsWith", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
				}

				generator.Emit(OpCodes.Call, _strEndsWith);
			}
			else
			{
				throw new Exception("Your filter has invalid syntax. Unrecognised operation: " + Operation);
			}
		}

		private void EmitGreaterThan(ILGenerator generator, Type valueType)
		{
			var greaterThanMethod = valueType.GetMethod("op_GreaterThan", BindingFlags.Static | BindingFlags.Public, null, new Type[] {
					valueType,
					valueType
				}, null);

			if (greaterThanMethod != null)
			{
				generator.Emit(OpCodes.Call, greaterThanMethod);
				return;
			}
			
			// Is unsigned?
			if (valueType == typeof(ushort) || valueType == typeof(uint) || valueType == typeof(ulong) || valueType == typeof(byte))
			{
				generator.Emit(OpCodes.Cgt_Un);
			}
			else if(valueType == typeof(short) || valueType == typeof(int) || valueType == typeof(long) || valueType == typeof(byte))
			{
				generator.Emit(OpCodes.Cgt);
			}
			else
			{
				throw new PublicException("This filter operation can only be used on numeric types.", "filter/numeric_operator");
			}
		}

		private void EmitLessThan(ILGenerator generator, Type valueType)
		{
			// If it has a dedicated LessThan method:
			var lessThanMethod = valueType.GetMethod("op_LessThan", BindingFlags.Static | BindingFlags.Public, null, new Type[] {
				valueType,
					valueType
				}, null);

			if (lessThanMethod != null)
			{
				generator.Emit(OpCodes.Call, lessThanMethod);
				return;
			}
			
			// Is unsigned?
			if (valueType == typeof(ushort) || valueType == typeof(uint) || valueType == typeof(ulong) || valueType == typeof(byte))
			{
				generator.Emit(OpCodes.Clt_Un);
			}
			else if (valueType == typeof(short) || valueType == typeof(int) || valueType == typeof(long) || valueType == typeof(byte))
			{
				generator.Emit(OpCodes.Clt);
			}
			else
			{
				throw new PublicException("This filter operation can only be used on numeric types.", "filter/numeric_operator");
			}

		}

		/// <summary>
		/// True if the given type is enumerable/ array-like (but not a string)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsEnumerable(Type type)
		{
			return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
		}

		private void EmitEquals(ILGenerator generator, FilterAst<T, ID> ast, MemberFilterTreeNode<T, ID> member, FilterTreeNode<T, ID> value)
		{
			var memberType = ast.EmitReadValue(generator, member, null);
			var valueType = ast.EmitReadValue(generator, value, memberType);

			var memberArray = IsEnumerable(memberType);
			var valueArray = IsEnumerable(valueType);

			// valueType can be an array, in which case it is equiv to IN(..).
			// if memberType is also an array (a list field), then it is an exact match.
			if (memberArray)
			{
				if (memberType != typeof(List<ulong>))
				{
					// Only supporting ulong lists (virtual ListAs field compatible) for now otherwise
					// the permutations set is huge and 99% would never be used anyway!
					throw new PublicException("This filter field is currently unsupported.", "filter/unsupported");
				}

				if (valueArray)
				{
					// Exact match. This assumes that valueArray is coerseable to IEnumerable of specifically ulong.
					var matchMethod = typeof(Filter<T, ID>).GetMethod(nameof(Filter<T, ID>.SetEquals));
					generator.Emit(OpCodes.Call, matchMethod);
				}
				else
				{
					// Contains.

					// Convert the singular value to a ulong:
					TypeIOEngine.CoerseToUlong(generator, valueType);

					var matchMethod = typeof(Filter<T, ID>).GetMethod(nameof(Filter<T, ID>.SetContains));
					generator.Emit(OpCodes.Call, matchMethod);
				}

				return;
			}
			else if (valueArray)
			{
				// IN(..) equiv. Get the IEnumerable<T> T:
				var itemType = GetEnumerableType(valueType);

				var matchMethod = typeof(Filter<T, ID>)
					.GetMethod(nameof(Filter<T, ID>.MemberInSet))
					.MakeGenericMethod(new Type[] {
						itemType
					});
				generator.Emit(OpCodes.Call, matchMethod);

				return;
			}

			// Source is a singular value.
			if (memberType == typeof(string))
			{
				if (valueType != typeof(string))
				{
					// Convert the value to a string.
					// The value is currently on the top of the stack so we can do so.
					var toStr = valueType.GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance);

					if (toStr == null)
					{
						throw new PublicException("Cannot convert value type to a string for checking if it equal to a string field.", "filter/invalid");
					}

					if (valueType.IsValueType)
					{
						// Store it in a local to call its toStr method.
						var loc = generator.DeclareLocal(valueType);
						generator.Emit(OpCodes.Stloc, loc);
						generator.Emit(OpCodes.Ldloca, loc);
						generator.Emit(OpCodes.Callvirt, toStr);
					}
					else
					{
						// Directly call toStr:
						generator.Emit(OpCodes.Callvirt, toStr);
					}
				}

				if (_strEquals == null)
				{
					_strEquals = typeof(string).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string) }, null);
				}

				generator.Emit(OpCodes.Call, _strEquals);
				return;
			}

			var equalsMethod = memberType.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public, null, new Type[] {
				memberType,
				valueType
			}, null);

			if (equalsMethod != null)
			{
				generator.Emit(OpCodes.Call, equalsMethod);
				return;
			}

			if (memberType == valueType)
			{
				// Common situation. If the type is Ceq compatible then proceed with it.
				if (memberType.IsValueType && !IsCeqCompatible(memberType))
				{
					throw new PublicException("Invalid filter: unable to combine these field types during equality.", "filter/invalid");
				}

				generator.Emit(OpCodes.Ceq);
			}
			else
			{
				// Cast valueType towards being memberType.
				throw new PublicException("Invalid filter: Field coersion is not supported at the moment.", "filter/invalid");
			}
		}

		/// <summary>
		/// Searches for the IEnumerable[T] interface description on the given type.
		/// </summary>
		/// <param name="valueType"></param>
		/// <returns></returns>
		private Type GetEnumerableType(Type valueType)
		{
			if (valueType.IsGenericType)
			{
				var def = valueType.GetGenericTypeDefinition();

				if (def == typeof(IEnumerable<>))
				{
					return valueType.GetGenericArguments()[0];
				}
			}

			var interfaces = valueType.GetInterfaces();

			foreach (var intf in interfaces)
			{
				if (intf.IsGenericType)
				{
					var def = GetEnumerableType(intf);

					if (def != null)
					{
						return def;
					}
				}
			}

			// Try its base type otherwise.
			var bt = valueType.BaseType;

			if (bt == null || bt == typeof(object))
			{
				return null;
			}

			return GetEnumerableType(bt);
		}

		private bool IsCeqCompatible(Type type)
		{
			if (!type.IsValueType)
			{
				return true;
			}

			if (type.IsEnum)
			{
				return true;
			}

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
					return true;

				default:
					return false; // Structs and other unsupported value types
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			if (Operation == "not")
			{
				builder.Append("!(");
				A.ToString(builder);
				builder.Append(')');
				return;
			}

			builder.Append('(');
			A.ToString(builder);
			builder.Append(' ');
			builder.Append(Operation);
			builder.Append(' ');
			B.ToString(builder);
			builder.Append(')');
		}

		/// <summary>
		/// If not null, this node is indexable (using the given indices).
		/// When more than one is returned, it's because it is a union of all the id's in those indices.
		/// </summary>
		/// <returns></returns>
		public override List<ContentField> GetIndexable()
		{
			if (Operation == "and" || Operation == "&&")
			{
				// Logical AND is indexable if either one is.
				var a = A.GetIndexable();
				var b = B.GetIndexable();

				if (a == null && b == null)
				{
					return null;
				}

				if (a != null)
				{
					// If they both are, it uses the "shortest".
					// Note that the shortest is not necessarily the length of the list - it can also be whichever one is 
					if (b != null)
					{
						if (a.Count == b.Count)
						{
							// If they're both the same length, pick one based on them being a ListAs field or not.
							// For example, "Id=4 and Tags={An_Array}". The Id=4 one wins, 
							// because it is more specific and therefore a better index to use.

							var aLists = 0;
							var bLists = 0;
							for (var x = 0; x < a.Count; x++)
							{
								if (a[x].IsVirtual && a[x].VirtualInfo.IsList)
								{
									aLists++;
								}
								if (b[x].IsVirtual && b[x].VirtualInfo.IsList)
								{
									bLists++;
								}
							}

							if (aLists == bLists)
							{
								return a;
							}

							// Fewer list entries wins
							return aLists < bLists ? a : b;
						}

						return a.Count < b.Count ? a : b;
					}

					// a only
					return a;
				}

				// b only
				return b;
			}
			else if (Operation == "or" || Operation == "||")
			{
				// "has these tags OR has these categories" for example.
				// Must be a union of both.
				var a = A.GetIndexable();
				var b = B.GetIndexable();

				if (a == null && b == null)
				{
					// Neither anyway
					return null;
				}

				if (a != null)
				{
					if (b != null)
					{
						var set = new List<ContentField>();

						// Merge:
						set.AddRange(a);
						set.AddRange(b);
						return set;
					}

					// a only
					return a;
				}

				// b only
				return b;
			}

			return null;
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
		/// The member name
		/// </summary>
		public string Name;

		/// <summary>
		/// The field to use.
		/// </summary>
		public ContentField Field;

		/// <summary>
		/// The field to use (if it's a context one).
		/// </summary>
		public ContextFieldInfo ContextField;

		/// <summary>
		/// True if this is a method call.
		/// </summary>
		public bool IsMethod
		{
			get {
				return Args != null;
			}
		}

		/// <summary>
		/// If method call, the args
		/// </summary>
		public List<FilterTreeNode<T, ID>> Args;

		/// <summary>
		/// True if this is a context field.
		/// </summary>
		public bool OnContext;

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append(Name);
			if(Args != null)
			{
				builder.Append('(');
				for (var i = 0; i < Args.Count; i++)
				{
					if (i != 0)
					{
						builder.Append(", ");
					}
					Args[i].ToString(builder);
				}
				builder.Append(')');
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public FilterTreeNode<T,ID> Parse(FilterAst<T,ID> ast)
		{
			var start = ast.Index;

			// Consume first letter:
			ast.Index++;

			// Keep reading letters and numbers until either a ( or whitespace.
			while (ast.More())
			{
				var current = ast.Peek();

				if (char.IsHighSurrogate(current))
				{
					ast.Index++;
					if (char.IsLowSurrogate(ast.Peek()))
					{
						ast.Index++;
					}
				}
				else if (char.IsLetterOrDigit(current))
				{
					ast.Index++;
				}
				else if (current == '(')
				{
					Args = new List<FilterTreeNode<T, ID>>();
					break;
				}
				else
				{
					break;
				}
			}

			// Name is start->index:
			Name = ast.SubstringFrom(start);

			if (Args != null)
			{
				// Skip the opening bracket:
				ast.Index++;
				ast.ConsumeWhitespace();

				if (Name == "On" || Name == "on")
				{
					Name = "on";

					// First arg should be a type name:
					start = ast.Index;

					while (ast.More())
					{
						var current = ast.Peek();

						if (char.IsHighSurrogate(current))
						{
							ast.Index++;
							if (char.IsLowSurrogate(ast.Peek()))
							{
								ast.Index++;
							}
						}
						else if (char.IsLetterOrDigit(current))
						{
							ast.Index++;
						}
						else
						{
							break;
						}
					}

					var typeName = ast.SubstringFrom(start);

					if (ast.Peek() != ',')
					{
						throw new PublicException("On() requires at least two parameters", "filter_invalid");
					}

					Args.Add(new StringFilterTreeNode<T, ID>() { Value = typeName });

					// Consume next method arg, which must only be an argument value:
					ast.Index++;
					ast.ConsumeWhitespace();

					if (ast.Peek() != '?')
					{
						throw new PublicException("The second parameter of On() can only be a ?", "filter_invalid");
					}

					ast.Index++;
					ast.ConsumeWhitespace();

					Args.Add(new ArgFilterTreeNode<T, ID>() { Id = ast.ArgIndex++ });

					if (ast.Peek() == ',')
					{
						// 3rd is the map name, a regular string:
						ast.Index++;
						ast.ConsumeWhitespace();

						var terminal = ast.Peek();

						if (terminal == '"' || terminal == '\'')
						{
							Args.Add(ast.ReadString(terminal, true));
						}
						else
						{
							throw new PublicException("The optional third parameter of On() can only be a constant string", "filter_invalid");
						}
					}

					if (ast.Peek() != ')')
					{
						throw new PublicException("Too many parameters given to On(). It accepts either two or three.", "filter_invalid");
					}
				}

				while (ast.Peek() != ')')
				{
					Args.Add(ast.ParseValue());
					ast.ConsumeWhitespace();
					var current = ast.Peek();
					if (!ast.More() || (current != ',' && current != ')'))
					{
						// Fail scenario
						throw new PublicException("Incomplete method call in filter at index " + ast.Index, "filter_invalid");
					}

					if (current == ',')
					{
						ast.Index++;
						ast.ConsumeWhitespace();
					}
				}

				// Consume the closing bracket:
				ast.Index++;
			}

			var resolved = Resolve(ast);

			// Consume whitespace:
			ast.ConsumeWhitespace();

			return resolved;
		}

		/// <summary>
		/// Resolves the field/ method from the name
		/// </summary>
		/// <param name="ast"></param>
		public FilterTreeNode<T, ID> Resolve(FilterAst<T, ID> ast)
		{
			// It's a field which must exist on typeof(T). It can be a virtual list field too.
			// If it is a virtual list field, OR it _has_ a virtual field tied to it, then this field is "mappable".
			var lcName = Name.ToLower();

			if (Args != null)
			{
				var func = FilterFunctions.Get<T, ID>(lcName);

				if (func == null)
				{
					throw new PublicException("Couldn't find filter function '" + Name + "'.", "filter_invalid");
				}

				return func(this, ast);
			}

			if (OnContext)
			{
				if (!ContextFields.Fields.TryGetValue(lcName, out ContextFieldInfo ctxField))
				{
					// Not a valid ctx field!
					throw new PublicException("Couldn't find filter field '" + Name + "' on context.", "filter_invalid");
				}

				ContextField = ctxField;
				return this;
			}

			var fields = ast.Service.GetContentFields();
			if (!fields.TryGetValue(lcName, out ContentField field))
			{
				if (!ContentFields.GlobalVirtualFields.TryGetValue(lcName, out field))
				{
					throw new PublicException("Couldn't find filter field '" + Name + "' on this type, or a global virtual field by the same name.", "filter_invalid");
				}

				// Some mappings are handled via ID fields on the actual target type.
				// We can short those out and replace them with regular array fields instead.

				// For example, imagine a virtual list field called CallToActions, and it is being used in a filter on a Video.
				// "CallToActions=[?]"
				// However the actual video only has a CallToActionId field, and only stores one.
				// In this situation, a mapping is "not required" and the "local" field will be that CallToActionId one.
				// Ultimately, that means the filter executes the same as this faster one:
				// "CallToActionId=[?]"

				var localMappedField = field.GetIdFieldIfMappingNotRequired(fields);

				if (localMappedField != null)
				{
					// Nice! No mapping is required because this type has the field on it.
					field = localMappedField;
				}

				// Either way, it has an array node.
				ast.HasArrayNodes = true;
			}
			else if (field.PropertyInfo != null)
			{
				// Can't use a filter on properties, as it isn't compatible with a remote database
				throw new Exception("Can't use " + Name + " in a filter. Only fields are compatible, not properties.");
			}

			// Field is the field to use.
			Field = field;

			return this;
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
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append("IsIncluded()");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="ast"></param>
		public override void Emit(ILGenerator generator, FilterAst<T, ID> ast)
		{
			// Arg 3 is the the isIncluded arg on Match:
			generator.Emit(OpCodes.Ldarg_3);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class ConstFilterTreeNode<T, ID> : FilterTreeNode<T, ID>
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{

		/// <summary>
		/// 
		/// </summary>
		public virtual long AsInt()
		{
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual string AsString()
		{
			return "";
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual decimal AsDecimal()
		{
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual bool AsBool()
		{
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append(AsString());
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
		/// 
		/// </summary>
		public string Value;

		/// <summary>
		/// 
		/// </summary>
		public override long AsInt()
		{
			return long.Parse(Value);
		}

		/// <summary>
		/// 
		/// </summary>
		public override string AsString()
		{
			return Value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override decimal AsDecimal()
		{
			return decimal.Parse(Value);
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool AsBool()
		{
			return !string.IsNullOrEmpty(Value);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append('"');
			builder.Append(Value.Replace("\"", "\\\""));
			builder.Append('"');
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="ast"></param>
		public override void Emit(ILGenerator generator, FilterAst<T, ID> ast)
		{
			// str
			generator.Emit(OpCodes.Ldstr, Value);
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
		/// 
		/// </summary>
		public long Value;

		/// <summary>
		/// 
		/// </summary>
		public override long AsInt()
		{
			return Value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string AsString()
		{
			return Value.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override decimal AsDecimal()
		{
			return Value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool AsBool()
		{
			return Value!=0;
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
		/// 
		/// </summary>
		public decimal Value;

		/// <summary>
		/// 
		/// </summary>
		public override long AsInt()
		{
			return (long)Value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string AsString()
		{
			return Value.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override decimal AsDecimal()
		{
			return Value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool AsBool()
		{
			return Value != 0;
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
		/// 
		/// </summary>
		public bool Value;

		/// <summary>
		/// 
		/// </summary>
		public override long AsInt()
		{
			return Value ? 1 : 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public override string AsString()
		{
			return Value ? "1" : "0";
		}

		/// <summary>
		/// 
		/// </summary>
		public override decimal AsDecimal()
		{
			return Value ? 1 : 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool AsBool()
		{
			return Value;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append(Value ? "true" : "false");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="ast"></param>
		public override void Emit(ILGenerator generator, FilterAst<T, ID> ast)
		{
			// A 1 or 0 depending on the val:
			generator.Emit(Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
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
		/// 
		/// </summary>
		public override string AsString()
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append("null");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="generator"></param>
		/// <param name="ast"></param>
		public override void Emit(ILGenerator generator, FilterAst<T, ID> ast)
		{
			// null:
			generator.Emit(OpCodes.Ldnull);
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
		/// Assigned arg ID
		/// </summary>
		public int Id;

		/// <summary>
		/// True if the user wants to provide a set
		/// </summary>
		public bool Array;

		/// <summary>
		/// The bound value.
		/// </summary>
		public ArgBinding Binding;

		/// <summary>
		/// 
		/// </summary>
		public override void ToString(StringBuilder builder)
		{
			builder.Append(Array ? "[" + Id + "]" : "{" + Id + "}");
		}
	}
}