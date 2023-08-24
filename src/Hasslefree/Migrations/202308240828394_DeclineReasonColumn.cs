namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeclineReasonColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("RentalT", "DeclineReason", c => c.String(maxLength: 1024, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("RentalT", "DeclineReason");
        }
    }
}
