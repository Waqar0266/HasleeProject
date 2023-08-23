namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LandlordApprovalColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("RentalT", "LandlordApproved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("RentalT", "LandlordApproved");
        }
    }
}
