using System.Data.Entity.ModelConfiguration;
using Hasslefree.Core.Domain.Security;

namespace Hasslefree.Data.Configurations.Security
{
	public class SecurityGroupLoginConfiguration : EntityTypeConfiguration<SecurityGroupLogin>
	{
		public SecurityGroupLoginConfiguration()
		{
			ToTable("SecurityGroupLogin");

			HasKey(sgl => new
			{
				sgl.LoginId,
				sgl.SecurityGroupId
			});

			HasRequired(sgl => sgl.Login)
				.WithMany(l => l.SecurityGroupLogins)
				.HasForeignKey(l => l.LoginId)
				.WillCascadeOnDelete(true);

			HasRequired(sgl => sgl.SecurityGroup)
				.WithMany(l => l.SecurityGroupLogins)
				.HasForeignKey(l => l.SecurityGroupId)
				.WillCascadeOnDelete(true);
		}
	}
}
