using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyConfiguration : EntityTypeConfiguration<Property>
	{
		public PropertyConfiguration()
		{
			// Table
			ToTable("Property");

			// Primary Key
			HasKey(a => a.PropertyId);

			HasRequired(x => x.Category)
			.WithMany()
			.HasForeignKey(x => x.CategoryId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.PrivatePropertyId).IsRequired().HasMaxLength(30);
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.Address).IsOptional().HasMaxLength(255);
			Property(a => a.Description).IsRequired().HasMaxLength(4500);
			Property(a => a.Price).IsRequired().HasPrecision(15, 5);
			Property(a => a.Title).IsRequired().HasMaxLength(200);
			Property(a => a.PropertyTypeEnum).IsRequired().HasMaxLength(20);

			Ignore(a => a.PropertyType);
		}
	}
}
