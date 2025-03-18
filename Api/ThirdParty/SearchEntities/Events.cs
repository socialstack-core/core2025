using Api.SearchEntities;

namespace Api.Eventing
{
	/// <summary>
	/// Events are instanced automatically. 
	/// You can however specify a custom type or instance them yourself if you'd like to do so.
	/// </summary>
	public partial class Events
	{
		/// <summary>
		/// Set of events for a searchEntity.
		/// </summary>
		public static EventGroup<SearchEntity> SearchEntity;
    }
}