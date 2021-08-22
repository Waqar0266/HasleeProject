using Hasslefree.Core.Domain.Common;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Common
{
	public class ContentConfiguration : EntityTypeConfiguration<Content>
	{
		public ContentConfiguration()
		{
			// Table
			ToTable("Content");

			// Primary Key
			HasKey(a => a.ContentId);

			// Columns
			Property(a => a.ModifiedOn).IsRequired();

			Property(a => a.Html).IsRequired();
			Property(a => a.ContentTypeEnum).IsRequired().HasMaxLength(100);

			Ignore(a => a.ContentType);
		}
	}
}
