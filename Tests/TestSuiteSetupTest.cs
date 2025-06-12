using Api.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
	public class TestSuiteSetupTest : TestSuite<UserService>
	{
		[Fact]
		public async Task EnsureTestSuiteSetup()
		{
			var actors = await GetActors();

			if (actors is null)
			{	
				throw new Exception("Actors is null");
			}
			Assert.True(actors.Any());
		}
	}
}	