using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.People.Implementations.Filters;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Web.Models.People.List;
using System;
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.People.Implementations
{
	public class ListPersonsService : IListPersonsService
	{
		#region Private Properties

		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Fields

		private bool _includeDates;

		private DateTime? _createdAfter;
		private DateTime? _createdBefore;

		private string _search;
		private FilterBy? _filterBy;
		private SortBy? _sortBy;
		private List<int> _accountIds;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private IQueryable<Person> _people;

		#endregion

		#region Constructor

		public ListPersonsService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<Person> personRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			LoginRepo = loginRepo;
			PersonRepo = personRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IListPersonsService

		public IListPersonsService IncludeDates()
		{
			_includeDates = true;

			return this;
		}

		public IListPersonsService CreatedAfter(DateTime? createdAfter)
		{
			_createdAfter = createdAfter;

			return this;
		}

		public IListPersonsService CreatedBefore(DateTime? createdBefore)
		{
			_createdBefore = createdBefore;

			return this;
		}

		public IListPersonsService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IListPersonsService FilterBy(string filterBy)
		{
			if (!Enum.TryParse(filterBy, true, out FilterBy value)) return this;

			_filterBy = value;

			return this;
		}

		public IListPersonsService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;

			_sortBy = value;

			return this;
		}

		public IListPersonsService WithAccountIds(List<int> ids)
		{
			_accountIds = ids;

			return this;
		}

		public IListPersonsService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;

			return this;
		}

		public PersonList List()
		{
			_people = PeopleQuery();

			FilterCreatedAfter();
			FilterCreatedBefore();
			FilterSearch();
			FilterBy();
			SortBy();
			FilterAccounts();

			GetTotalRecords();
			GetPaging();

			return new PersonList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _people.AsEnumerable().Select(p => new PersonListItem
				{
					PersonId = p.PersonId,
					LoginId = p.Logins.FirstOrDefault()?.LoginId,
					CreatedOn = _includeDates ? p.CreatedOn : (DateTime?)null,
					ModifiedOn = _includeDates ? p.ModifiedOn : (DateTime?)null,
					PersonGuid = p.PersonGuid,
					Surname = p.Surname,
					Initials = p.FirstName[0] + "",
					FirstName = p.FirstName,
					Email = p.Email,
					PersonStatus = p.PersonStatus,
					HasAdminPrivileges = p.Logins.Any(l => l.SecurityGroupLogins.Any())
				}).ToList()
			};
		}

		#endregion

		#region Private Methods

		private IQueryable<Person> PeopleQuery()
		{
			var peopleFuture = PersonRepo.Table.Future();

			return peopleFuture.AsQueryable();
		}

		private void FilterCreatedAfter()
		{
			if (!_createdAfter.HasValue) return;

			_people = _people.Where(p => p.CreatedOn >= _createdAfter.Value);
		}

		private void FilterCreatedBefore()
		{
			if (!_createdBefore.HasValue) return;

			_people = _people.Where(p => p.CreatedOn < _createdBefore.Value);
		}

		private void FilterAccounts()
		{
			if (!_accountIds?.Any() ?? true) return;
		}

		private void FilterSearch()
		{
			if (string.IsNullOrWhiteSpace(_search)) return;

			_people = _people.Where(a => a.FirstName.Contains(_search)
										 || a.Surname.Contains(_search)
										 || a.Email.Contains(_search));
		}

		private void FilterBy()
		{
			switch (_filterBy)
			{
				case Filters.FilterBy.Customer:
					_people = _people.Where(p => p.Logins.Any(l => !l.SecurityGroupLogins.Any()));
					break;
				case Filters.FilterBy.HasAdminPrivileges:
					_people = _people.Where(p => p.Logins.Any(l => l.SecurityGroupLogins.Any()));
					break;
			}
		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.Email:
					_people = _people.OrderBy(p => p.Email);
					break;
				case Filters.SortBy.EmailDesc:
					_people = _people.OrderByDescending(p => p.Email);
					break;
				case Filters.SortBy.Name:
					_people = _people.OrderBy(p => p.FirstName);
					break;
				case Filters.SortBy.NameDesc:
					_people = _people.OrderByDescending(p => p.FirstName);
					break;
				case Filters.SortBy.Surname:
					_people = _people.OrderBy(p => p.Surname);
					break;
				case Filters.SortBy.SurnameDesc:
					_people = _people.OrderByDescending(p => p.Surname);
					break;
				default:
					_people = _people.OrderBy(p => p.PersonId);
					break;
			}
		}

		private void GetTotalRecords()
		{
			_totalRecords = _people.Select(p => p.PersonId).Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_people = _people.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		#endregion
	}
}
