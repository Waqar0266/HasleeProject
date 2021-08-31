using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyOtherFeaturesKeyValueConfiguration : EntityTypeConfiguration<PropertyOtherFeaturesKeyValue>
	{
		public PropertyOtherFeaturesKeyValueConfiguration()
		{
			// Table
			ToTable("PropertyOtherFeaturesKeyValue");

			// Primary Key
			HasKey(a => a.PropertyOtherFeaturesKeyValueId);

			HasRequired(x => x.Property)
			.WithMany(x => x.OtherFeaturesKeyValues)
			.HasForeignKey(x => x.PropertyId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.Key).IsRequired().HasMaxLength(100);
			Property(a => a.Value).IsRequired().HasMaxLength(200);
		}
	}
}
