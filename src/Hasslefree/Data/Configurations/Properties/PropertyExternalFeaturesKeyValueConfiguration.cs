using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyExternalFeaturesKeyValueConfiguration : EntityTypeConfiguration<PropertyExternalFeaturesKeyValue>
	{
		public PropertyExternalFeaturesKeyValueConfiguration()
		{
			// Table
			ToTable("PropertyExternalFeaturesKeyValue");

			// Primary Key
			HasKey(a => a.PropertyExternalFeaturesKeyValueId);

			HasRequired(x => x.Property)
			.WithMany(x => x.ExternalFeaturesKeyValues)
			.HasForeignKey(x => x.PropertyId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.Key).IsRequired().HasMaxLength(100);
			Property(a => a.Value).IsRequired().HasMaxLength(200);
		}
	}
}
