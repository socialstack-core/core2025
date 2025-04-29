using System.Threading.Tasks;
using Api.Contexts;
using Api.Eventing;
using System;
using Api.Users;
using Microsoft.AspNetCore.Http;
using Api.Pages;
using Api.Permissions;

namespace Api.UserLastVisited
{
	/// <summary>
	/// Handles users. Updates last visited timestamp.
	/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
	/// </summary>
	public partial class UserLastVisitedService : AutoService
	{
		private UserLastVisitedConfig _config;
		private UserService _users;
		
		/// <summary>
		/// Instanced automatically. Use injection to use this service, or Startup.Services.Get.
		/// </summary>
		public UserLastVisitedService(UserService users)
        {
			_users = users;
			_config = GetConfig<UserLastVisitedConfig>();
			
            Events.Page.BeforeNavigate.AddEventListener((Context context, Page page, string url) => {
			
				if (_config != null && !_config.Disabled && context.UserId > 0)
				{
					var ctx = context;

                    // Don't await this one:
                    _ = Task.Run(async () =>
					{
						if (ctx.User != null && ctx.User.LastVisitedUtc.Date != DateTime.UtcNow.Date)
						{
                            await _users.Update(ctx, ctx.User, (Context ctx, User usr, User orig) =>
							{
								usr.LastVisitedUtc = DateTime.UtcNow.Date;
							}, DataOptions.IgnorePermissions | DataOptions.CheckNotChanged);
						}
					});		
				}

                return new ValueTask<Page>(page);
            });
		}
	}    
}
