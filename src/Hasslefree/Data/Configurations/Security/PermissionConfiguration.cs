using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using Hasslefree.Core.Domain.Security;
using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace Hasslefree.Data.Configurations.Security
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class PermissionConfiguration : EntityTypeConfiguration<Permission>
	{
		public PermissionConfiguration()
		{
			// Table
			ToTable("Permission");
			
			#region Properties

			Property(p => p.CreatedOn).IsRequired();
			Property(p => p.ModifiedOn).IsRequired();
			Property(p => p.PermissionUniqueName).IsRequired().HasMaxLength(255)
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(new IndexAttribute("UIX_SecurityGroup_PermissionUniqueName", 1)
					{
						IsUnique = true
					}));
			Property(p => p.PermissionDisplayName).IsRequired();
			Property(p => p.PermissionDescription).IsOptional();

			#endregion
		}
	}
}

