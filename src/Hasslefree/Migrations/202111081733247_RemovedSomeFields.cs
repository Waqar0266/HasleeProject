namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedSomeFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("Tenant", "Fax");
            DropColumn("Tenant", "Mobile");
            DropColumn("Tenant", "Email");
        }
        
        public override void Down()
        {
            AddColumn("Tenant", "Email", c => c.String(maxLength: 100, storeType: "nvarchar"));
            AddColumn("Tenant", "Mobile", c => c.String(maxLength: 20, storeType: "nvarchar"));
            AddColumn("Tenant", "Fax", c => c.String(maxLength: 20, storeType: "nvarchar"));
        }
    }
}
