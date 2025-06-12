﻿using Api.Startup;
using Api.Eventing;
using Api.Contexts;
using Api.Permissions;
using System.Threading.Tasks;

namespace Api.Addresses
{
	/// <summary>
	/// Instances capabilities during the very earliest phases of startup.
	/// </summary>
	[EventListener]
	public class Permissions
	{
		/// <summary>
		/// Instanced automatically.
		/// </summary>
		public Permissions()
		{
			// Hook the default role setup. It's done like this so it can be removed by a plugin if wanted.
			Events.CapabilityOnSetup.AddEventListener((Context context, object source) =>
			{
				// Remove public viewing (as it's enabled by default):
				Roles.Guest.Revoke("address_load", "address_list");
				Roles.Public.Revoke("address_load", "address_list");
				Roles.Member.Revoke("address_load", "address_list");
				Roles.Member.If("IsSelf()").ThenGrant("address_load", "address_list");

				return new ValueTask<object>(source);
			}, 20);
		}
	}
}