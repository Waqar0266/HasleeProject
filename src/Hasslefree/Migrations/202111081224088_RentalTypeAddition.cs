namespace Hasslefree.Data.Migrations
{
	using System.Data.Entity.Migrations;

	public partial class RentalTypeAddition : DbMigration
    {
        public override void Up()
        {
            AddColumn("RentalT", "RentalTTypeEnum", c => c.String(nullable: false, maxLength: 35, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("RentalT", "RentalTTypeEnum");
        }
    }
}
