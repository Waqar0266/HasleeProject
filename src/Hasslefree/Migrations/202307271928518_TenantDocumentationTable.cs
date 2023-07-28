namespace Hasslefree.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenantDocumentationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TenantDocumentation",
                c => new
                    {
                        TenantDocumentationId = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false, precision: 0),
                        TenantId = c.Int(nullable: false),
                        DownloadId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenantDocumentationId)                
                .ForeignKey("Download", t => t.DownloadId)
                .ForeignKey("Tenant", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.DownloadId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TenantDocumentation", "TenantId", "Tenant");
            DropForeignKey("TenantDocumentation", "DownloadId", "Download");
            DropIndex("TenantDocumentation", new[] { "DownloadId" });
            DropIndex("TenantDocumentation", new[] { "TenantId" });
            DropTable("TenantDocumentation");
        }
    }
}
