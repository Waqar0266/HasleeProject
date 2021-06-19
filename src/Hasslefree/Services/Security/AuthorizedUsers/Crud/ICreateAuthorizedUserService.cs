using Hasslefree.Web.Models.Security.AuthorizedUsers;
using Hasslefree.Web.Models.Security.SecurityGroups;
using System.Collections.Generic;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public interface ICreateAuthorizedUserService
	{
		bool HasWarnings { get; }
		List<AuthorizedUserWarning> Warnings { get; }

		ICreateAuthorizedUserService WithModel(AuthorizedUserFullModel baseModel);
		ICreateAuthorizedUserService WithModel(string email, List<SecurityGroupBaseModel> securityGroups);

		bool Create();
	}
}
