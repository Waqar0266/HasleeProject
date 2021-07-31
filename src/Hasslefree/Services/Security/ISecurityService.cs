using System.Collections.Generic;

namespace Hasslefree.Services.Security
{
	public interface ISecurityService
	{
		bool IsSystemAdmin(int loginId);
		bool IsInSecurityGroup(int loginId, List<string> securityGroups);
		bool HasPermission(int loginId, string permission);
	}
}
