namespace Hasslefree.Data.Migrations
{
	using System.Data.Entity.Migrations;

	public class Configuration : DbMigrationsConfiguration<HasslefreeDatabaseContextMySql>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			ContextKey = "Hasslefree.Data.HasslefreeDatabaseContextMySql";

			// Use MySQL
			SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.EntityFramework.MySqlMigrationSqlGenerator());
			CodeGenerator = new MySql.Data.EntityFramework.MySqlMigrationCodeGenerator();
		}

		protected override void Seed(HasslefreeDatabaseContextMySql context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}
	}
}
