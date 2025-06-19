using Api.Configuration;
using Api.Contexts;
using Api.Database;
using Api.Eventing;
using Api.Permissions;
using Api.Startup;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class GlobalSetupFixture : IDisposable
{
	private readonly static List<string> CleanTables = new();

	
	private MySQLDatabaseService _database;

	private string _mainDatabaseName;

	private string _testDatabaseName;
	
	/// <summary>
	/// Constructor runs before any tests
	/// </summary>
	public GlobalSetupFixture()
	{
		// Hello! The very first thing we'll do is instance all event handlers.
		Api.Eventing.Events.Init();

		// Tell other code (mainly mysql) that we are running as an xunit test collection:
		Services.BuildHost = "xunit";

		
		// Clone stdout into error engine:
		StdOut.Writer = new ConsoleWriter(Console.Out);
		Console.SetOut(StdOut.Writer);

		var setupHandlersMethod = GetType().GetMethod(nameof(SetupService));

		if (setupHandlersMethod is null)
		{
			throw new Exception("Setup handler is null");
		}
 
		Events.Service.AfterCreate.AddEventListener((Context context, AutoService service) => {
		
			if (service == null || service.ServicedType == null)
			{
				return new ValueTask<AutoService>(service);
			}
			var servicedType = service.ServicedType;
		
			if (servicedType != null)
			{
				// Add data load events:
				var setupType = setupHandlersMethod.MakeGenericMethod(new Type[] {
					servicedType,
					service.IdType
				});
		
				setupType.Invoke(this, new object[] {
					service
				});
			}
			return new ValueTask<AutoService>(service);
		}, 2);

		// Next we find any EventListener classes.
		var allTypes = typeof(EntryPoint).Assembly.DefinedTypes;

		foreach (var typeInfo in allTypes)
		{
			// If it:
			// - Is a class
			// - Has the EventListener attribute
			// Then we instance it.

			if (!typeInfo.IsClass)
			{
				continue;
			}

			if (typeInfo.GetCustomAttributes(typeof(EventListenerAttribute), true).Length == 0)
			{
				continue;
			}

			// Got one - instance it now:
			Activator.CreateInstance(typeInfo);
		}

		// Ok - modules have now connected any core events or have performed early startup functionality.

		Task.Run(async () =>
		{
			// Fire off initial OnStart handlers:
			await Api.Eventing.Events.TriggerStart();
		}).Wait();

		string apiSocketFile = null;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			// Listen on a Unix socket too:
			apiSocketFile = System.IO.Path.GetFullPath("api.sock");

			try
			{
				// Delete if exists:
				System.IO.File.Delete(apiSocketFile);
			}
			catch { }
		}

		// Get environment name:
		var env = "dev";
		
		// Set environment:
		Services.Environment = Services.SanitiseEnvironment(env);
		Services.OriginalEnvironment = env;

		// web server keeps the tests running, 
		// need to rethink this

		Services.RegisterAndStart();

		
	}
	/// <summary>
	/// Sets up for the given type with its event group along with updating any DB tables.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="ID"></typeparam>
	/// <param name="service"></param>
	public void SetupService<T, ID>(AutoService<T, ID> service)
		where T : Content<ID>, new()
		where ID : struct, IConvertible, IEquatable<ID>, IComparable<ID>
	{
		var isDbStored = service.DataIsPersistent;

		if (!isDbStored)
		{
			return;
		}
		
		service.EventGroup.Delete.AddEventListener(async (Context context, T entity) =>
		{
			await EnsureClean(service);
			return entity;
		}, 1);

		service.EventGroup.Create.AddEventListener(async (Context context, T entity) =>
		{
			await EnsureClean(service);
			return entity;
		}, 1);

		service.EventGroup.Update.AddEventListener(async (Context context, T entity, ChangedFields changes, DataOptions opts) => {
			await EnsureClean(service);
			return entity;
		}, 1);

		service.EventGroup.List.AddEventListener(async (Context context, QueryPair<T, ID> entity) => {
			await EnsureClean(service);
			return entity;
		}, 1);

		service.EventGroup.Load.AddEventListener(async (Context context, T entity, ID id) => {
			await EnsureClean(service);
			return entity;
		}, 1);
	}

    // Implement IDisposable to run teardown code after all tests
    public void Dispose()
	{
		// Teardown code here
		Console.WriteLine("Global teardown - runs once after all tests.");
	}


	public async ValueTask EnsureClean(AutoService service) {
		var tableName = MySQLSchema.TableName(service.EntityName);
		if (!CleanTables.Contains(tableName))
		{
			CleanTables.Add(tableName);

			// clean the table
			// -> ask mysql service to copy table from template DB -> the running one

			if (_database == null)
			{
				_database = Services.Get<MySQLDatabaseService>();
			}

			if ( string.IsNullOrEmpty(_mainDatabaseName) || string.IsNullOrEmpty(_testDatabaseName) )
			{
				var connectionStrings = AppSettings.GetSection("MongoConnectionStrings") ?? 
															 AppSettings.GetSection("ConnectionStrings") ?? 
															 throw new Exception("No connection strings are present");

				var mainDbConnectionString = connectionStrings[
					System.Environment.GetEnvironmentVariable("ConnectionStringName") ?? "DefaultConnection"
				] ?? throw new Exception("Default connection string is null");

				var testDbConnectionString = connectionStrings["TestingConnection"] ?? throw new Exception("Test connection string is empty");

				var mainDbConnection = new Uri(mainDbConnectionString);
				var testDbConnection = new Uri(testDbConnectionString);

				if (mainDbConnection.AbsolutePath == testDbConnection.AbsolutePath)
				{
					throw new Exception("You cannot perform tests against your development database");
				}
			}
        }
    }

	protected static List<string> GetDirtyTableTypes() 
	{
		return CleanTables;
	}
}


[CollectionDefinition("Global Setup Collection")]
public class GlobalSetupCollection : ICollectionFixture<GlobalSetupFixture>
{
    // This class has no code, and is never created. 
    // Its purpose is simply to apply [CollectionDefinition] and ICollectionFixture<> interfaces.
}
