using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Services.Catalog.Categories.Crud.Filters;
using Hasslefree.Web.Models.Catalog.Categories.List;
using System;
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class ListCategoriesService : IListCategoriesService
	{
		#region Private Properties

		private IDataRepository<Category> CategoryRepo { get; }
		private IDataRepository<Picture> PictureRepo { get; }

		#endregion

		#region Fields

		private DateTime? _createdAfter;
		private DateTime? _createdBefore;

		private string _search;
		private FilterBy? _filterBy;
		private SortBy? _sortBy;

		private List<int> _productIds;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private IQueryable<Category> _categories;

		#endregion

		#region Constructor

		public ListCategoriesService
		(
			IDataRepository<Category> categoryRepo,
			IDataRepository<Picture> pictureRepo
		)
		{
			CategoryRepo = categoryRepo;
			PictureRepo = pictureRepo;
		}

		#endregion

		#region IListCategoriesService

		public IListCategoriesService CreatedBefore(DateTime? createdBefore)
		{
			_createdBefore = createdBefore;
			return this;
		}

		public IListCategoriesService CreatedAfter(DateTime? createdAfter)
		{
			_createdAfter = createdAfter;
			return this;
		}

		public IListCategoriesService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IListCategoriesService SortBy(string sortBy)
		{
			if (!Enum.TryParse(sortBy, true, out SortBy value)) return this;
			_sortBy = value;
			return this;
		}

		public IListCategoriesService FilterBy(string filterBy)
		{
			if (!Enum.TryParse(filterBy, true, out FilterBy value)) return this;
			_filterBy = value;
			return this;
		}

		public IListCategoriesService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;

			return this;
		}

		public IListCategoriesService WithProductIds(List<int> productIds)
		{
			_productIds = productIds;
			return this;
		}

		public CategoryList List(bool includeDates = true, bool includePicturePath = false)
		{
			_categories = CategoryQuery(includePicturePath);

			FilterCreatedBefore();
			FilterCreatedAfter();
			FilterSearch();
			FilterBy();
			SortBy();
			FilterProductIds();

			GetTotalRecords();
			GetPaging();

			return new CategoryList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				Items = _categories.AsEnumerable().Select(c => new CategoryListItem
				{
					CategoryId = c.CategoryId,
					CreatedOn = includeDates ? c.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? c.ModifiedOn : (DateTime?)null,
					Description = c.Description,
					Path = c.Path,
					Name = c.Name,
					NestedLevel = c.NestedLevel,
					DisplayOrder = c.DisplayOrder,
					ParentCategoryId = c.ParentCategoryId,
					Hidden = c.Hidden,
					Tag = c.Tag,
					PictureUrl = includePicturePath ? c.Picture?.Path : null
				}).ToList()
			};
		}

		#endregion

		#region Private Methods

		private IQueryable<Category> CategoryQuery(bool includePicturePath)
		{
			var cFuture = (from c in CategoryRepo.Table select c).Future();

			if (!includePicturePath) return cFuture.AsQueryable();

			var pFuture = (from c in CategoryRepo.Table
						   where c.PictureId.HasValue
						   join p in PictureRepo.Table on c.PictureId.Value equals p.PictureId
						   select p).DeferredFirstOrDefault().FutureValue();
			// ReSharper enable UnusedVariable

			return cFuture.AsQueryable();
		}

		private void FilterSearch()
		{
			if (IsNullOrWhiteSpace(_search)) return;

			string searchQuery = _search.ToLower().Trim();

			_categories = _categories.Where(c => CategorySearchHelper(c.Name).Contains(searchQuery) ||
												  CategorySearchHelper(c.Path).Contains(searchQuery));
		}

		private string CategorySearchHelper(string property) => property?.Replace("/", "").ToLower();

		private void FilterBy()
		{
			switch (_filterBy)
			{
				case Filters.FilterBy.Hidden:
					_categories = _categories.Where(c => c.Hidden);
					break;
				case Filters.FilterBy.Parent:
					_categories = _categories.Where(c => !c.ParentCategoryId.HasValue);
					break;
			}
		}

		private void SortBy()
		{
			switch (_sortBy)
			{
				case Filters.SortBy.Created:
					_categories = _categories.OrderBy(c => c.CreatedOn);
					break;
				case Filters.SortBy.CreatedDesc:
					_categories = _categories.OrderByDescending(c => c.CreatedOn);
					break;
				case Filters.SortBy.Name:
					_categories = _categories.OrderBy(c => c.Name);
					break;
				case Filters.SortBy.NameDesc:
					_categories = _categories.OrderByDescending(c => c.CategoryId);
					break;
				case Filters.SortBy.Path:
					_categories = _categories.OrderBy(c => c.Path).ThenBy(c => c.NestedLevel).ThenBy(c => c.DisplayOrder);
					break;
				case Filters.SortBy.PathDesc:
					_categories = _categories.OrderByDescending(c => c.Path).ThenByDescending(c => c.NestedLevel).ThenByDescending(c => c.DisplayOrder);
					break;
				default:
					_categories = _categories.OrderBy(c => c.CategoryId);
					break;
			}
		}

		private void FilterCreatedAfter()
		{
			if (!_createdAfter.HasValue) return;

			_categories = _categories.Where(a => a.CreatedOn >= _createdAfter.Value);
		}

		private void FilterCreatedBefore()
		{
			if (!_createdBefore.HasValue) return;

			_categories = _categories.Where(a => a.CreatedOn < _createdBefore.Value);
		}

		private void FilterProductIds()
		{
			if (!_productIds?.Any() ?? true) return;
		}

		private void GetTotalRecords()
		{
			_totalRecords = _categories.Select(c => c.CategoryId).Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_categories = _categories.Skip(_page * _pageSize.Value).Take(_pageSize.Value);
		}

		#endregion
	}
}
