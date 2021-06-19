using Hasslefree.Core.Domain.Security;
using System.Collections.Generic;

namespace Hasslefree.Core
{
	public interface IUserContext
	{
		Login Login { get; }
		List<SecurityGroup> SecurityGroups { get; }
		List<Permission> Permissions { get; }
		bool IsSystemAdmin { get; }
	}
}
