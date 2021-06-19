using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Security.Groups.Filters;
using Hasslefree.Web.Models.Security.SecurityGroups.List;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Security.Groups
{
	public class ListSecurityGroupsService : IListSecurityGroupsService
	{
		#region Private Properties
		
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Fields

		private DateTime? _createdBefore;
		private DateTime? _createdAfter;

		private string _search;
		private FilterBy? _filterBy;
		private SortBy? _sortBy;

		private List<int> _permissionIds;
		private List<int> _loginIds;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private IQueryable<SecurityGroup> _groups;

		#endregion

		#region Constructor

		public ListSecurityGroupsService
		(
			IDataRepository<SecurityGroup> securityGroupRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			// Repos
			SecurityGroupRepo = securityGroupRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IListSecurityGroupsService

		public IListSecurityGroupsService CreatedAfter(DateTime? createdAfter)
		{
			_createdAfter = createdAfter;
			return this;
		}

		public IListSecurityGroupsService CreatedBefore(DateTime? createdBefore)
		{
			_createdBefore = createdBefore;
			return this;
		}

		public IListSecurityGroupsService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IListSecurityGroupsService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;
			_sortBy = value;
			return this;
		}

		public IListSecurityGroupsService FilterBy(string filterBy)
		{
			if (!Enum.TryParse(filterBy, true, out FilterBy value)) return this;
			_filterBy = value;
			return this;
		}

		public IListSecurityGroupsService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;
			return this;
		}

		public IListSecurityGroupsService WithPermissionIds(List<int> ids)
		{
			_permissionIds = ids;
			return this;
		}

		public IListSecurityGroupsService WithLoginIds(List<int> ids)
		{
			_loginIds = ids;
			return this;
		}

		#endregion

		public SecurityGroupList List(bool includeDates = true)
		{
			_groups = SecurityGroupQuery();

			FilterCreatedBefore();
			FilterCreatedAfter();

			Search();
			FilterBy();
			SortBy();

			FilterUsers();
			FilterPermissions();

			GetTotalRecords();
			GetPaging();

			return new SecurityGroupList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _groups.Select(sg => new SecurityGroupListItem
				{
					SecurityGroupId = sg.SecurityGroupId,
					CreatedOn = includeDates ? sg.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? sg.ModifiedOn : (DateTime?)null,
					Name = sg.SecurityGroupName,
					Description = sg.SecurityGroupDesc,
					IsSystemSecurityGroup = sg.IsSystemSecurityGroup,
					TotalUsers = sg.SecurityGroupLogins.Count,
					TotalPermissions = sg.Permissions.Count
				}).ToList()
			};
		}

		#region Private Methods

		private IQueryable<SecurityGroup> SecurityGroupQuery()
		{
			var sgFuture = (from sg in SecurityGroupRepo.Table.Include(sg => sg.Permissions) select sg).Future();

			// ReSharper disable UnusedVariable
			var sglFuture = (from sg in SecurityGroupRepo.Table
							 join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
							 select sgl).Future();
			// ReSharper enable UnusedVariable

			return sgFuture.AsQueryable();
		}

		private void FilterCreatedAfter()
		{
			if (!_createdAfter.HasValue) return;

			_groups = _groups.Where(sg => sg.CreatedOn >= _createdAfter.Value);
		}

		private void FilterCreatedBefore()
		{
			if (!_createdBefore.HasValue) return;

			_groups = _groups.Where(sg => sg.CreatedOn < _createdBefore.Value);
		}

		private void Search()
		{
			if (String.IsNullOrWhiteSpace(_search)) return;

			_groups = _groups.Where(g => g.SecurityGroupName.Contains(_search)
										 || g.SecurityGroupDesc.Contains(_search));
		}

		private void FilterBy()
		{
			switch (_filterBy)
			{
				case Filters.FilterBy.IsSystemGroup:
					_groups = _groups.Where(g => g.IsSystemSecurityGroup);
					break;
				case Filters.FilterBy.IsNotSystemGroup:
					_groups = _groups.Where(g => !g.IsSystemSecurityGroup);
					break;
			}
		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.Name:
					_groups = _groups.OrderBy(g => g.SecurityGroupName);
					break;
				case Filters.SortBy.NameDesc:
					_groups = _groups.OrderByDescending(g => g.SecurityGroupName);
					break;
				case Filters.SortBy.IsSystemGroup:
					_groups = _groups.OrderBy(g => g.IsSystemSecurityGroup);
					break;
				case Filters.SortBy.IsNotSystemGroup:
					_groups = _groups.OrderBy(g => !g.IsSystemSecurityGroup);
					break;
				case Filters.SortBy.Members:
					_groups = _groups.OrderBy(g => g.SecurityGroupLogins.Count);
					break;
				case Filters.SortBy.MembersDesc:
					_groups = _groups.OrderByDescending(g => g.SecurityGroupLogins.Count);
					break;
				case Filters.SortBy.Permissions:
					_groups = _groups.OrderBy(g => g.Permissions.Count);
					break;
				case Filters.SortBy.PermissionsDesc:
					_groups = _groups.OrderByDescending(g => g.Permissions.Count);
					break;
				default:
					_groups = _groups.OrderBy(sg => sg.SecurityGroupId);
					break;
			}
		}

		private void FilterPermissions()
		{
			if (!_permissionIds?.Any() ?? true) return;

			_groups = _groups.Where(sg => sg.Permissions.Any(p => _permissionIds.Contains(p.PermissionId)));
		}

		private void FilterUsers()
		{
			if (!_loginIds?.Any() ?? true) return;

			_groups = _groups.Where(sg => sg.SecurityGroupLogins.Any(sgl => _loginIds.Contains(sgl.LoginId)));
		}

		private void GetTotalRecords()
		{
			_totalRecords = _groups.Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_groups = _groups.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		#endregion
	}
}
