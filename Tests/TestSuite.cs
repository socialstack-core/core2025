using Api.Contexts;
using Api.Permissions;
using Api.Startup;
using Api.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    [Collection("Global Setup Collection")]
	public abstract partial class TestSuite<ServiceType> : IDisposable
        where ServiceType : AutoService
    {
        private readonly List<User> actors = new();

        private readonly List<Context> actorContexts = new();

        private readonly Context developerContext = new Context(new Role(){
            Id = 1,
            UserId = 1
        });

        protected readonly ServiceType service;

        public TestSuite()
        {
            service = Services.Get<ServiceType>();
        }

        /// <summary>
        /// Gets a collection of actors so we can test permissions are correctly assigned.
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<User>> GetActors()
        {
            if (!actors.Any())
            {
                // load actors, one per role. 

                var roleService = Services.Get<RoleService>();
                var userService = Services.Get<UserService>();
                
                if (roleService is null)
                {
                    throw new Exception("Role service is null");
                }
                
                if (userService is null)
                {
                    throw new Exception("UserService is null");
                }
                
                
                // lets create one per role.

                var allRoles = await roleService.Where("Id > ?", DataOptions.IgnorePermissions).Bind((uint) 0).ListAll(developerContext);

                if (allRoles is null)
                {
                    throw new Exception("No Roles found");
                }

                if (!allRoles.Any())
                {
                    throw new Exception("No Roles found");
                }

                foreach(var role in allRoles)
                {
                    var user = new User(){
                        Username = "Unit Test " + role.Name,
                        Email = $"actor{role.Name}@nomail.com",
                        Role = role.Id
                    };
                    
                    actors.Add(await userService.Create(developerContext, user, DataOptions.IgnorePermissions));

                };
            }
            return actors;
        }

        public Context GetDeveloperContext()
        {
            return developerContext;
        }
        public Context GetContext(User user)
        {
            var contextLookup = actorContexts.Where(context => context.RoleId == user.Role);

            if (contextLookup.Any())
            {
                return contextLookup.First();
            }
            var context = new Context(new Role(){
                Id = user.Role,
                UserId = user.Id
            });

            actorContexts.Add(context);
            return context;
        }

        public async ValueTask<User> GetActorByRole(Role role)
        {
            return await GetActorByRole(role.Id);            
        } 
        public async ValueTask<User> GetActorByRole(uint RoleId)
        {
            List<User> actorCollection;

            if (!actors.Any())
            {
                actorCollection = await GetActors();
            } else {
                actorCollection = actors;
            }
            var actor = actorCollection.Where(actor => actor.Role == RoleId);

            return actor.Any() ? actor.First() : null;
        }

        public void Dispose()
        {
            // cleaup happens here
        }
    }
}