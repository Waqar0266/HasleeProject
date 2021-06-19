using System.Collections.Generic;
using Hasslefree.Web.Models.Security.SecurityGroups;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public interface IUpdateAuthorizedUserService
	{
		bool HasWarnings { get; }
		List<AuthorizedUserWarning> Warnings { get; }

		IUpdateAuthorizedUserService WithId(int loginId);
		IUpdateAuthorizedUserService WithSecurityGroups(List<SecurityGroupBaseModel> securityGroups);

		bool Edit();
	}
}
