namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class TenantPerson : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM TenantDocumentation WHERE TenantId > 0");
            Sql("DELETE FROM Tenant WHERE TenantId > 0");
            AddColumn("Tenant", "PersonId", c => c.Int(nullable: true));
            CreateIndex("Tenant", "PersonId");
            AddForeignKey("Tenant", "PersonId", "Person", "PersonId");
        }

        public override void Down()
        {
            DropForeignKey("Tenant", "PersonId", "Person");
            DropIndex("Tenant", new[] { "PersonId" });
            DropColumn("Tenant", "PersonId");
        }
    }
}
