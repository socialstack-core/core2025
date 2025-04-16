using System;
using Api.Database;
using Api.Translate;
using Api.Users;


namespace Api.Components
{
	
	/// <summary>
	/// A ComponentGroup
	/// </summary>
	public partial class ComponentGroup : VersionedContent<uint>
	{
        /// <summary>
        /// The name of the component group
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;
	}

}