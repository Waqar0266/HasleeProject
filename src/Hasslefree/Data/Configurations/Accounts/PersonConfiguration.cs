using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;
using Hasslefree.Core.Domain.Accounts;

namespace Hasslefree.Data.Configurations.Accounts
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class PersonConfiguration : EntityTypeConfiguration<Person>
	{
		public PersonConfiguration()
		{
			// Table
			ToTable("Person");

			// Primary Key
			HasKey(a => a.PersonId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.PersonGuid).IsRequired();
			Property(a => a.FirstName).HasMaxLength(32);
			Property(a => a.Surname).HasMaxLength(32);
			Property(a => a.Title).HasMaxLength(16);
			Property(a => a.Phone).HasMaxLength(16);
			Property(a => a.Fax).HasMaxLength(16);
			Property(a => a.Mobile).HasMaxLength(16);
			Property(a => a.Email).IsRequired().HasMaxLength(64);
			Property(a => a.GenderEnum).IsRequired().HasMaxLength(16);
			Property(a => a.PersonStatusEnum).IsRequired().HasMaxLength(16);
			Property(a => a.IdNumber).IsOptional().HasMaxLength(32);

			// Ignore
			Ignore(a => a.Gender);
			Ignore(a => a.PersonStatus);

			// Constraints & Indexes
			Property(a => a.PersonGuid).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("UIX_Person_PersonGuid", 1) { IsUnique = true }));
			Property(a => a.Email).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("UIX_Person_Email", 1) { IsUnique = true }));
		}
	}
}