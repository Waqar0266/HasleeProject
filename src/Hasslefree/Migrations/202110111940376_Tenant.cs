namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tenant : DbMigration
    {
        public override void Up()
        {
            AddColumn("ExistingRental", "Tenant", c => c.String(maxLength: 155, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("ExistingRental", "Tenant");
        }
    }
}
