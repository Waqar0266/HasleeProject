using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Security.Login.Filters;
using Hasslefree.Web.Models.Security.Login.List;
using System;
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Login
{
	public class ListLoginsService : IListLoginsService
	{
		#region Private Properties

		private IDataRepository<LoginDb> LoginRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Fields

		private DateTime? _createdAfter;
		private DateTime? _createdBefore;

		private string _search;
		private FilterBy? _filterBy;
		private SortBy? _sortBy;

		private List<int> _securityGroupIds;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private IQueryable<LoginDb> _logins;

		#endregion

		#region Constructor

		public ListLoginsService
		(
			IDataRepository<LoginDb> loginRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			// Repos
			LoginRepo = loginRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IListLoginsService
		public IListLoginsService CreatedAfter(DateTime? createdAfter)
		{
			_createdAfter = createdAfter;
			return this;
		}

		public IListLoginsService CreatedBefore(DateTime? createdBefore)
		{
			_createdBefore = createdBefore;
			return this;
		}

		public IListLoginsService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IListLoginsService FilterBy(string filterBy)
		{
			if (!Enum.TryParse(filterBy, true, out FilterBy value)) return this;
			_filterBy = value;
			return this;
		}

		public IListLoginsService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;
			_sortBy = value;
			return this;
		}

		public IListLoginsService WithSecurityGroupIds(List<int> ids)
		{
			_securityGroupIds = ids;
			return this;
		}

		public IListLoginsService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;
			return this;
		}

		public LoginList List(bool includeDates = true)
		{
			_logins = LoginQuery();

			CreatedAfter();
			CreatedBefore();
			Search();
			SortBy();
			FilterBy();
			FilterSecurityGroups();

			GetTotalRecords();
			GetPaging();

			return new LoginList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _logins.Select(l => new LoginListItem
				{
					LoginId = l.LoginId,
					PersonId = l.PersonId,
					CreatedOn = includeDates ? l.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? l.ModifiedOn : (DateTime?)null,
					Email = l.Email,
					Salutation = l.Salutation,
					Active = l.Active
				}).ToList()
			};
		}

		#endregion

		#region Private Properties

		private IQueryable<LoginDb> LoginQuery()
		{
			var lFuture = LoginRepo.Table.Future();

			// ReSharper disable once UnusedVariable
			var sglFuture = (from l in LoginRepo.Table
							 join sgl in SecurityGroupLoginRepo.Table on l.LoginId equals sgl.LoginId
							 select sgl).Future();

			return lFuture.AsQueryable();
		}

		private void CreatedAfter()
		{
			if (!_createdAfter.HasValue) return;

			_logins = _logins.Where(l => l.CreatedOn >= _createdAfter.Value);
		}

		private void CreatedBefore()
		{
			if (!_createdBefore.HasValue) return;

			_logins = _logins.Where(l => l.CreatedOn < _createdBefore.Value);
		}

		private void Search()
		{
			if (string.IsNullOrWhiteSpace(_search)) return;

			_logins = _logins.Where(l => l.Email.Contains(_search));
		}

		private void FilterBy()
		{
			switch (_filterBy)
			{
				case Filters.FilterBy.Active:
					_logins = _logins.Where(l => l.Active);
					break;
				case Filters.FilterBy.Inactive:
					_logins = _logins.Where(l => !l.Active);
					break;
				case Filters.FilterBy.HasAdminPrivilege:
					_logins = _logins.Where(l => l.SecurityGroupLogins.Any());
					break;
				case Filters.FilterBy.IsCustomer:
					_logins = _logins.Where(l => !l.SecurityGroupLogins.Any());
					break;
			}
		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.Email:
					_logins = _logins.OrderBy(l => l.Email);
					break;
				case Filters.SortBy.EmailDesc:
					_logins = _logins.OrderByDescending(l => l.Email);
					break;
				case Filters.SortBy.TotalSecurityGroups:
					_logins = _logins.OrderBy(l => l.SecurityGroupLogins.Count);
					break;
				case Filters.SortBy.TotalSecurityGroupsDesc:
					_logins = _logins.OrderByDescending(l => l.SecurityGroupLogins.Count);
					break;
				default:
					_logins = _logins.OrderBy(l => l.LoginId);
					break;
			}
		}

		private void FilterSecurityGroups()
		{
			if (!_securityGroupIds?.Any() ?? true) return;

			_logins = _logins.Where(l => l.SecurityGroupLogins.Any(sgl => _securityGroupIds.Contains(sgl.SecurityGroupId)));
		}

		private void GetTotalRecords()
		{
			_totalRecords = _logins.Select(l => l.LoginId).Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_logins = _logins.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		#endregion
	}
}
