using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
    public class RentalTAgentDocumentationConfiguration : EntityTypeConfiguration<RentalTAgentDocumentation>
    {
        public RentalTAgentDocumentationConfiguration()
        {
            // Table
            ToTable("RentalTAgentDocumentation");

            // Primary Key
            HasKey(a => a.RentalTAgentDocumentationId);

            HasRequired(a => a.Download)
            .WithMany()
            .HasForeignKey(a => a.DownloadId)
            .WillCascadeOnDelete(false);

            HasRequired(a => a.Agent)
            .WithMany()
            .HasForeignKey(a => a.AgentId)
            .WillCascadeOnDelete(false);

            HasRequired(a => a.RentalT)
            .WithMany()
            .HasForeignKey(a => a.RentalTId)
            .WillCascadeOnDelete(false);

            // Columns
            Property(a => a.CreatedOn).IsRequired();
        }
    }
}
