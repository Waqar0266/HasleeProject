using Hasslefree.Core.Domain.Common;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Common
{
	public class FirmConfiguration : EntityTypeConfiguration<Firm>
	{
		public FirmConfiguration()
		{
			// Table
			ToTable("Firm");

			// Primary Key
			HasKey(a => a.FirmId);

			//Foreign Keys
			HasRequired(a => a.PhysicalAddress)
			.WithMany()
			.HasForeignKey(a => a.PhysicalAddressId)
			.WillCascadeOnDelete(true);

			HasRequired(a => a.PostalAddress)
			.WithMany()
			.HasForeignKey(a => a.PostalAddressId)
			.WillCascadeOnDelete(true);

			// Columns
			Property(a => a.ModifiedOn).IsRequired();

			Property(a => a.AiNumber).HasMaxLength(128);
			Property(a => a.BusinessName).HasMaxLength(128);
			Property(a => a.Email).HasMaxLength(128);
			Property(a => a.Fax).HasMaxLength(32);
			Property(a => a.Phone).HasMaxLength(32);
			Property(a => a.ReferenceNumber).HasMaxLength(55);
			Property(a => a.TradeName).HasMaxLength(128);
		}
	}
}
