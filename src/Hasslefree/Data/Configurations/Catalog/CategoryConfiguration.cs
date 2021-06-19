using Hasslefree.Core.Domain.Catalog;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace Hasslefree.Data.Configurations.Catalog
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class CategoryConfiguration : EntityTypeConfiguration<Category>
	{
		public CategoryConfiguration()
		{
			// Table
			ToTable("Category");

			// Primary Key
			HasKey(a => a.CategoryId);

			// Foreign Keys
			HasOptional(a => a.ParentCategory)
				.WithMany(a => a.SubCategories)
				.HasForeignKey(a => a.ParentCategoryId)
				.WillCascadeOnDelete(false);

			HasOptional(a => a.Picture)
				.WithMany(a => a.Categories)
				.HasForeignKey(a => a.PictureId);

			/* Columns */
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.NestedLevel).IsRequired();
			Property(a => a.DisplayOrder).IsRequired();
			Property(a => a.Path).HasMaxLength(255).IsRequired();
			Property(a => a.Name).HasMaxLength(128).IsRequired();
			Property(a => a.Description).IsOptional();
			Property(a => a.Hidden).IsRequired();
			Property(a => a.Tag).HasMaxLength(1024);

			Property(a => a.SeoId).IsRequired();

			// Constraints & Indexes
			Property(a => a.Path)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(new IndexAttribute("UIX_Category_Path", 1) { IsUnique = true }));

			Property(a => a.NestedLevel)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(new IndexAttribute("IX_Category_Nesting", 1)));

			Property(a => a.DisplayOrder)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(new IndexAttribute("IX_Category_Nesting", 2)));

			Property(a => a.Name)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
				new IndexAnnotation(new IndexAttribute("IX_Category_Name", 1)));
		}
	}
}
