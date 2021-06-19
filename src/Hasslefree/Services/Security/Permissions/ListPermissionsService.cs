using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Services.Security.Permissions.Filters;
using Hasslefree.Web.Models.Security.Permissions.List;
using System;
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Security.Permissions
{
	public class ListPermissionsService : IListPermissionsService
	{
		#region Private Properties

		private IDataRepository<Permission> PermissionRepo { get; }
		private IDataRepository<SecurityGroup> SecurityGroupRepo { get; }
		private IDataRepository<SecurityGroupLogin> SecurityGroupLoginRepo { get; }

		#endregion

		#region Fields

		private IQueryable<Permission> _permissions;

		private string _search;

		private SortBy? _sortBy;
		private List<int> _securityGroupIds;
		private List<int> _loginIds;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		#endregion

		#region Constructor

		public ListPermissionsService
		(
			IDataRepository<Permission> permissionRepo,
			IDataRepository<SecurityGroup> securityGroupRepo,
			IDataRepository<SecurityGroupLogin> securityGroupLoginRepo
		)
		{
			PermissionRepo = permissionRepo;
			SecurityGroupRepo = securityGroupRepo;
			SecurityGroupLoginRepo = securityGroupLoginRepo;
		}

		#endregion

		#region IGetPermissionService

		public IListPermissionsService WithSearch(string search)
		{
			_search = search;

			return this;
		}

		public IListPermissionsService WithSecurityGroupIds(List<int> ids)
		{
			_securityGroupIds = ids;

			return this;
		}

		public IListPermissionsService WithLoginIds(List<int> ids)
		{
			_loginIds = ids;

			return this;
		}

		public IListPermissionsService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;

			_sortBy = value;

			return this;
		}

		public IListPermissionsService WithPaging(int page, int pageSize)
		{
			_page = page;
			_pageSize = pageSize;

			return this;
		}

		public PermissionList List(bool includeDates = true, bool includeUniqueName = true)
		{
			_permissions = PermissionQuery();

			Search();
			FilterSecurityGroups();
			FilterLogins();
			SortBy();

			GetTotalRecords();
			GetPaging();

			return new PermissionList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _permissions.Select(p => new PermissionListItem
				{
					PermissionId = p.PermissionId,
					CreatedOn = includeDates ? p.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? p.ModifiedOn : (DateTime?)null,
					DisplayName = p.PermissionDisplayName,
					Description = p.PermissionDescription,
					SystemName = includeUniqueName ? p.PermissionUniqueName : null,
					Group = p.PermissionGroupName
				}).ToList()
			};
		}

		#endregion

		#region Private Method

		private IQueryable<Permission> PermissionQuery()
		{
			var pFuture = PermissionRepo.Table.Future();

			// ReSharper disable UnusedVariable
			var sgFuture = SecurityGroupRepo.Table.Future();

			var lFuture = (from sg in SecurityGroupRepo.Table
						   join sgl in SecurityGroupLoginRepo.Table on sg.SecurityGroupId equals sgl.SecurityGroupId
						   select sgl);
			// ReSharper enable UnusedVariable

			return pFuture.AsQueryable();
		}

		private void Search()
		{
			if (IsNullOrWhiteSpace(_search)) return;

			_permissions = _permissions.Where(p => p.PermissionUniqueName.Contains(_search)
												   || p.PermissionGroupName.Contains(_search)
												   || p.PermissionDescription.Contains(_search)
												   || p.PermissionDisplayName.Contains(_search));
		}

		private void FilterSecurityGroups()
		{
			if (!(_securityGroupIds?.Any() ?? false)) return;

			_permissions = _permissions.Where(p => p.SecurityGroups.Any(sg => _securityGroupIds.Contains(sg.SecurityGroupId)));
		}

		private void FilterLogins()
		{
			if (!(_loginIds?.Any() ?? false)) return;

			_permissions = _permissions.Where(p => p.SecurityGroups.Any(sg => sg.SecurityGroupLogins.Any(sgl => _loginIds.Contains(sgl.LoginId))));
		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.Name:
					_permissions = _permissions.OrderBy(p => p.PermissionDisplayName);
					break;
				case Filters.SortBy.NameDesc:
					_permissions = _permissions.OrderByDescending(p => p.PermissionDisplayName);
					break;
				case Filters.SortBy.Group:
					_permissions = _permissions.OrderBy(p => p.PermissionGroupName);
					break;
				case Filters.SortBy.GroupDesc:
					_permissions = _permissions.OrderByDescending(p => p.PermissionGroupName);
					break;
				default:
					_permissions = _permissions.OrderBy(p => p.PermissionId);
					break;
			}
		}

		private void GetTotalRecords()
		{
			_totalRecords = _permissions.Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_permissions = _permissions.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		#endregion
	}
}