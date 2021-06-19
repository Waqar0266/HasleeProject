using System.Collections.Generic;
using Hasslefree.Web.Models.Security.AuthorizedUsers;

namespace Hasslefree.Services.Security.AuthorizedUsers.Search
{
	public interface ISearchUsersService
	{
		ISearchUsersService WithTerm(string search);

		List<SearchUserModel> Search();
	}
}
