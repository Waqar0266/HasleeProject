using Hasslefree.Core.Domain.Security;
using System.Collections.Generic;

namespace Hasslefree.Core.Infrastructure
{
	public interface IPermissionRegistrar
	{
		List<Permission> PermissionList { get; }
	}
}
