using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace Hasslefree.Data.Configurations.Common
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class CountryConfiguration : EntityTypeConfiguration<Core.Domain.Common.Country>
	{
		public CountryConfiguration()
		{
			// Table
			ToTable("Country");

			// Primary Key
			HasKey(a => a.CountryId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.Name).IsRequired().HasMaxLength(100);
			Property(a => a.AllowsBilling).IsRequired();
			Property(a => a.AllowsShipping).IsRequired();
			Property(a => a.TwoLetterIsoCode).HasMaxLength(2);
			Property(a => a.ThreeLetterIsoCode).HasMaxLength(3);
			Property(a => a.NumericIsoCode).IsRequired();
			Property(a => a.SubjectToVat).IsRequired();
			Property(a => a.Published).IsRequired();
			Property(a => a.DisplayOrder).IsRequired();

			// Constraints & Indexes
            Property(a => a.Name)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName, 
				new IndexAnnotation(
					new IndexAttribute("UIX_Country_Name") { IsUnique = true }));

            Property(a => a.TwoLetterIsoCode)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName, 
				new IndexAnnotation(
					new IndexAttribute("UIX_Country_TwoLetterIsoCode") { IsUnique = true }));

            Property(a => a.ThreeLetterIsoCode)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName, 
				new IndexAnnotation(
					new IndexAttribute("UIX_Country_ThreeLetterIsoCode") { IsUnique = true }));
		}
	}
}
