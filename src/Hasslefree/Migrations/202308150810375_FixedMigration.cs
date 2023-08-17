namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "RentalTAgentDocumentation",
                c => new
                {
                    RentalTAgentDocumentationId = c.Int(nullable: false, identity: true),
                    CreatedOn = c.DateTime(nullable: false, precision: 0),
                    AgentId = c.Int(nullable: false),
                    RentalTId = c.Int(nullable: false),
                    DownloadId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.RentalTAgentDocumentationId)
                .ForeignKey("Agent", t => t.AgentId)
                .ForeignKey("Download", t => t.DownloadId)
                .ForeignKey("RentalT", t => t.RentalTId);
                //.Index(t => t.AgentId)
                //.Index(t => t.RentalTId)
                //.Index(t => t.DownloadId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("RentalTAgentDocumentation", "RentalTId", "RentalT");
            DropForeignKey("RentalTAgentDocumentation", "DownloadId", "Download");
            DropForeignKey("RentalTAgentDocumentation", "AgentId", "Agent");
            DropIndex("RentalTAgentDocumentation", new[] { "DownloadId" });
            DropIndex("RentalTAgentDocumentation", new[] { "RentalTId" });
            DropIndex("RentalTAgentDocumentation", new[] { "AgentId" });
            DropTable("RentalTAgentDocumentation");
        }
    }
}
