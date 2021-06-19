using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;
using Hasslefree.Core.Domain.Security;

namespace Hasslefree.Data.Configurations.Security
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class SessionConfiguration : EntityTypeConfiguration<Session>
	{
		public SessionConfiguration()
		{
			//Map to table
			ToTable("Session");

			//Define primary key
			HasKey(s => s.SessionId);

			HasOptional(a => a.Login)
				.WithMany(a => a.Sessions)
				.HasForeignKey(a => a.LoginId)
				.WillCascadeOnDelete(false);

			// Column Specifications
			Property(s => s.Reference).HasMaxLength(16).IsRequired();
			Property(s => s.CreatedOn).IsRequired();
			Property(s => s.ModifiedOn).IsRequired();
			Property(s => s.IpAddress).IsRequired().HasMaxLength(16);
			Property(s => s.Latitude).IsRequired();
			Property(s => s.Longitude).IsRequired();
			Property(s => s.ExpiresOn).IsOptional();

			// Indexes & Constraints
			Property(a => a.Reference)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(
						new[]
						{
							new IndexAttribute("UIX_Session_Ref", 1) {IsUnique = true}
						}));
		}
	}
}
