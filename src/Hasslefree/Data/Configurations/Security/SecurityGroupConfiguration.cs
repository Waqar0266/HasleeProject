using Hasslefree.Core.Domain.Security;
using System.Data.Entity.ModelConfiguration;

namespace Hasslefree.Data.Configurations.Security
{
	public class SecurityGroupConfiguration : EntityTypeConfiguration<SecurityGroup>
	{
		public SecurityGroupConfiguration()
		{
			// Table
			ToTable("SecurityGroup");

			// Primary Key
			HasKey(sg => sg.SecurityGroupId);

			#region Foreign Keys

			HasMany(sg => sg.Permissions)
				.WithMany(p => p.SecurityGroups)
				.Map(sgp =>
				{
					sgp.MapLeftKey("SecurityGroupId");
					sgp.MapRightKey("PermissionId");
					sgp.ToTable("SecurityGroupPermissions");
				});

			#endregion

			#region Properties

			Property(sg => sg.CreatedOn).IsRequired();
			Property(sg => sg.ModifiedOn).IsRequired();
			Property(sg => sg.SecurityGroupName).IsRequired();
			Property(sg => sg.IsSystemSecurityGroup).IsRequired();

			#endregion
		}
	}
}
