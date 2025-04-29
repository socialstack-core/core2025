using Api.AutoForms;
using Api.Database;
using Api.Permissions;
using Api.Startup;
using Api.Translate;
using Api.Users;


namespace Api.Components
{
	
	/// <summary>
	/// A ComponentGroup
	/// </summary>
	[HasVirtualField("Role", typeof(Role), "Id")]
	public partial class ComponentGroup : VersionedContent<uint>
	{
        /// <summary>
        /// The name of the component group
        /// </summary>
        [DatabaseField(Length = 200)]
		[Localized]
		public string Name;

		/// <summary>
		/// a flat string[] json array of components a role is allowed to access
		/// </summary>
		[Module("Admin/ComponentGroup")]
		public string AllowedComponents;

		/// <summary>
		/// The role this component group belongs to.
		/// </summary>
		public uint Role;
	}

}