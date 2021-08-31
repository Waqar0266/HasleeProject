using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyRoomKeyValueConfiguration : EntityTypeConfiguration<PropertyRoomKeyValue>
	{
		public PropertyRoomKeyValueConfiguration()
		{
			// Table
			ToTable("PropertyRoomKeyValue");

			// Primary Key
			HasKey(a => a.PropertyRoomKeyValueId);

			HasRequired(x => x.Property)
			.WithMany(x => x.RoomKeyValues)
			.HasForeignKey(x => x.PropertyId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.Key).IsRequired().HasMaxLength(100);
			Property(a => a.Value).IsRequired().HasMaxLength(200);
		}
	}
}
