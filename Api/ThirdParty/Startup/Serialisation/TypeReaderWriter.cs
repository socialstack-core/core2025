using Api.Contexts;
using Api.SocketServerLibrary;
using Api.Startup;
using Api.Users;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Api.Startup
{
	/// <summary>
	/// A reader/ writer.
	/// </summary>
	public partial class TypeReaderWriter<T>
	{
		/// <summary>
		/// Writes only the type and id fields of the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="writer"></param>
		public virtual void WriteJsonPartial(T obj, Writer writer)
		{

		}

		/// <summary>
		/// Writes the given object to the given writer in JSON format.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="writer"></param>
		/// <param name="context"></param>
		/// <param name="isIncludes"></param>
		public virtual void WriteJsonUnclosed(T obj, Writer writer, Context context, bool isIncludes)
		{

		}
	}

	/// <summary>
	/// Used for writing/ reading types as documents. 
	/// These write all db storable content in to a JSON object which is not wrapped in "result".
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class TypeDocumentReaderWriter<T>
	{
		/// <summary>
		/// The field set as it was when this was created.
		/// </summary>
		public ContentFields Fields;

		/// <summary>
		/// Writes the given object as stored JSON.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="writer"></param>
		public virtual void WriteStoredJson(T obj, Writer writer)
		{
			
		}

		/// <summary>
		/// Reads the given JObject in to the given target object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="src"></param>
		public virtual void ReadStoredJson(T obj, JObject src)
		{

		}

	}
}