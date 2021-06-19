using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hasslefree.Data;
using Hasslefree.Web.Models.Security.AuthorizedUsers;

namespace Hasslefree.Services.Security.AuthorizedUsers.Search
{
	public class SearchUsersService : ISearchUsersService
	{
		#region Private Properties

		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }

		#endregion

		#region Fields

		private string _search;

		#endregion

		#region Constructor

		public SearchUsersService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo
		)
		{
			LoginRepo = loginRepo;
		}

		#endregion

		#region ISearchUsersService

		public ISearchUsersService WithTerm(string search)
		{
			_search = search;

			return this;
		}

		public List<SearchUserModel> Search()
		{
			var users = LoginRepo.Table
									.Include(l => l.Person)
									.Where(l => l.LoginId > 0 &&
												(l.Email.Contains(_search) ||
												 l.Person.FirstName.Contains(_search) ||
												 l.Person.Surname.Contains(_search)));

			return users.Select(l => new SearchUserModel
			{
				LoginId = l.LoginId,
				Email = l.Email,
				FullName = l.Person.FirstName + " " + l.Person.Surname
			}).ToList();
		}

		#endregion
	}
}
