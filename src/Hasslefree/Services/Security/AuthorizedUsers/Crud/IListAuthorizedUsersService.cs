using Hasslefree.Web.Models.Security.AuthorizedUsers;
using Hasslefree.Web.Models.Security.AuthorizedUsers.ListFilters;
using System.Collections.Generic;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public interface IListAuthorizedUsersService
	{
		IListAuthorizedUsersService WithSearch(string search);
		IListAuthorizedUsersService SortBy(AuthorizedUserSortBy sortBy);
		IListAuthorizedUsersService WithPaging(int page = 0, int pageSize = 50);

		List<AuthorizedUserBaseModel> List();
	}
}
