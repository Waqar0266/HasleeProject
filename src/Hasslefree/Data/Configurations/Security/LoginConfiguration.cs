using Hasslefree.Core.Domain.Security;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Security
{
	public class LoginConfiguration : EntityTypeConfiguration<Login>
	{
		public LoginConfiguration()
		{
			// Table
			ToTable("Login");

			// Primary Key
			HasKey(l => l.LoginId);

			// Foreign Key
			HasRequired(l => l.Person)
				.WithMany(l => l.Logins)
				.HasForeignKey(l => l.PersonId);

			#region Properties

			Property(l => l.PersonId).IsRequired()
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(new IndexAttribute("UIX_Person_PersonId", 1)
					{
						IsUnique = true
					}));
			Property(l => l.CreatedOn).IsRequired();
			Property(l => l.ModifiedOn).IsRequired();
			Property(l => l.Email).IsRequired().HasMaxLength(64)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(new IndexAttribute("UIX_Login_Email", 1)
					{
						IsUnique = true
					}));
			Property(l => l.PasswordSalt).IsOptional().HasMaxLength(32);
			Property(l => l.Password).IsRequired().HasMaxLength(255);
			Property(l => l.Salutation).IsOptional();
			Property(l => l.Active).IsRequired();

			#endregion
		}
	}
}
