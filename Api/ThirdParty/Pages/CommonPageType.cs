namespace Api.Pages
{
	
	/// <summary>
	/// The types of common page.
	/// </summary>
	public partial class CommonPageType
	{
		/// <summary>
		/// Admin pages of the form /en-admin/{CONTENT_TYPE_NAME}
		/// </summary>
		public static CommonPageType AdminList = new CommonPageType("admin_list");
		
		/// <summary>
		/// Admin pages of the form /en-admin/{CONTENT_TYPE_NAME}/{ID}
		/// </summary>
		public static CommonPageType AdminEdit = new CommonPageType("admin_edit");
		
		/// <summary>
		/// Admin pages of the form /en-admin/{CONTENT_TYPE_NAME}/add
		/// </summary>
		public static CommonPageType AdminAdd = new CommonPageType("admin_add");
		
		/// <summary>
		/// A key for this page type.
		/// </summary>
		public string Key;
		
		/// <summary>
		/// Create a new page type with the given key.
		/// </summary>
		public CommonPageType(string key)
		{
			Key = key;
		}
		
	}
	
}