using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Data.Configurations.Media
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class DownloadConfiguration : EntityTypeConfiguration<Download>
	{
		public DownloadConfiguration()
		{
			// Table
			ToTable("Download");

			// Primary Key
			HasKey(a => a.DownloadId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.FileName).IsRequired().HasMaxLength(255);
			Property(a => a.Extension).IsRequired().HasMaxLength(8);
			Property(a => a.MediaStorageEnum).IsRequired().HasMaxLength(16);
			Property(a => a.RelativeFolderPath).HasMaxLength(255);
			Property(a => a.ContentType).IsRequired().HasMaxLength(255);
			Property(a => a.Size).IsRequired();
            Property(a => a.DownloadTypeEnum).IsRequired().HasMaxLength(10);

			// Ignores
			Ignore(a => a.MediaStorage);
            Ignore(a => a.DownloadType);
			
		}
	}
}


