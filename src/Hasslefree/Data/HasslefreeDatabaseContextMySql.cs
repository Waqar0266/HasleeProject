using Dapper;
using Hasslefree.Core.Domain;
using Hasslefree.Data.Initializers;
using Hasslefree.Data.Models;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hasslefree.Data
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class HasslefreeDatabaseContextMySql : DbContext, IDataContext, IReadOnlyContext
	{
		/// <summary>
		/// A lock used to prevent 2 threads resolving the same connection string
		/// </summary>
		private static readonly object Lock = new object();

		/// <summary>
		/// Easy 1 spot replace.
		/// </summary>
		private const string DefaultContext = "DatabaseContext";

		#region Constructor

		/// <summary>
		/// Initialize the database context with a database initializer
		/// </summary>
		/// <param name="initializer"></param>
		public HasslefreeDatabaseContextMySql(MySqlInitializer initializer) : base(GetConnectionString(DefaultContext))
		{
			Configuration.LazyLoadingEnabled = false;

			// Set the DB initializer
			Database.SetInitializer(initializer);

			// Enable query logging
			// PS: Don't activate this and push. You will die.
			// Database.Log = LogQuery;
		}

		/// <summary>
		/// Initialize the database context with a database initializer
		/// </summary>
		public HasslefreeDatabaseContextMySql() : base(GetConnectionString(DefaultContext))
		{
			Configuration.LazyLoadingEnabled = false;

			// Set the DB initializer
			Database.SetInitializer(new MySqlInitializer());

			// Enable query logging
			// PS: Don't activate this and push. You will die.
			// Database.Log = LogQuery;
		}

		/// <summary>
		/// Initialize database connection with certain name
		/// </summary>
		/// <param name="name"></param>
		public HasslefreeDatabaseContextMySql(string name) : base(GetConnectionString(name))
		{
			//Disable lazy loading
			Configuration.LazyLoadingEnabled = false;

			var objectContext = (this as IObjectContextAdapter).ObjectContext;

			// Sets the command timeout for all the commands
			objectContext.CommandTimeout = 600;

			//PS: Don't activate this and push. You will die.
			//Database.Log = LogQuery;
		}

		/// <summary>
		/// Get the connection string to use with the context
		/// </summary>
		/// <returns></returns>
		private static string GetConnectionString(string name)
		{
			lock (Lock)
			{
				return ConfigurationManager.ConnectionStrings[name].ConnectionString;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the specified entity set from the context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public new DbSet<T> Set<T>() where T : BaseEntity => base.Set<T>();

		private DbContext _dbContext;
		private DbContext ThisDbContext => _dbContext ?? (_dbContext = (DbContext)this);

		#endregion

		#region Methods

		/// <summary>
		/// Create the model based on the entity type configuration mappings
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());

			//Don't pluralize table names
			modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();

			base.OnModelCreating(modelBuilder);
		}

		/// <summary>
		/// Indicates if the database targeted by the connection string exists
		/// </summary>
		/// <returns></returns>
		public bool DatabaseExists() => Database.Exists();

		public void ExecuteSql(string sql)
		{
			((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
			Database.ExecuteSqlCommand(sql);
		}

		public Task ExecuteSqlAsync(string sql)
		{
			((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
			return Database.ExecuteSqlCommandAsync(sql);
		}

		public int Execute(string sql, Dictionary<string, string> parameters, int timeout = 30)
		{
			using (var connection = Database.Connection)
			{
				if (connection.State != ConnectionState.Open)
					connection.Open();

				using (var cmd = connection.CreateCommand())
				{
					cmd.CommandText = sql;
					cmd.CommandTimeout = timeout;

					foreach (var parameter in parameters)
					{
						var para = cmd.CreateParameter();
						para.ParameterName = parameter.Key;
						para.Value = parameter.Value;
						cmd.Parameters.Add(para);
					}

					return cmd.ExecuteNonQuery();
				}
			}
		}

		public Task<int> ExecuteAsync(string sql, Dictionary<string, string> parameters, int timeout = 30)
		{
			using (var connection = Database.Connection)
			{
				if (connection.State != ConnectionState.Open)
					connection.Open();

				using (var cmd = connection.CreateCommand())
				{
					cmd.CommandText = sql;
					cmd.CommandTimeout = timeout;

					foreach (var parameter in parameters)
					{
						var para = cmd.CreateParameter();
						para.ParameterName = parameter.Key;
						para.Value = parameter.Value;
						cmd.Parameters.Add(para);
					}

					return cmd.ExecuteNonQueryAsync();
				}
			}
		}

		public IEnumerable<T> Query<T>(string sql, object param = null, Boolean dapper = false)
		{
			if (dapper)
				return Connection.Query<T>(sql, param);

			if (param == null)
				return Database.SqlQuery<T>(sql);

			var type = param.GetType();
			var props = type.GetProperties();

			if (ProviderName.ToLower().Contains("mysql"))
			{
				var mpairs = props.Select(x => new MySqlParameter(String.Concat("@", x.Name), x.GetValue(param, null))).ToArray();

				// ReSharper disable once CoVariantArrayConversion
				return Database.SqlQuery<T>(sql, mpairs);
			}

			var pairs = props.Select(x => new SqlParameter(String.Concat("@", x.Name), x.GetValue(param, null))).ToArray();

			// ReSharper disable once CoVariantArrayConversion
			return Database.SqlQuery<T>(sql, pairs);
		}

		public void BulkSaveChanges() => ThisDbContext.BulkSaveChanges();

		public Task BulkSaveChangesAsync() => ThisDbContext.BulkSaveChangesAsync();

		public DbContextTransaction BeginTransaction() => ThisDbContext.Database.BeginTransaction();

		public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel) => ThisDbContext.Database.BeginTransaction(isolationLevel);

		public void BulkInsert<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			ThisDbContext.BulkInsert(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
				options.ColumnInputExpression = defaultOptions.ColumnInputExpression ?? options.ColumnInputExpression;
			});
		}

		public Task BulkInsertAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			return ThisDbContext.BulkInsertAsync(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.AutoMapOutputDirection = defaultOptions.AutoMapOutputDirection;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
				options.ColumnInputExpression = defaultOptions.ColumnInputExpression ?? options.ColumnInputExpression;
			});
		}

		public void BulkUpdate<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			ThisDbContext.BulkUpdate(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
				options.ColumnInputExpression = defaultOptions.ColumnInputExpression ?? options.ColumnInputExpression;
			});
		}

		public Task BulkUpdateAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			return ThisDbContext.BulkUpdateAsync(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
				options.ColumnInputExpression = defaultOptions.ColumnInputExpression ?? options.ColumnInputExpression;
			});
		}

		public void BulkMerge<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			ThisDbContext.BulkMerge(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.AutoMapOutputDirection = defaultOptions.AutoMapOutputDirection;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
				options.ColumnInputExpression = defaultOptions.ColumnInputExpression ?? options.ColumnInputExpression;
				options.IgnoreOnMergeUpdateExpression = defaultOptions.IgnoreOnMergeUpdateExpression ?? options.IgnoreOnMergeUpdateExpression;
			});
		}

		public Task BulkMergeAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			return ThisDbContext.BulkMergeAsync(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.AutoMapOutputDirection = defaultOptions.AutoMapOutputDirection;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
				options.ColumnInputExpression = defaultOptions.ColumnInputExpression ?? options.ColumnInputExpression;
				options.IgnoreOnMergeUpdateExpression = defaultOptions.IgnoreOnMergeUpdateExpression ?? options.IgnoreOnMergeUpdateExpression;
			});
		}

		public void BulkDelete<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			ThisDbContext.BulkDelete(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
			});
		}

		public Task BulkDeleteAsync<T>(IEnumerable<T> entities, Action<BulkOperationOptions<T>> bulkOperationFactory = null) where T : BaseEntity
		{
			var defaultOptions = new BulkOperationOptions<T>();
			bulkOperationFactory?.Invoke(defaultOptions);

			return ThisDbContext.BulkDeleteAsync(entities, options =>
			{
				options.IncludeGraph = defaultOptions.IncludeGraph;
				options.ColumnPrimaryKeyExpression = defaultOptions.ColumnPrimaryKeyExpression ?? options.ColumnInputExpression;
			});
		}

		public Dictionary<String, Object> GetPrimaryKey<T>(T entity) where T : BaseEntity
		{
			Object GetEntityValue(String name) => entity.GetType().GetProperty(name)?.GetValue(entity, null);

			var primaryKeyProperties = GetKeyNames<T>();

			return (from p in primaryKeyProperties
					select new
					{
						Name = p,
						Value = GetEntityValue(p)
					}).ToDictionary(a => a.Name, b => b.Value);
		}

		#endregion

		/// <summary>
		/// Log the query to disk
		/// </summary>
		/// <param name="sql"></param>
		[SuppressMessage("ReSharper", "UnusedMember.Local")]
		private void LogQuery(string sql)
		{
			var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\querylog.txt");
			using (var sw = new System.IO.StreamWriter(path, true))
			{
				sw.Write(sql);
				sw.Close();
			}
		}

		/// <summary>
		/// Returns the database connection for the context
		/// </summary>
		public DbConnection Connection => Database.Connection;

		public string ProviderName => ConfigurationManager.ConnectionStrings[DefaultContext].ProviderName;

		private String[] GetKeyNames<T>() where T : BaseEntity
		{
			var metadata = ((IObjectContextAdapter)ThisDbContext).ObjectContext.MetadataWorkspace;

			// Get the mapping between CLR types and metadata OSpace
			var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

			// Get metadata for given CLR type
			var entityMetadata = metadata
				.GetItems<EntityType>(DataSpace.OSpace)
				.Single(e => objectItemCollection.GetClrType(e) == typeof(T));

			return entityMetadata.KeyProperties.Select(p => p.Name).ToArray();
		}
	}
}
