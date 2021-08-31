using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyBuildingKeyValueConfiguration : EntityTypeConfiguration<PropertyBuildingKeyValue>
	{
		public PropertyBuildingKeyValueConfiguration()
		{
			// Table
			ToTable("PropertyBuildingKeyValue");

			// Primary Key
			HasKey(a => a.PropertyBuildingKeyValueId);

			HasRequired(x => x.Property)
			.WithMany(x => x.BuildingKeyValues)
			.HasForeignKey(x => x.PropertyId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.Key).IsRequired().HasMaxLength(100);
			Property(a => a.Value).IsRequired().HasMaxLength(200);
		}
	}
}
