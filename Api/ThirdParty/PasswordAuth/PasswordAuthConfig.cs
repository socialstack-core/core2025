using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.PasswordAuth
{
	/// <summary>
	/// The appsettings.json config block for password auth.
	/// </summary>
    public class PasswordAuthConfig
    {
		/// <summary>
		/// Check if password is exposed or not.
		/// </summary>
		public bool CheckIfExposed { get; set; } = true;
		
		/// <summary>
		/// Min length.
		/// </summary>
		public int MinLength { get; set; } = 10;

		/// <summary>
		/// If true, the API will not create a default user if no users are present.
		/// </summary>
		public bool DisableDefaultUser { get; set; } = true;
	}
	
}
