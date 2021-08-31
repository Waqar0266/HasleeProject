using Hasslefree.Core.Domain.Properties;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Properties
{
	public class PropertyPictureConfiguration : EntityTypeConfiguration<PropertyPicture>
	{
		public PropertyPictureConfiguration()
		{
			// Table
			ToTable("PropertyPicture");

			// Primary Key
			HasKey(a => a.PropertyPictureId);

			HasRequired(x => x.Property)
			.WithMany(x => x.Images)
			.HasForeignKey(x => x.PropertyId)
			.WillCascadeOnDelete(false);

			HasRequired(x => x.Picture)
			.WithMany(x => x.PropertyPictures)
			.HasForeignKey(x => x.PictureId)
			.WillCascadeOnDelete(false);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
		}
	}
}
