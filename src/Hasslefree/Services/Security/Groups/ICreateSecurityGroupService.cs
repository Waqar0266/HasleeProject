using System.Collections.Generic;

namespace Hasslefree.Services.Security.Groups
{
	public interface ICreateSecurityGroupService
	{
		bool HasWarnings { get; }
		List<SecurityGroupWarning> Warnings { get; }
		int SecurityGroupId { get; }

		ICreateSecurityGroupService New(string name, string description);

		ICreateSecurityGroupService WithUser(int loginId);
		ICreateSecurityGroupService WithUsers(IEnumerable<int> loginIds);

		ICreateSecurityGroupService WithPermission(int permissionId);
		ICreateSecurityGroupService WithPermissions(IEnumerable<int> permissionIds);

		bool Create(bool saveChanges = true);
	}
}
