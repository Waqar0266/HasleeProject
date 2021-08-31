using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyOverviewKeyValueConfiguration : EntityTypeConfiguration<PropertyOverviewKeyValue>
	{
		public PropertyOverviewKeyValueConfiguration()
		{
			// Table
			ToTable("PropertyOverviewKeyValue");

			// Primary Key
			HasKey(a => a.PropertyOverviewKeyValueId);

			HasRequired(x => x.Property)
			.WithMany(x => x.OverviewKeyValues)
			.HasForeignKey(x => x.PropertyId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.Key).IsRequired().HasMaxLength(100);
			Property(a => a.Value).IsRequired().HasMaxLength(200);
		}
	}
}
