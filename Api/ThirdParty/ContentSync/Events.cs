using Api.ContentSync;
using Api.Permissions;
using System.Collections.Generic;

namespace Api.Eventing
{

	/// <summary>
	/// Events are instanced automatically. 
	/// You can however specify a custom type or instance them yourself if you'd like to do so.
	/// </summary>
	public partial class Events
	{
		/// <summary>
		/// Set of events for clustered servers.
		/// </summary>
		public static EventGroup<ClusteredServer> ClusteredServer;
		
		/// <summary>
		/// Set of events for nr types.
		/// </summary>
		public static EventGroup<NetworkRoomType> NetworkRoomType;
		
	}

}