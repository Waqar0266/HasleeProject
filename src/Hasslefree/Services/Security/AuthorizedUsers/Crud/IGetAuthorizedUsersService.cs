using Hasslefree.Web.Models.Security.AuthorizedUsers;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public interface IGetAuthorizedUsersService
	{
		IGetAuthorizedUsersService WithId(int authorizedUserId);
		AuthorizedUserFullModel Get();
	}
}
