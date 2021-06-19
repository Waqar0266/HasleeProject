using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hasslefree.Core.Domain.Security;

namespace Hasslefree.Services.Security.Groups
{
	public interface IUpdateSecurityGroupService
	{
		bool HasWarnings { get; }
		List<SecurityGroupWarning> Warnings { get; }

		IUpdateSecurityGroupService this[int securityGroupId] { get; }
		IUpdateSecurityGroupService WithSecurityGroupId(int securityGroupId);

		IUpdateSecurityGroupService Set<T>(Expression<Func<SecurityGroup, T>> lambda, object value);

		IUpdateSecurityGroupService WithPermission(int permissionId);
		IUpdateSecurityGroupService RemovePermission(int permissionId);

		IUpdateSecurityGroupService WithUser(int loginId);
		IUpdateSecurityGroupService RemoveUser(int loginId);

		bool Update(bool saveChanges = true);
	}
}
