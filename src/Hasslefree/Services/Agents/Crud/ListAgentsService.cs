using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Agents.Crud.Filters;
using Hasslefree.Web.Models.Agents;
using System;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Agents.Crud
{
	public class ListAgentsService : IListAgentsService, IInstancePerRequest
	{
		#region Private Properties

		private IReadOnlyRepository<Agent> AgentRepo { get; }

		#endregion

		#region Fields

		private DateTime? _createdAfter;
		private DateTime? _createdBefore;

		private string _search;
		private FilterBy? _filterBy;
		private SortBy? _sortBy;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private IQueryable<Agent> _agents;

		#endregion

		#region Constructor

		public ListAgentsService
		(
			IReadOnlyRepository<Agent> agentRepo
		)
		{
			AgentRepo = agentRepo;
		}

		#endregion

		#region IListAgentsService

		public IListAgentsService CreatedBefore(DateTime? createdBefore)
		{
			_createdBefore = createdBefore;
			return this;
		}

		public IListAgentsService CreatedAfter(DateTime? createdAfter)
		{
			_createdAfter = createdAfter;
			return this;
		}

		public IListAgentsService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IListAgentsService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;
			_sortBy = value;
			return this;
		}

		public IListAgentsService FilterBy(string filterBy)
		{
			if (!Enum.TryParse(filterBy, true, out FilterBy value)) return this;
			_filterBy = value;
			return this;
		}

		public IListAgentsService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;

			return this;
		}

		public AgentList List()
		{
			_agents = AgentQuery();

			FilterCreatedBefore();
			FilterCreatedAfter();
			FilterSearch();
			FilterBy();
			SortBy();

			GetTotalRecords();
			GetPaging();

			return new AgentList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _agents.AsEnumerable().Select(c => new AgentListItem
				{
					AgentId = c.AgentId,
					Name = c.Person == null ? GetTempData(c.TempData).Split(';')[1] : c.Person.FirstName,
					Status = c.AgentStatusEnum,
					Type = c.AgentTypeEnum,
					Surname = c.Person == null ? GetTempData(c.TempData).Split(';')[2] : c.Person.Surname,
					Title = c.Person == null ? GetTempData(c.TempData).Split(';')[0] : c.Person.Title
				}).ToList()
			};
		}

		#endregion

		#region Private Methods

		private IQueryable<Agent> AgentQuery()
		{
			var cFuture = (from c in AgentRepo.Table.Include(a => a.Person) select c).Future();
			return cFuture.AsQueryable();
		}

		private void FilterSearch()
		{
			if (IsNullOrWhiteSpace(_search)) return;

			string searchQuery = _search.ToLower().Trim();

			_agents = _agents.Where(c => AgentSearchHelper(c.Person.FirstName).Contains(searchQuery));
		}

		private string AgentSearchHelper(string property) => property?.Replace("/", "").ToLower();

		private void FilterBy()
		{

		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.Created:
					_agents = _agents.OrderBy(c => c.CreatedOn);
					break;
				case Filters.SortBy.CreatedDesc:
					_agents = _agents.OrderByDescending(c => c.CreatedOn);
					break;
				case Filters.SortBy.Name:
					_agents = _agents.OrderBy(c => c.Person.FirstName);
					break;
				case Filters.SortBy.NameDesc:
					_agents = _agents.OrderByDescending(c => c.Person.FirstName);
					break;

				default:
					_agents = _agents.OrderBy(c => c.AgentId);
					break;
			}
		}

		private void FilterCreatedAfter()
		{
			if (!_createdAfter.HasValue) return;

			_agents = _agents.Where(a => a.CreatedOn >= _createdAfter.Value);
		}

		private void FilterCreatedBefore()
		{
			if (!_createdBefore.HasValue) return;

			_agents = _agents.Where(a => a.CreatedOn < _createdBefore.Value);
		}

		private void GetTotalRecords()
		{
			_totalRecords = _agents.Select(c => c.AgentId).Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_agents = _agents.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		private string GetTempData(string tempData)
		{
			return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(tempData));
		}

		#endregion
	}
}
