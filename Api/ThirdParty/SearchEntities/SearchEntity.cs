using Api.Users;

namespace Api.SearchEntities
{
	/// <summary>
	/// A SearchEntity
	/// </summary>
	public partial class SearchEntity : UserCreatedContent<uint>
    {
		/// <summary>
		/// The entity name/service of entity to be indexed
		/// </summary>
		public string ContentType;

		/// <summary>
		/// The id of entity to be indexed
		/// </summary>
		public ulong ContentId;

		/// <summary>
		/// The action being performed
		/// </summary>
		public string Action;
    }
}