using System;
using System.Text.Json.Serialization;
using Api.Database;
using Api.Permissions;
using Api.Startup;

namespace Api.NavMenus
{
	
	/// <summary>
	/// A particular entry within a navigation menu.
	/// </summary>
	public partial class AdminNavMenuItem : Content<uint>
	{
		/// <summary>
		/// The title of this nav menu entry.
		/// </summary>
		[DatabaseField(Length = 200)]
		public string Title;
		
		/// <summary>
		/// The Page key to target.
		/// </summary>
		public string PageKey;
		
		/// <summary>
		/// The target URL.
		/// </summary>
		public string Url;

		/// <summary>
		/// Optional image to show with this item.
		/// </summary>
		[DatabaseField(Length = 100)]
		public string IconRef;
		
		/// <summary>
		/// The content type on the page.
		/// </summary>
		/// <exception cref="PublicException"></exception>
		[JsonIgnore]
		public Type PageContentType
		{
			get
			{
				var type = PageKey.Split(":")[1];

				if (string.IsNullOrEmpty(type))
				{
					throw new PublicException($"Invalid PageKey found, {PageKey} is invalid",
						"admin-nav-menu-item/bad-page-key");
				}

				var svc = Services.Get(type + "Service");
				return svc.ServicedType;
			}
		}
	}

}