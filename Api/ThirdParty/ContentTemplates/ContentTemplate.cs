using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.ContentTemplates
{
	
	/// <summary>
	/// A ContentTemplate
	/// </summary>
	public partial class ContentTemplate : VersionedContent<uint>
	{
		/// <summary>
		/// The name of the content template
		/// </summary>
		public string Name;

		/// <summary>
		/// The description of the content template
		/// </summary>
		public string Description;

		/// <summary>
		/// The content type
		/// web = 1,
		/// email = 2,
		/// Add more types as needed etc... PDF 
		/// </summary>
		public uint TemplateType;

		/// <summary>
		/// Holds all information about how regions are used in this template
		/// </summary>
		public string TemplateJson;

		/// <summary>
		/// The parent template, 0 if non.
		/// </summary>
		public uint ParentTemplate;
		
	}

}