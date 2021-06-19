using System.Data.Entity;

namespace Hasslefree.Data.Initializers
{
	/// <summary>
	/// This is the standard Hasslefree database initializer.
	/// </summary>
	/// <remarks>
	/// This initializer will check if the database exists, and if not, it will create it.
	/// It will also look for a file called ~/App_Data/Install/SqlServer/Constraints.sql and a file called ~/App_Data/Install/SqlServer/Indexes.sql and execute them against the database.
	/// </remarks>
	public class MySqlInitializer : MigrateDatabaseToLatestVersion<HasslefreeDatabaseContextMySql, Migrations.Configuration>
	{

	}
}
