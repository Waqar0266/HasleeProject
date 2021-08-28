using Hasslefree.Core.Domain.Rentals;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Rentals
{
	public class RentalResolutionMemberConfiguration : EntityTypeConfiguration<RentalResolutionMember>
	{
		public RentalResolutionMemberConfiguration()
		{
			// Table
			ToTable("RentalResolutionMember");

			// Primary Key
			HasKey(a => a.RentalResolutionMemberId);

			HasRequired(a => a.RentalResolution)
			.WithMany()
			.HasForeignKey(a => a.RentalResolutionId)
			.WillCascadeOnDelete(false);

			HasOptional(a => a.Signature)
			.WithMany()
			.HasForeignKey(a => a.SignatureId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.IdNumber).IsRequired().HasMaxLength(30);
			Property(a => a.Name).IsRequired().HasMaxLength(50);
			Property(a => a.Surname).IsRequired().HasMaxLength(50);
			Property(a => a.SignedAt).IsOptional().HasMaxLength(100);
			Property(a => a.SignedOn).IsOptional();
		}
	}
}
