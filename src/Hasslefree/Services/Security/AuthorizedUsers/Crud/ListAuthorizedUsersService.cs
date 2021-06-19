using Hasslefree.Data;
using Hasslefree.Web.Models.Security.AuthorizedUsers;
using Hasslefree.Web.Models.Security.AuthorizedUsers.ListFilters;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static System.String;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public class ListAuthorizedUsersService : IListAuthorizedUsersService
	{
		#region Private Properties

		private IDataRepository<LoginDb> LoginRepo { get; }

		#endregion

		#region Fields
		
		private string _search;
		private AuthorizedUserSortBy _sortBy;
		private int _page;
		private int _pageSize;
		private IQueryable<LoginDb> _authorizedUsers;

		#endregion

		#region Constructor

		public ListAuthorizedUsersService
		(
			IDataRepository<LoginDb> loginRepo
		)
		{
			LoginRepo = loginRepo;
		}

		#endregion

		#region IListAuthorizedUsersService

		public IListAuthorizedUsersService WithSearch(string search)
		{
			_search = search;

			return this;
		}

		public IListAuthorizedUsersService SortBy(AuthorizedUserSortBy sortBy)
		{
			_sortBy = sortBy;

			return this;
		}

		public IListAuthorizedUsersService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;

			return this;
		}

		public List<AuthorizedUserBaseModel> List()
		{
			_authorizedUsers = LoginRepo.Table.Include(l => l.Person)
										.Include(l => l.SecurityGroupLogins.Select(sgl => sgl.SecurityGroup))
										.Include(l => l.SecurityGroupLogins.Select(sgl => sgl.SecurityGroup.Permissions))
										.Where(l => l.SecurityGroupLogins.Count > 0);

			FilterAuthorizedUsers();
			SortSecurityGroups();
			GetPaging();

			return _authorizedUsers.Select(l => new AuthorizedUserBaseModel
			{
				LoginId = l.LoginId,
				Email = l.Email,
				FullName = l.Person.FirstName + " " + l.Person.Surname,
				Active = l.Active,
				TotalSecurityGroups = l.SecurityGroupLogins.Count,
			}).ToList();
		}

		#endregion

		#region Private Methods

		private void FilterAuthorizedUsers()
		{
			if (!IsNullOrWhiteSpace(_search))
				_authorizedUsers = _authorizedUsers.Where(l => l.Person.Email.Contains(_search) ||
															   l.Person.FirstName.Contains(_search) ||
															   l.Person.Surname.Contains(_search));
		}

		private void SortSecurityGroups()
		{
			switch (_sortBy)
			{
				case AuthorizedUserSortBy.Name:
					_authorizedUsers = _authorizedUsers.OrderBy(l => l.Person.FirstName + " " + l.Person.Surname);
					break;
				case AuthorizedUserSortBy.Email:
					_authorizedUsers = _authorizedUsers.OrderBy(l => l.Email);
					break;
				case AuthorizedUserSortBy.SecurityGroupCountAsc:
					_authorizedUsers = _authorizedUsers.OrderBy(l => l.SecurityGroupLogins.Count);
					break;
				case AuthorizedUserSortBy.SecurityGroupCountDesc:
					_authorizedUsers = _authorizedUsers.OrderByDescending(l => l.SecurityGroupLogins.Count);
					break;
				default:
					_authorizedUsers = _authorizedUsers.OrderBy(l => l.LoginId);
					break;
			}
		}

		private void GetPaging()
		{
			_authorizedUsers = _authorizedUsers.Skip(_page * _pageSize).Take(_pageSize);
		}

		#endregion
	}
}
