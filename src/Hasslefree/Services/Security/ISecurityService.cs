namespace Hasslefree.Services.Security
{
	public interface ISecurityService
	{
		bool IsSystemAdmin(int loginId);
		bool IsInSecurityGroup(int loginId, string securityGroup);
		bool HasPermission(int loginId, string permission);
	}
}
