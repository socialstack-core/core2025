using Api.Startup;

namespace Api.Permissions{
	
	[ListAs("RolePermits", IsPrimary = false)]
	[ListAs("RoleExclusions", IsPrimary = false)]
	public partial class Role{}
	
}