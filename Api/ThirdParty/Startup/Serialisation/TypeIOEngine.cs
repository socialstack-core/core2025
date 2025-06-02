using Api.Contexts;
using Api.Database;
using Api.Permissions;
using Api.SocketServerLibrary;
using Api.Translate;
using Stripe;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Api.Startup
{
	/// <summary>
	/// Generates a method which will write out all the fields of a type using raw byte copying whenever possible.
	/// As JSON strings are utf8, virtually all the heavy overhead is in conversion to and from UTF8. 
	/// Use ustring whenever feasible to avoid this overhead.
	/// </summary>
	public static class TypeIOEngine
    {
		/// <summary>
		/// Ensures unique names for assemblies generated during this session.
		/// </summary>
		private static int counter = 1;
		
		/// <summary>
		/// Generates a system native write for the given structure.
		/// </summary>
		public static TypeReaderWriter<T> Generate<T, ID>(JsonStructure<T,ID> fieldSet, AutoService service, Context context)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			var set = new List<JsonStructure<T, ID>>
			{
				fieldSet
			};
			return Generate(set, service, context)[0];
		}

		/// <summary>
		/// The bytes for "true"
		/// </summary>
		public static readonly byte[] TrueBytes = new byte[] { 116, 114, 117, 101 };

		/// <summary>
		/// The bytes for ',"id":
		/// </summary>
		public static readonly byte[] IdHeader = new byte[] { (byte)',', (byte)'"', (byte)'i', (byte)'d', (byte)'"', (byte)':' };

		/// <summary>
		/// The bytes for "false"
		/// </summary>
		public static readonly byte[] FalseBytes = new byte[] { 102, 97, 108, 115 , 101 }; 

		/// <summary>
		/// The bytes for "null"
		/// </summary>
		public static readonly byte[] NullBytes = new byte[] { 110, 117, 108, 108 };

		private static ConcurrentDictionary<Type, JsonFieldType> _typeMap;
		
		/// <summary>
		/// Gets JSON field generators.
		/// </summary>
		/// <returns></returns>
		private static ConcurrentDictionary<Type, JsonFieldType> GetTypeMap()
		{
			if (_typeMap != null)
			{
				return _typeMap;
			}
			
			var map = new ConcurrentDictionary<Type, JsonFieldType>();
			_typeMap = map;

			AddTo(map, typeof(bool), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				emitValue();
				WriteBool(code);
			});

			var writeSUint = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(uint) });
			var writeSInt = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(int) });

			var writeSUlong = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(ulong) });
			var writeSLong = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(long) });
			var writeSDateTime = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(DateTime) });
			var writeSDouble = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(double) });
			var writeSFloat = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(float) });
			var writeSDecimal = typeof(Writer).GetMethod("WriteS", new Type[] { typeof(decimal) });
			var writeEscapedUString = typeof(Writer).GetMethod("WriteEscaped", new Type[] { typeof(ustring) });
			var writeJsonString = typeof(Writer).GetMethod("Write", new Type[] { typeof(JsonString) });
			var writeEscapedJsonString = typeof(Writer).GetMethod("WriteEscaped", new Type[] { typeof(JsonString) });
			var writeEscapedString = typeof(Writer).GetMethod("WriteEscaped", new Type[] { typeof(string) });

			AddTo(map, typeof(uint), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSUint);
			});

			AddTo(map, typeof(int), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSInt);
			});

			AddTo(map, typeof(byte), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSUint);
			});

			AddTo(map, typeof(char), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSUint);
			});

			AddTo(map, typeof(sbyte), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSInt);
			});

			AddTo(map, typeof(ushort), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSUint);
			});

			AddTo(map, typeof(short), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSInt);
			});

			AddTo(map, typeof(ulong), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSUlong);
			});

			AddTo(map, typeof(long), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSLong);
			});

			AddTo(map, typeof(DateTime), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSDateTime);
			});

			AddTo(map, typeof(ustring), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeEscapedUString);
			});
			
			AddTo(map, typeof(JsonString), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				if (isDocumentFormat)
				{
					code.Emit(OpCodes.Callvirt, writeJsonString);
				}
				else
				{
					code.Emit(OpCodes.Callvirt, writeEscapedJsonString);
				}
			});

			AddTo(map, typeof(string), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeEscapedString);
			});

			AddTo(map, typeof(double), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSDouble);
			});

			AddTo(map, typeof(float), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSFloat);
			});

			AddTo(map, typeof(decimal), (ILGenerator code, Action emitValue, bool isDocumentFormat) =>
			{
				code.Emit(OpCodes.Ldarg_2); // Writer
				emitValue();
				code.Emit(OpCodes.Callvirt, writeSDecimal);
			});

			// https://github.com/dotnet/runtime/blob/927b1c54956ddb031a2e1a3eddb94ccc16004c27/src/libraries/System.Private.CoreLib/src/System/Number.Formatting.cs#L1333

			return map;
		}

		/// <summary>
		/// Generates a document reader/writer for the given content type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="ID"></typeparam>
		/// <param name="fields"></param>
		/// <returns></returns>
		public static TypeDocumentReaderWriter<T> GenerateDocumentReaderWriter<T, ID>(ContentFields fields)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{

			AssemblyName assemblyName = new AssemblyName("GeneratedDocRW_" + counter);
			counter++;
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

			// Base type to use:
			var baseType = typeof(TypeDocumentReaderWriter<T>);

			// Start building the type:
			var typeName = typeof(T).Name;
			TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName + "_DocRW", TypeAttributes.Public, baseType);

			var writerMethod = typeBuilder.DefineMethod("WriteStoredJson", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(void), new Type[] {
				typeof(T),
				typeof(Writer)
			});

			ILGenerator writeBody = writerMethod.GetILGenerator();

			var first = true;

			EmitWriteChar(writeBody, '{');

			foreach (var field in fields.List)
			{
				if (field.FieldInfo == null)
				// || field.VirtualInfo != null && field.VirtualInfo.IsList { future feature, would need to grab these sets too }
				{
					continue;
				}

				if (field.FieldInfo.FieldType.IsArray)
				{
					Log.Warn("typeio", "Temporarily ignored an array field from probably revisions (WIP)");
					continue;
				}

				if (first)
				{
					first = false;
				}
				else
				{
					EmitWriteChar(writeBody, ',');
				}

				EmitWriteASCII(writeBody, "\"" + field.Name + "\"");
				EmitWriteChar(writeBody, ':');

				var type = field.FieldInfo.FieldType;
				var nextField = writeBody.DefineLabel();

				if (!type.IsValueType)
				{
					// if(x.field == null) { writer.WriteASCII("null"); }
					writeBody.Emit(OpCodes.Ldarg_1); // T obj
					writeBody.Emit(OpCodes.Ldfld, field.FieldInfo); // .theField
					writeBody.Emit(OpCodes.Ldnull);
					writeBody.Emit(OpCodes.Ceq);
					var notNull = writeBody.DefineLabel();
					writeBody.Emit(OpCodes.Brfalse, notNull);
						EmitWriteASCII(writeBody, "null");
						writeBody.Emit(OpCodes.Br, nextField);
					writeBody.MarkLabel(notNull);
				}

				// Here it is either a value type (which can still be a Nullable) or a not null object.

				if (type.IsGenericType)
				{
					var gDef = type.GetGenericTypeDefinition();

					if (gDef == typeof(Localized<>))
					{
						// Call ToJson.
						var toJson = type.GetMethod("ToJson", BindingFlags.Instance | BindingFlags.Public, new Type[] { typeof(Writer) });

						if (toJson == null)
						{
							throw new Exception("ToJson(writer) on a localised type missing somehow!");
						}

						writeBody.Emit(OpCodes.Ldarg_1); // T obj
						writeBody.Emit(OpCodes.Ldflda, field.FieldInfo); // .theLocalizedField (this)
						writeBody.Emit(OpCodes.Ldarg_2); // writer
						writeBody.Emit(OpCodes.Callvirt, toJson); // invoke toJson

					}
					else if (gDef == typeof(Nullable<>))
					{
						EmitWriteBasicField(writeBody, field, (ILGenerator gen) =>
						{
							gen.Emit(OpCodes.Ldarg_1); // T obj
						}, true);
					}
					else
					{
						// probably a list - fail!
						throw new NotSupportedException("Can't currently store this generic type in db documents. " +
							"This can happen if there is e.g. a List in a versioned piece of content.");
					}
				}
				else
				{
					EmitWriteBasicField(writeBody, field, (ILGenerator gen) => {
						gen.Emit(OpCodes.Ldarg_1); // T obj
					}, true);
				}

				writeBody.MarkLabel(nextField);
			}

			EmitWriteChar(writeBody, '}');

			writeBody.Emit(OpCodes.Ret);

			// Finish the type and instance it.
			Type builtType = typeBuilder.CreateType();
			var instance = Activator.CreateInstance(builtType) as TypeDocumentReaderWriter<T>;
			return instance;
		}

		/// <summary>
		/// Emits writer.WriteASCII(val);
		/// </summary>
		/// <param name="gen"></param>
		/// <param name="val"></param>
		public static void EmitWriteASCII(ILGenerator gen, string val)
		{
			gen.Emit(OpCodes.Ldarg_2); // writer.
			var writeAscii = typeof(Writer).GetMethod("WriteASCII", BindingFlags.Public | BindingFlags.Instance);
			gen.Emit(OpCodes.Ldstr, val);
			gen.Emit(OpCodes.Callvirt, writeAscii);
		}
		
		/// <summary>
		/// Emits writer.Write(theChar);
		/// </summary>
		/// <param name="gen"></param>
		/// <param name="val"></param>
		public static void EmitWriteChar(ILGenerator gen, char val)
		{
			gen.Emit(OpCodes.Ldarg_2); // writer.
			var writeByte = typeof(Writer).GetMethod("Write", BindingFlags.Public | BindingFlags.Instance, new Type[]{
				typeof(byte)
			});

			gen.Emit(OpCodes.Ldc_I4, (int)((byte)val));
			gen.Emit(OpCodes.Callvirt, writeByte);
		}

		/// <summary>
		/// Emits JSON write of a given content field in to the given body. 
		/// It must be a basic type or a nullable of one of those types.
		/// </summary>
		/// <param name="body"></param>
		/// <param name="field"></param>
		/// <param name="objLoader"></param>
		/// <param name="isDocumentFormat"></param>
		/// <exception cref="Exception"></exception>
		public static void EmitWriteBasicField(ILGenerator body, ContentField field, Action<ILGenerator> objLoader, bool isDocumentFormat = false)
		{
			// Check if it's a nullable:
			var fieldType = field.FieldType;

			var nullableType = Nullable.GetUnderlyingType(fieldType);

			if (nullableType != null)
			{
				fieldType = nullableType;
			}

			var typeMap = GetTypeMap();

			if (!typeMap.TryGetValue(fieldType, out JsonFieldType jft))
			{
				// Can't serialise this here.
				throw new Exception("Unable to serialise fields of this type (" + fieldType.Name + ").");
			}

			jft.EmitWrite(body, field, nullableType, objLoader, isDocumentFormat);
		}

		/// <summary>
		/// Generates ID collectors for the given fields.
		/// </summary>
		/// <param name="idFields"></param>
		/// <returns></returns>
		public static void GenerateIDCollectors(ContentField[] idFields)
		{
			var atLeastOne = false;

			for (var i = 0; i < idFields.Length; i++)
			{
				if (idFields[i].IDCollectorType == null)
				{
					atLeastOne = true;
					break;
				}
			}

			if (!atLeastOne)
			{
				return;
			}

			AssemblyName assemblyName = new AssemblyName("GeneratedTypesID_" + counter);
			counter++;
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

			for (var i = 0; i < idFields.Length; i++)
			{
				var idField = idFields[i];

				if (idField.IDCollectorType != null)
				{
					continue;
				}

				Type baseType;
				MethodInfo addMethod;
				TypeBuilder typeBuilder;
				MethodBuilder collect;
				ILGenerator body;

				if (idField.VirtualInfo != null && idField.VirtualInfo.DynamicTypeField != null)
				{
					// Virtual field which requires collecting both type and ID.
					// In this scenario, DynType is a string and ID source is a ulong always.
					baseType = typeof(MultiIdCollector);
					addMethod = baseType.GetMethod("Add");


					// Build the type:
					typeBuilder = moduleBuilder.DefineType("FieldWriter_" + idField.Name + "_" + counter, TypeAttributes.Public, baseType);
					counter++;

					collect = typeBuilder
						.DefineMethod("Collect",
							MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
							typeof(void),
							new Type[] {
								typeof(object),
								typeof(Context)
							}
						);

					// "This" is the ID collector, with it's Add method.
					// arg1 is the object to collect from.
					body = collect.GetILGenerator();

					body.Emit(OpCodes.Ldarg_0);

					// Put the type field (or property) value on the stack:
					var typeSrc = idField.VirtualInfo.DynamicTypeField;

					if (typeSrc.FieldInfo != null)
					{
						body.Emit(OpCodes.Ldarg_1);
						body.Emit(OpCodes.Ldfld, typeSrc.FieldInfo);
					}
					else
					{
						// Property
						body.Emit(OpCodes.Ldarg_1);
						body.Emit(OpCodes.Callvirt, typeSrc.PropertyInfo.GetGetMethod());
					}

					// Next, put the ID field (or property) on the stack:
					var idSrc = idField.VirtualInfo.IdSource;

					if (idSrc.FieldInfo != null)
					{
						body.Emit(OpCodes.Ldarg_1);
						var idFldType = idSrc.FieldInfo.FieldType;

						// If it's a localized ID, resolve to the actual value.
						if (idFldType.IsGenericType && idFldType.GetGenericTypeDefinition() == typeof(Localized<>))
						{
							body.Emit(OpCodes.Ldflda, idSrc.FieldInfo);
							body.Emit(OpCodes.Ldarg_2); // context
							body.Emit(OpCodes.Ldc_I4_1); // fallback (true)
							var localizedGetMethod = idFldType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance, new Type[]{ typeof(Context), typeof(bool) });
							body.Emit(OpCodes.Callvirt, localizedGetMethod);
						}
						else
						{
							body.Emit(OpCodes.Ldfld, idSrc.FieldInfo);
						}
					}
					else
					{
						// Property
						body.Emit(OpCodes.Ldarg_1);
						body.Emit(OpCodes.Callvirt, idSrc.PropertyInfo.GetGetMethod());
						var idFldType = idSrc.PropertyInfo.PropertyType;

						if (idFldType.IsGenericType && idFldType.GetGenericTypeDefinition() == typeof(Localized<>))
						{
							// Need an address - store as a local:
							var localizedLocal = body.DeclareLocal(idFldType);
							body.Emit(OpCodes.Stloc, localizedLocal);
							body.Emit(OpCodes.Ldloca, localizedLocal);
							body.Emit(OpCodes.Ldarg_2); // context
							body.Emit(OpCodes.Ldc_I4_1); // fallback (true)
							var localizedGetMethod = idFldType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(Context), typeof(bool) });
							body.Emit(OpCodes.Callvirt, localizedGetMethod);
						}
					}

					// Add to the collector:
					body.Emit(OpCodes.Callvirt, addMethod);
					body.Emit(OpCodes.Ret);

					// Lock the type:
					idField.SetIDCollectorType(typeBuilder.CreateType());
					continue;
				}

				var idFieldType = idField.FieldType;

				// If it's localized, unwrap it. Must always happen before nullable check.
				if (idFieldType.IsGenericType && idFieldType.GetGenericTypeDefinition() == typeof(Localized<>))
				{
					idFieldType = idFieldType.GetGenericArguments()[0];
				}

				// If the field type is nullable, use the base type here.
				var nullableBaseType = Nullable.GetUnderlyingType(idFieldType);

				baseType = typeof(IDCollector<>).MakeGenericType(new Type[] {
					nullableBaseType == null ? idFieldType : nullableBaseType
				});

				// Get the Add method:
				addMethod = baseType.GetMethod("Add");

				// Build the type:
				typeBuilder = moduleBuilder.DefineType("FieldWriter_" + idField.Name + "_" + counter, TypeAttributes.Public, baseType);
				counter++;

				collect = typeBuilder.DefineMethod(
					"Collect", 
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, 
					typeof(void), new Type[] {
						typeof(object),
						typeof(Context)
					}
				);

				// "This" is the ID collector, with it's Add method.
				// arg1 is the object to collect from.
				body = collect.GetILGenerator();

				body.Emit(OpCodes.Ldarg_0);

				// Put the field (or property) value on the stack:
				if (idField.FieldInfo != null)
				{
					body.Emit(OpCodes.Ldarg_1);

					var idFldType = idField.FieldType;
					if (idFldType.IsGenericType && idFldType.GetGenericTypeDefinition() == typeof(Localized<>))
					{
						body.Emit(OpCodes.Ldflda, idField.FieldInfo);
						body.Emit(OpCodes.Ldarg_2); // context
						body.Emit(OpCodes.Ldc_I4_1); // fallback (true)
						var localizedGetMethod = idFldType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(Context), typeof(bool) });
						body.Emit(OpCodes.Callvirt, localizedGetMethod);
					}
					else
					{
						body.Emit(OpCodes.Ldfld, idField.FieldInfo);
					}
				}
				else
				{
					// Property
					body.Emit(OpCodes.Ldarg_1);
					body.Emit(OpCodes.Callvirt, idField.PropertyInfo.GetGetMethod());

					var idFldType = idField.PropertyInfo.PropertyType;
					if (idFldType.IsGenericType && idFldType.GetGenericTypeDefinition() == typeof(Localized<>))
					{
						// Need an address - store as a local:
						var localizedLocal = body.DeclareLocal(idFldType);
						body.Emit(OpCodes.Stloc, localizedLocal);
						body.Emit(OpCodes.Ldloca, localizedLocal);
						body.Emit(OpCodes.Ldarg_2); // context
						body.Emit(OpCodes.Ldc_I4_1); // fallback (true)
						var localizedGetMethod = idFldType.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(Context), typeof(bool) });
						body.Emit(OpCodes.Callvirt, localizedGetMethod);
					}
				}

				if (nullableBaseType != null)
				{
					// If nullable, and it's null, do nothing. To check though, we need an address. We'll need to store it in a local for that:
					var loc = body.DeclareLocal(idFieldType);
					var after = body.DefineLabel();
					body.Emit(OpCodes.Stloc, loc);
					body.Emit(OpCodes.Ldloca, 0);
					body.Emit(OpCodes.Dup);
					body.Emit(OpCodes.Callvirt, idFieldType.GetProperty("HasValue").GetGetMethod());
					// The t/f is now on the stack. Check if it's null, and if so, ret.
					body.Emit(OpCodes.Ldc_I4_0);
					body.Emit(OpCodes.Ceq);
					body.Emit(OpCodes.Brfalse, after);
					body.Emit(OpCodes.Pop); // Remove the Ldarg_0 and the val (which we duped above to read HasValue) from the stack.
					body.Emit(OpCodes.Pop);
					body.Emit(OpCodes.Ret);
					body.MarkLabel(after);
					body.Emit(OpCodes.Callvirt, idFieldType.GetProperty("Value").GetGetMethod());
				}

				// Add:
				body.Emit(OpCodes.Callvirt, addMethod);
				body.Emit(OpCodes.Ret);

				// Lock the type:
				idField.SetIDCollectorType(typeBuilder.CreateType());
			}

		}

		private static JsonFieldType AddTo(ConcurrentDictionary<Type, JsonFieldType> map, Type type, Action<ILGenerator, Action, bool> onWriteValue)
		{
			var fieldType = new JsonFieldType(type, onWriteValue);
			map[type] = fieldType;
			return fieldType;
		}
		
		/// <summary>
		/// Used for debugging bolt generated IO.
		/// </summary>
		public static void DebugField(string s)
		{
			Log.Info("bolt", s);
		}
		
		/// <summary>
		/// Generates a system native write for the given structures. This list will usually be the list of all roles for a type on the first run.
		/// </summary>
		public static List<TypeReaderWriter<T>> Generate<T, ID>(List<JsonStructure<T, ID>> fieldSets, AutoService service, Context context)
			where T : Content<ID>, new()
			where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
		{
			var result = new List<TypeReaderWriter<T>>();
			
			AssemblyName assemblyName = new AssemblyName("GeneratedTypesRW_" + counter);
			counter++;
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

			foreach (var set in fieldSets)
			{
				if (set == null)
				{
					continue;
				}

				// Base type to use:
				var baseType = typeof(TypeReaderWriter<T>);

				// Start building the type:
				var typeName = typeof(T).Name;
				TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName + "_RW", TypeAttributes.Public, baseType);

				
				// Just an empty constructor. The actual pre-defined byte[]'s will be set direct to the fields shortly.
				ConstructorBuilder ctor0 = typeBuilder.DefineConstructor(
					MethodAttributes.Public,
					CallingConventions.Standard,
					[ typeof(AutoService<T,ID>) ]
				);
				ILGenerator constructorBody = ctor0.GetILGenerator();

				// all byte[] fields to initialise:
				var fieldsToInit = new List<PreGeneratedByteField>();

				// The JSON "header" which will be of the form {"type":"typename"
				var header = AddField(typeBuilder, fieldsToInit, "{\"type\":\"" + typeName + "\"");

				var writeJsonPartialMethod = typeBuilder.DefineMethod("WriteJsonPartial", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(void), new Type[] {
					typeof(T),
					typeof(Writer)
				});

				ILGenerator writerPartialBody = writeJsonPartialMethod.GetILGenerator();

				// Write the header:
				header.Write(writerPartialBody);

				if (IdHeaderRef == null)
				{
					IdHeaderRef = GetFromCommonField(nameof(IdHeader), IdHeader);
				}

				var typeMap = GetTypeMap();

				// The ID:
				var idField = set.GetField("id", JsonFieldGroup.Any);

				if (typeMap.TryGetValue(idField.FieldInfo.FieldType, out JsonFieldType idJft))
				{
					// ,"id":
					IdHeaderRef.Write(writerPartialBody);

					idJft.EmitWrite(writerPartialBody, idField, null);
				}

				// }
				WriteChar(writerPartialBody, '}');

				writerPartialBody.Emit(OpCodes.Ret);

				var writeJsonMethod = typeBuilder.DefineMethod("WriteJsonUnclosed", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(void), [
					typeof(T),
					typeof(Writer),
					typeof(Context),
					typeof(bool)
				]);
				ILGenerator writerBody = writeJsonMethod.GetILGenerator();

				// Write the header:
				header.Write(writerBody);

				var fieldService = Services.Get<ContentFieldAccessRuleService>();

				var fields = Task.Run(() => 
					fieldService.Where("RoleId = ? and EntityName = ?")
								.Bind(context.RoleId)
								.Bind(typeof(T).Name)
								.ListAll(new(1, 1, 1))
				).GetAwaiter().GetResult().Result;
				
				if (set.Fields != null){

					// Add each field
					foreach(var field in set.Fields.Values)
					{
						if(field == null || field.Name == "Type")
						{
							continue;
						}

						var ignore = false;

						if (field.Attributes != null)
						{
							foreach (var attrib in field.Attributes)
							{
								if ((attrib as Newtonsoft.Json.JsonIgnoreAttribute) != null || (attrib as System.Text.Json.Serialization.JsonIgnoreAttribute) != null)
								{
									// This field is not for us.
									ignore = true;
									break;
								}
							}
						}

						if (ignore)
						{
							continue;
						}

						// Skip all virtual fields.
						if (field.PropertyGet == null && field.FieldInfo == null)
						{
							continue;
						}

						// The type we may be outputting a value outputter for:
						var fieldType = field.TargetType;
						var isLocalised = false;

						if (fieldType.IsGenericType)
						{
							var typeDef = fieldType.GetGenericTypeDefinition();

							if (typeDef == typeof(Localized<>))
							{
								isLocalised = true;
								fieldType = fieldType.GetGenericArguments()[0];
							}
						}

						// Check if it's a nullable:
						Type nullableType = Nullable.GetUnderlyingType(fieldType);

						if (nullableType != null)
						{
							fieldType = nullableType;
						}

						if (!typeMap.TryGetValue(fieldType, out JsonFieldType jft))
						{
							// Can't serialise this here.
							Log.Warn("typeio", "Skipped serialising a field because it is of an unrecognised type: " + field.Name + " on " + typeof(T));
							continue;
						}

						var fieldName = field.Name;
						var lowercaseFirst = char.ToLower(fieldName[0]) + fieldName.Substring(1);

						var matchingField = fields.Find(rule => rule.FieldName == lowercaseFirst);

						bool? constTrueOrFalse = null;
						string filterString = null;
						
						if(matchingField is null || string.IsNullOrEmpty(matchingField.CanRead)){
							constTrueOrFalse = field.Readable;
						}else if( matchingField.CanRead == "true"){
							constTrueOrFalse = true;
						}else if( matchingField.CanRead == "false"){
							constTrueOrFalse = false;
						}else{
							// will be a full filter obj
							filterString = matchingField.CanRead;
						}

						if (constTrueOrFalse.HasValue && constTrueOrFalse.Value == false)
						{
							// This field is not for us.
							continue;
						}

						// The field name:
						var property = AddField(typeBuilder, fieldsToInit, ",\"" + lowercaseFirst + "\":");

						// Define a label for the end of the conditional block
						var endOfIfLabel = writerBody.DefineLabel();

						// this/instance
						
						if (!string.IsNullOrEmpty(filterString))
						{

							var fieldBuilder = typeBuilder.DefineField("filter_" + lowercaseFirst, typeof(FilterBase), FieldAttributes.Private);

							
							constructorBody.Emit(OpCodes.Ldarg_0);
							constructorBody.Emit(OpCodes.Ldarg_1);
							constructorBody.Emit(OpCodes.Ldstr, filterString); 
							constructorBody.Emit(OpCodes.Ldc_I4_1); 
							constructorBody.Emit(OpCodes.Callvirt,
								typeof(AutoService).GetMethod(
									nameof(AutoService.GetGeneralFilterFor), 
									BindingFlags.Instance | BindingFlags.Public,
									[
										typeof (string), typeof(bool)
									]
								)
							);

							
							constructorBody.Emit(OpCodes.Stfld, fieldBuilder);
							
							

							writerBody.Emit(OpCodes.Ldarg_0); // this
							writerBody.Emit(OpCodes.Ldfld, fieldBuilder); // .filter_whatever
							writerBody.Emit(OpCodes.Ldarg_3); // context
							writerBody.Emit(OpCodes.Ldarg_1); // the object
							writerBody.Emit(OpCodes.Ldarg, 4); // isIncluded
							writerBody.Emit(OpCodes.Callvirt, typeof(FilterBase).GetMethod(nameof(FilterBase.Match), BindingFlags.Instance | BindingFlags.Public, [typeof(Context), typeof(object), typeof(bool) ]));
							
							// Branch if false (skip the block if the condition is false)
							writerBody.Emit(OpCodes.Brfalse, endOfIfLabel);
						}	
						// Code to execute if the condition is true
						// ,"fieldName":
						property.Write(writerBody);
						// Value->str:
						if (isLocalised)
						{
							jft.EmitLocalisedWrite(writerBody, field, nullableType);
						}
						else
						{
							jft.EmitWrite(writerBody, field, nullableType);
						}
						// Mark the end of the conditional block
						writerBody.MarkLabel(endOfIfLabel);


					}
				}

				writerBody.Emit(OpCodes.Ret);

				constructorBody.Emit(OpCodes.Ret);
				
				// Finish the type.
				Type builtType = typeBuilder.CreateType();
				var instance = Activator.CreateInstance(builtType, [service]) as TypeReaderWriter<T>;

				foreach (var field in fieldsToInit)
				{
					var fld = instance.GetType().GetField(field.Field.Name, BindingFlags.NonPublic | BindingFlags.Instance);

					fld.SetValue(instance, field.Value);
				}

				result.Add(instance);
			}
			
			return result;
		}

		private static PreGeneratedByteField True;
		private static PreGeneratedByteField False;
		private static PreGeneratedByteField Null;
		private static PreGeneratedByteField IdHeaderRef;

		/// <summary>
		/// Writes either "true" or "false" based on the current value on the top of the stack.
		/// </summary>
		/// <param name="writerBody"></param>
		public static void WriteNull(ILGenerator writerBody)
		{
			if (Null == null)
			{
				Null = GetFromCommonField(nameof(NullBytes), NullBytes);
			}

			Null.Write(writerBody);
		}

		/// <summary>
		/// Writes either "true" or "false" based on the current value on the top of the stack.
		/// </summary>
		/// <param name="writerBody"></param>
		public static void WriteBool(ILGenerator writerBody)
		{
			if (True == null)
			{
				True = GetFromCommonField(nameof(TrueBytes), TrueBytes);
				False = GetFromCommonField(nameof(FalseBytes), FalseBytes);
			}

			var startOfFalse = writerBody.DefineLabel();
			var endOfIfElse = writerBody.DefineLabel();
			writerBody.Emit(OpCodes.Brfalse, startOfFalse);
			True.Write(writerBody);
			writerBody.Emit(OpCodes.Br, endOfIfElse);
			writerBody.MarkLabel(startOfFalse);
			False.Write(writerBody);
			writerBody.MarkLabel(endOfIfElse);
		}

		/// <summary>
		/// Creates a pre-gen byte field from one of the common fields.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		private static PreGeneratedByteField GetFromCommonField(string name, byte[] val)
		{
			var commonField = typeof(TypeIOEngine).GetField(name, BindingFlags.Public | BindingFlags.Static);
			return new PreGeneratedByteField()
			{
				Field = commonField,
				Value = val
			};
		}

		/// <summary>
		/// Emits IL which will write exactly 1 character (the given one) to the stream.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="character"></param>
		private static void WriteChar(ILGenerator writer, char character)
		{
			if (_writeByte == null)
			{
				_writeByte = typeof(Writer).GetMethod("Write", new Type[] {
					typeof(byte)
				});
			}

			var byteVal = (int)character;
			writer.Emit(OpCodes.Ldarg_2);
			writer.Emit(OpCodes.Ldc_I4, byteVal);
			writer.Emit(OpCodes.Callvirt, _writeByte);
		}

		/// <summary>
		/// Writer.Write(byte)
		/// </summary>
		private static MethodInfo _writeByte;

		/// <summary>
		/// Adds a field with the given textual content. It will be added as a UTF8 byte[].
		/// </summary>
		/// <param name="typeBuilder"></param>
		/// <param name="fields"></param>
		/// <param name="content"></param>
		/// <param name="includeLength"></param>
		/// <returns></returns>
		public static PreGeneratedByteField AddField(TypeBuilder typeBuilder, List<PreGeneratedByteField> fields, string content, bool includeLength = false)
		{
			var textBytes = System.Text.Encoding.UTF8.GetBytes(content);

			if (includeLength)
			{
				// Text which is 250 bytes or shorter:
				var len = textBytes.Length;
				var bytes = new byte[1 + textBytes.Length];
				Array.Copy(textBytes, 0, bytes, 1, textBytes.Length);
				bytes[0] = (byte)len;
			}

			return AddField(typeBuilder, fields, textBytes);
		}

		/// <summary>
		/// Adds a field with the given byte[] content.
		/// </summary>
		/// <param name="typeBuilder"></param>
		/// <param name="fields"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public static PreGeneratedByteField AddField(TypeBuilder typeBuilder, List<PreGeneratedByteField> fields, byte[] content)
		{
			var field = typeBuilder.DefineField("data_" + fields.Count, typeof(byte[]), FieldAttributes.InitOnly | FieldAttributes.Private);

			var fieldToInit = new PreGeneratedByteField()
			{
				Field = field,
				Value = content
			};

			fields.Add(fieldToInit);
			return fieldToInit;
		}
	}

	/// <summary>
	/// A particular supported JSON field type.
	/// </summary>
	public class JsonFieldType
	{
		/// <summary>
		/// System type.
		/// </summary>
		public Type Type;

		/// <summary>
		/// Serialises the value which is already on the stack.
		/// </summary>
		public Action<ILGenerator, Action, bool> OnSerialise;

		/// <summary>
		/// Defines info about an available field type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="onSerialise"></param>
		public JsonFieldType(Type type, Action<ILGenerator, Action, bool> onSerialise)
		{
			Type = type;
			OnSerialise = onSerialise;
		}

		/// <summary>
		/// Emits a write of a Localised field.
		/// </summary>
		/// <param name="body"></param>
		/// <param name="field"></param>
		/// <param name="nullableType"></param>
		/// <param name="isDocumentFormat"></param>
		public void EmitLocalisedWrite(ILGenerator body, JsonField field, Type nullableType, bool isDocumentFormat = false)
		{
			Label endOfStatementLabel = body.DefineLabel();

			// e.g. Localised<uint?> -> valueType is uint?
			var localisedType = field.TargetType;
			var valueType = localisedType.GetGenericArguments()[0];

			// Always put the result value in to a local variable.
			var loc = body.DeclareLocal(valueType);

			if (field.PropertyGet != null)
			{
				var propertyLocal = body.DeclareLocal(localisedType);
				body.Emit(OpCodes.Ldarg_1);
				body.Emit(OpCodes.Callvirt, field.PropertyGet);
				body.Emit(OpCodes.Stloc, propertyLocal);
				body.Emit(OpCodes.Ldloca, propertyLocal);
			}
			else
			{
				// Field:
				body.Emit(OpCodes.Ldarg_1);
				body.Emit(OpCodes.Ldflda, field.FieldInfo);
			}

			// An address of the Localized struct is currently on the stack.

			// Push the context and a constant 'true'
			body.Emit(OpCodes.Ldarg_3); // context
			body.Emit(OpCodes.Ldc_I4_1); // fallback = true

			// Can now call Get:
			var localisedGetMethod = localisedType.GetMethod(
				"Get", BindingFlags.Public | BindingFlags.Instance,
				new Type[] {
					typeof(Context),
					typeof(bool)
				});

			body.Emit(OpCodes.Callvirt, localisedGetMethod);

			// The value, which can still be a nullable, is now in the local:
			body.Emit(OpCodes.Stloc, loc);

			if (nullableType != null)
			{
				body.Emit(OpCodes.Ldloca, loc);

				// Check if it is null (and if so, output "null" and skip the following value)
				var hasValueMethod = valueType.GetProperty("HasValue").GetGetMethod();

				body.Emit(OpCodes.Callvirt, hasValueMethod);

				Label notNullLabel = body.DefineLabel();

				body.Emit(OpCodes.Brtrue, notNullLabel);
				TypeIOEngine.WriteNull(body);
				body.Emit(OpCodes.Br, endOfStatementLabel);
				body.MarkLabel(notNullLabel);
			}

			// Serialise the value:
			OnSerialise(body, () => {

				if (nullableType == null)
				{
					// Emit a read of the local:
					body.Emit(OpCodes.Ldloc, loc);
				}
				else
				{
					// This value is nullable, but it's specifically not null.
					// Put the actual value onto the stack here.
					body.Emit(OpCodes.Ldloca, loc);
					var getValueMethod = field.TargetType.GetProperty("Value").GetGetMethod();
					body.Emit(OpCodes.Callvirt, getValueMethod);
				}
			}, isDocumentFormat);

			body.MarkLabel(endOfStatementLabel);
		}

		/// <summary>
		/// Emits the necessary command to serialise the field.
		/// </summary>
		public void EmitWrite(ILGenerator body, JsonField field, Type nullableType, bool isDocumentFormat = false)
		{
			Label? endOfStatementLabel = nullableType == null ? null : body.DefineLabel();

			// Check if it is null - output an address to the struct so we can call HasValue.
			LocalBuilder propertyLocal = null;

			if (nullableType != null)
			{
				// the object we're getting the field/ property from:
				body.Emit(OpCodes.Ldarg_1);

				if (field.PropertyGet != null)
				{
					// Property. Store in a local such that the address can be used.
					propertyLocal = body.DeclareLocal(field.TargetType);
					body.Emit(OpCodes.Callvirt, field.PropertyGet);
					body.Emit(OpCodes.Stloc, propertyLocal);
					body.Emit(OpCodes.Ldloca, propertyLocal);
				}
				else
				{
					// Field:
					body.Emit(OpCodes.Ldflda, field.FieldInfo);
				}

				// Check if it is null (and if so, output "null" and skip the following value)
				var hasValueMethod = field.TargetType.GetProperty("HasValue").GetGetMethod();

				body.Emit(OpCodes.Callvirt, hasValueMethod);

				Label notNullLabel = body.DefineLabel();

				body.Emit(OpCodes.Brtrue, notNullLabel);
				TypeIOEngine.WriteNull(body);
				body.Emit(OpCodes.Br, endOfStatementLabel.Value);
				body.MarkLabel(notNullLabel);
			}

			// Serialise the value:
			OnSerialise(body, () => {

				if (nullableType == null)
				{

					// Emit a read of the field value:
					body.Emit(OpCodes.Ldarg_1);
					
					if (field.PropertyGet != null)
					{
						// Property:
						body.Emit(OpCodes.Callvirt, field.PropertyGet);
					}
					else
					{
						// Field:
						body.Emit(OpCodes.Ldfld, field.FieldInfo);
					}
				}
				else
				{
					// This value is nullable.

					// Not null output. Put the actual value onto the stack here.
					if (field.PropertyGet != null)
					{
						body.Emit(OpCodes.Ldloca, propertyLocal);
					}
					else
					{
						// the object we're getting the field/ property from:
						body.Emit(OpCodes.Ldarg_1);
						body.Emit(OpCodes.Ldflda, field.FieldInfo);
					}

					var getValueMethod = field.TargetType.GetProperty("Value").GetGetMethod();
					body.Emit(OpCodes.Callvirt, getValueMethod);
				}
			}, isDocumentFormat);

			if (endOfStatementLabel.HasValue)
			{
				body.MarkLabel(endOfStatementLabel.Value);
			}
		}

		/// <summary>
		/// Emits the necessary command to serialise the field.
		/// </summary>
		public void EmitWrite(ILGenerator body, ContentField field, Type nullableType, Action<ILGenerator> objLoader, bool isDocumentFormat = false)
		{
			Label? endOfStatementLabel = nullableType == null ? null : body.DefineLabel();

			// Check if it is null - output an address to the struct so we can call HasValue.
			LocalBuilder propertyLocal = null;

			if (nullableType != null)
			{
				// the object we're getting the field/ property from:
				objLoader(body);

				if (field.PropertyInfo != null)
				{
					// Property. Store in a local such that the address can be used.
					propertyLocal = body.DeclareLocal(field.FieldType);
					body.Emit(OpCodes.Callvirt, field.PropertyInfo.GetGetMethod());
					body.Emit(OpCodes.Stloc, propertyLocal);
					body.Emit(OpCodes.Ldloca, propertyLocal);
				}
				else
				{
					// Field:
					body.Emit(OpCodes.Ldflda, field.FieldInfo);
				}

				// Check if it is null (and if so, output "null" and skip the following value)
				var hasValueMethod = field.FieldType.GetProperty("HasValue").GetGetMethod();

				body.Emit(OpCodes.Callvirt, hasValueMethod);

				Label notNullLabel = body.DefineLabel();

				body.Emit(OpCodes.Brtrue, notNullLabel);
				TypeIOEngine.WriteNull(body);
				body.Emit(OpCodes.Br, endOfStatementLabel.Value);
				body.MarkLabel(notNullLabel);
			}

			// Serialise the value:
			OnSerialise(body, () => {

				if (nullableType == null)
				{

					// Emit a read of the field value:
					objLoader(body);

					if (field.PropertyInfo != null)
					{
						// Property:
						body.Emit(OpCodes.Callvirt, field.PropertyInfo.GetGetMethod());
					}
					else
					{
						// Field:
						body.Emit(OpCodes.Ldfld, field.FieldInfo);
					}
				}
				else
				{
					// This value is nullable.

					// Not null output. Put the actual value onto the stack here.
					if (field.PropertyInfo != null)
					{
						body.Emit(OpCodes.Ldloca, propertyLocal);
					}
					else
					{
						// the object we're getting the field/ property from:
						objLoader(body);
						body.Emit(OpCodes.Ldflda, field.FieldInfo);
					}

					var getValueMethod = field.FieldType.GetProperty("Value").GetGetMethod();
					body.Emit(OpCodes.Callvirt, getValueMethod);
				}
			}, isDocumentFormat);

			if (endOfStatementLabel.HasValue)
			{
				body.MarkLabel(endOfStatementLabel.Value);
			}
		}
	}
	
	/// <summary>
	/// A constructed custom content type.
	/// </summary>
	public class ConstructedCustomContentType
	{
		/// <summary>
		/// The ID of the CustomContentType.
		/// </summary>
		public uint Id;
		/// <summary>
		/// The underlying custom type.
		/// </summary>
		public Type ContentType;
		/// <summary>
		/// The controller type for this custom type.
		/// </summary>
		public Type ControllerType;
		/// <summary>
		/// The autoservice for this custom type.
		/// </summary>
		public AutoService Service;
	}

}