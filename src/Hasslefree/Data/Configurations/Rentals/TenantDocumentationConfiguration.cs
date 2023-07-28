using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
    public class TenantDocumentationConfiguration : EntityTypeConfiguration<TenantDocumentation>
    {
        public TenantDocumentationConfiguration()
        {
            // Table
            ToTable("TenantDocumentation");

            // Primary Key
            HasKey(a => a.TenantDocumentationId);

            HasRequired(a => a.Download)
            .WithMany()
            .HasForeignKey(a => a.DownloadId)
            .WillCascadeOnDelete(false);

            HasRequired(a => a.Tenant)
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .WillCascadeOnDelete(false);

            // Columns
            Property(a => a.CreatedOn).IsRequired();
        }
    }
}
