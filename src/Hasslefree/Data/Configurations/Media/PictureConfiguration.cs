using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Data.Configurations.Media
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class PictureConfiguration : EntityTypeConfiguration<Picture>
	{
		public PictureConfiguration()
		{
			// Table
			ToTable("Picture");

			// Primary Key
			HasKey(a => a.PictureId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.FormatEnum).HasMaxLength(32).IsRequired();
			Property(a => a.Name).HasMaxLength(255).IsRequired();
			Property(a => a.Folder).HasMaxLength(255);
			Property(a => a.Path).HasMaxLength(255);
			Property(a => a.Transforms).HasMaxLength(255);
			Property(a => a.AltText).HasMaxLength(255);
			Property(a => a.MimeType).HasMaxLength(50);

			// Ignore
			Ignore(a => a.Format);

			// Indexes & Constraints
			Property(a => a.Path)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(
						new[]
						{
							new IndexAttribute("UIX_Picture_Path", 1) {IsUnique = true}
						}));
		}
	}
}