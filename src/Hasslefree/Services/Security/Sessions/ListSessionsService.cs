using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Security.Sessions.Filters;
using Hasslefree.Web.Models.Security.Sessions.List;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Security.Sessions
{
	public class ListSessionsService : IListSessionsService
	{
		#region Private Properties

		private IDataRepository<Session> SessionRepo { get; }

		#endregion

		#region Fields

		private DateTime? _createdAfter;
		private DateTime? _createdBefore;

		private string _search;
		private FilterBy? _filterBy;
		private SortBy? _sortBy;
		private List<int> _loginIds;
		private List<int> _accountIds;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private IQueryable<Session> _sessions;

		#endregion

		#region Constructor

		public ListSessionsService
		(
			IDataRepository<Session> sessionRepo
		)
		{
			// Repos
			SessionRepo = sessionRepo;
		}

		#endregion

		#region IGetSessionsService

		public IListSessionsService CreatedAfter(DateTime? createdAfter)
		{
			_createdAfter = createdAfter;
			return this;
		}

		public IListSessionsService CreatedBefore(DateTime? createdBefore)
		{
			_createdBefore = createdBefore;
			return this;
		}

		public IListSessionsService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IListSessionsService FilterBy(string filterBy)
		{
			if (!Enum.TryParse(filterBy, true, out FilterBy value)) return this;
			_filterBy = value;
			return this;
		}

		public IListSessionsService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;
			_sortBy = value;
			return this;
		}

		public IListSessionsService WithLogins(List<int> ids)
		{
			_loginIds = ids;
			return this;
		}

		public IListSessionsService WithAccounts(List<int> ids)
		{
			_accountIds = ids;
			return this;
		}

		public IListSessionsService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;
			return this;
		}

		public SessionList List(bool includeDates = true)
		{
			_sessions = SessionRepo.Table;

			FilterCreatedAfter();
			FilterCreatedBefore();
			Search();
			FilterBy();
			SortBy();
			FilterLogins();
			FilterAccounts();

			GetTotalRecords();
			GetPaging();

			return new SessionList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _sessions.Select(s => new SessionListItem
				{
					SessionId = s.SessionId,
					CreatedOn = includeDates ? s.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? s.ModifiedOn : (DateTime?)null,
					Reference = s.Reference,
					IpAddress = s.IpAddress,
					Latitude = s.Latitude,
					Longitude = s.Longitude,
					LoginId = s.LoginId,
					ExpiresOn = s.ExpiresOn
				}).ToList()
			};
		}

		#endregion

		#region Private Methods

		private void FilterCreatedAfter()
		{
			if (_createdAfter == null) return;

			_sessions = _sessions.Where(s => s.CreatedOn >= _createdAfter);
		}

		private void FilterCreatedBefore()
		{
			if (_createdBefore == null) return;

			_sessions = _sessions.Where(s => s.CreatedOn < _createdBefore);
		}

		private void Search()
		{
			if (string.IsNullOrWhiteSpace(_search)) return;

			_sessions = _sessions.Where(s => s.Reference.Contains(_search));
		}

		private void FilterBy()
		{
			switch (_filterBy)
			{
				case Filters.FilterBy.IsAnonymous:
					_sessions = _sessions.Where(s => !s.LoginId.HasValue);
					break;
				case Filters.FilterBy.HasUser:
					_sessions = _sessions.Where(s => s.LoginId.HasValue);
					break;
				case Filters.FilterBy.Expired:
					_sessions = _sessions.Where(s => s.ExpiresOn.HasValue
													 && s.ExpiresOn.Value < DateTime.UtcNow);
					break;
				case Filters.FilterBy.Active:
					_sessions = _sessions.Where(s => !s.ExpiresOn.HasValue
													 || s.ExpiresOn.Value > DateTime.UtcNow);
					break;
			}
		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.IpAddress:
					_sessions = _sessions.OrderBy(s => s.IpAddress);
					break;
				case Filters.SortBy.IpAddressDesc:
					_sessions = _sessions.OrderByDescending(s => s.IpAddress);
					break;
				case Filters.SortBy.Reference:
					_sessions = _sessions.OrderBy(s => s.Reference);
					break;
				case Filters.SortBy.ReferenceDesc:
					_sessions = _sessions.OrderByDescending(s => s.Reference);
					break;
				default:
					_sessions = _sessions.OrderBy(s => s.SessionId);
					break;
			}
		}

		private void FilterLogins()
		{
			if (!_loginIds?.Any() ?? true) return;

			_sessions = _sessions.Where(s => s.LoginId.HasValue && _loginIds.Contains(s.LoginId.Value));
		}

		private void FilterAccounts()
		{
			if (!_accountIds?.Any() ?? true) return;
		}

		private void GetTotalRecords()
		{
			_totalRecords = _sessions.Select(s => s.SessionId).Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_sessions = _sessions.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		#endregion
	}
}
