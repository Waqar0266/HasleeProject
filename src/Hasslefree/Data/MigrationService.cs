using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using Hasslefree.Core;
using Hasslefree.Core.Data;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Data
{
	public class MigrationService : IMigrationService
	{
		#region Constants

		private const String MigrationPrefix = "Hasslefree.Data.Migrations.Up.";
		private const string DefaultContext = "DatabaseContext";

		private const String GetInitialSql = @"
SELECT `MigrationId` FROM `__MigrationHistory` WHERE `ContextKey` = 'Hasslefree.Data.HasslefreeDatabaseContextMySql';
";

		private const String CreateMigrationsTableSql = @"
CREATE TABLE IF NOT EXISTS `__Migrations`
(
	`MigrationId` NVARCHAR(128) PRIMARY KEY,
    `DateRunUtc` DATETIME ,
    `Custom` BIT DEFAULT 0
);
";

		private const String CreateMigrationSql = @"
INSERT INTO `__Migrations`
VALUES('{0}', UTC_TIMESTAMP(), {1});
";

		private const String GetMigrationsSql = "SELECT * FROM `__Migrations`;";

		#endregion

		#region Statics

		private static Dictionary<String, Boolean> _databasesMigrated = new Dictionary<String, Boolean>(StringComparer.OrdinalIgnoreCase);
		private static readonly Object MigrationLock = new Object();

		public static IMigrationService Current => EngineContext.Current.Resolve<IMigrationService>();

		#endregion

		#region Properties

		private IConnectionStringResolver ConnectionStringResolver { get; }
		private IWebHelper Web { get; }

		private String DataFolder => Web.MapPath("~/App_Data/MySql");
		private List<String> ScriptFiles => new List<String>()
		{
			"\\Events.sql",
			"\\Functions.sql",
			"\\Procedures.sql",
			"\\Views.sql",
			"\\Indexes.sql"
		};

		#endregion

		#region Fields

		private DbContext _dbContext;
		private String _databaseName;
		private String _connectionString;

		#endregion

		#region Constructor

		public MigrationService(
				IConnectionStringResolver connectionStringResolver,
				IWebHelper web
			)
		{
			ConnectionStringResolver = connectionStringResolver;
			Web = web;
		}

		#endregion

		#region IMigrationService

		public void Migrate(DbContext context)
		{
			_dbContext = context;
			_connectionString = GetConnectionString(DefaultContext);

			if (_databasesMigrated.ContainsKey(_connectionString))
				return;

			try
			{
				_databaseName = _dbContext.Database.Connection.Database;

				lock (MigrationLock)
				{
					if (_databasesMigrated.ContainsKey(_connectionString))
						return;

					// Create the database if it doesn't exist
					// Creates with InitialCreate
					var created = _dbContext.Database.CreateIfNotExists();

					using (var conn = new MySqlConnection(_connectionString))
					{
						if (conn.State != ConnectionState.Open)
							conn.Open();

						Migrations(conn);

						if(created)
							Scripts(conn);
					}

					if (!_databasesMigrated.ContainsKey(_connectionString))
						_databasesMigrated.Add(_connectionString, true);
				}
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}

			Clean();
		}

		#endregion

		#region Private Methods

		private void Clean()
		{
			_dbContext = null;
			_databaseName = String.Empty;
			_connectionString = String.Empty;
		}

		/// <summary>
		/// Get the connection string to use with the context
		/// </summary>
		/// <returns></returns>
		private String GetConnectionString(string name)
		{
			return ConnectionStringResolver.WithName(name).ConnectionString;
		}

		private String GetScript(String location)
		{
			var script = File.ReadAllText(location);

			return script;
		}

		#region Migrations

		private void Migrations(MySqlConnection conn)
		{
			// Create __Migrations Table
			CreateMigrationsTable(conn);

			// Get all migrations
			var allMigrations = GetAllMigrations();

			// Get initial migration
			var initialMigration = GetInitialMigration(conn);

			// Get all migrations already run
			var runMigrations = GetRunMigrations(conn);

			// Calculate which migrations still need to be run
			var newMigrations = GetNewMigrations(allMigrations, initialMigration, runMigrations);

			// Run migrations
			RunMigrations(conn, newMigrations);
		}

		private List<FileMigration> GetAllMigrations() => GetCoreMigrations().Union(GetCustomMigrations()).ToList();

		private List<FileMigration> GetCoreMigrations()
		{
			return Assembly.GetExecutingAssembly()
				.GetManifestResourceNames()
				.Where(a => a.StartsWith(MigrationPrefix))
				.Select(a => new FileMigration
				{
					MigrationId = a.Replace(MigrationPrefix, ""),
					Custom = false
				})
				.ToList();
		}

		private List<FileMigration> GetCustomMigrations()
		{
			var customScriptFolder = Path.Combine(DataFolder, _databaseName, "up");
			if (!Directory.Exists(customScriptFolder))
				return new List<FileMigration>();

			var customScripts = Directory.GetFiles(customScriptFolder).Where(a => !ScriptFiles.Any(a.EndsWith)).ToList();
			return customScripts.Select(a => new FileMigration()
			{
				MigrationId = Path.GetFileName(a),
				Custom = true,
				Path = a
			}).ToList();
		}

		private String GetMigration(FileMigration migration) => migration.Custom ? GetScript(migration.Path) : GetCoreMigration(migration.MigrationId);

		private String GetCoreMigration(String migrationId)
		{
			var location = MigrationPrefix + migrationId;

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(location))
			{
				if (stream == null)
					return String.Empty;

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		private void CreateMigrationsTable(MySqlConnection connection)
		{
			using (var cmd = new MySqlCommand(CreateMigrationsTableSql, connection))
			{
				cmd.ExecuteNonQuery();
			}
		}

		private String GetInitialMigration(MySqlConnection connection)
		{
			var oldMigrations = connection.Query<String>(GetInitialSql).ToList();

			if (oldMigrations.Count > 1)
			{
				if (!oldMigrations.Contains("202010131111047_OrderQtyBehavior"))
					throw new Exception("Database is not in a state to move to the new migration framework.");
			}

			return oldMigrations.FirstOrDefault();
		}

		private Dictionary<String, DbMigration> GetRunMigrations(MySqlConnection connection) => connection.Query<DbMigration>(GetMigrationsSql).ToDictionary(a => a.MigrationId);

		private HashSet<FileMigration> GetNewMigrations(List<FileMigration> allMigrations, String initialMigration, Dictionary<String, DbMigration> runMigrations)
		{
			return (from am in allMigrations
					where
						String.Compare(initialMigration, am.MigrationId, StringComparison.OrdinalIgnoreCase) <= 0 &&
						!runMigrations.ContainsKey(am.MigrationId)
					select am).ToHashSet();
		}

		private void RunMigrations(MySqlConnection connection, HashSet<FileMigration> newMigrations)
		{
			foreach (var migration in newMigrations)
			{
				var sql = GetMigration(migration);
				using (var cmd = new MySqlCommand(sql, connection))
				{
					cmd.ExecuteNonQuery();
				}

				using (var cmd = new MySqlCommand(String.Format(CreateMigrationSql, migration.MigrationId, migration.Custom ? 1 : 0), connection))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}

		#endregion

		#region Scripts

		/// <summary>
		/// Gets and executes all scripts
		/// These are found in App_Data\MySql and also in each individual database folder
		/// </summary>
		/// <param name="conn"></param>
		private void Scripts(MySqlConnection conn)
		{
			var allScripts = GetAllScripts();

			RunScripts(conn, allScripts);
		}

		private List<String> GetAllScripts() => GetCoreScripts().Union(GetCustomScripts()).ToList();

		private List<String> GetCoreScripts()
		{
			var coreScripts = Directory.GetFiles(DataFolder).Where(a => ScriptFiles.Any(a.EndsWith)).ToList();

			return coreScripts;
		}

		private List<String> GetCustomScripts()
		{
			var customScriptFolder = Path.Combine(DataFolder, _databaseName);
			if (!Directory.Exists(customScriptFolder))
				return new List<String>();

			var customScripts = Directory.GetFiles(customScriptFolder).Where(a => ScriptFiles.Any(a.EndsWith)).ToList();
			return customScripts;
		}

		private void RunScripts(MySqlConnection connection, List<String> scripts)
		{
			foreach (var sql in scripts.Select(GetScript))
			{
				if (String.IsNullOrWhiteSpace(sql))
					continue;

				using (var cmd = new MySqlCommand(sql, connection))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}

		#endregion

		#endregion

		#region Private Classes

		private class DbMigration
		{
			public String MigrationId { get; set; }
			public DateTime DateRunUtc { get; set; }
			public Boolean Custom { get; set; }
		}

		private class FileMigration
		{
			public String MigrationId { get; set; }
			public Boolean Custom { get; set; }
			public String Path { get; set; }
		}

		#endregion
	}
}
