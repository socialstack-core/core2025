
using Api.ContentSync;
using Api.Contexts;
using Api.Permissions;
using Api.SocketServerLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Api.Startup
{
	public partial class StdOutController : ControllerBase
	{
		
		
		/// <summary>
		/// Forces a GC run. Convenience for testing for memory leaks.
		/// </summary>
		[HttpGet("whoami")]
		public ServerIdentification WhoAmI()
		{
			// Get server ID from csync service:
			var id = Services.Get<ContentSyncService>().ServerId;
			return new ServerIdentification() {
				Id = id
			};
		}

	}

	/// <summary>
	/// Server identifier from the whoami endpoint.
	/// </summary>
	public struct ServerIdentification {
		/// <summary>
		/// Server ID.
		/// </summary>
		public uint Id;
	}
}