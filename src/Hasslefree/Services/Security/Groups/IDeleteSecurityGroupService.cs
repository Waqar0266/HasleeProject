using System.Collections.Generic;

namespace Hasslefree.Services.Security.Groups
{
	public interface IDeleteSecurityGroupService
	{
		bool HasWarnings { get; }
		List<SecurityGroupWarning> Warnings { get; }

		IDeleteSecurityGroupService this[int securityGroupId] { get; }
		IDeleteSecurityGroupService this[List<int> securityGroupIds] { get; }

		bool Remove(bool saveChanges = true);
	}
}
