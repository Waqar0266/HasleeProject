using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;
using Hasslefree.Core.Domain.Emails;

namespace Hasslefree.Data.Configurations.Emails
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class EmailConfiguration : EntityTypeConfiguration<Email>
	{
		public EmailConfiguration()
		{
			// Table
			ToTable("Email");

			// Primary Key
			HasKey(a => a.EmailId);

			// Columns
			Property(a => a.CreatedOn).IsRequired();
			Property(a => a.ModifiedOn).IsRequired();
			Property(a => a.Type).IsRequired().HasMaxLength(64);
			Property(a => a.Send).IsRequired();
			Property(a => a.SendTypeEnum).IsRequired().HasMaxLength(12);
			Property(a => a.From).IsOptional().HasMaxLength(64);
			Property(a => a.Subject).IsOptional().HasMaxLength(64);
			Property(a => a.Url).IsOptional().HasMaxLength(512);
			Property(a => a.Recipient).IsOptional().HasMaxLength(512);
			Property(a => a.Template).IsOptional();

			// Ignores
			Ignore(a => a.SendType);

			// Indexes
			Property(a => a.Type).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("UIX_Email_StoreId_Type", 2) { IsUnique = true }));
		}
	}
}
