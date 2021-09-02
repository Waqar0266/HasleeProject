using Hasslefree.Core;
using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Properties;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Filter;
using Hasslefree.Web.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using static System.String;

namespace Hasslefree.Services.Filter
{
	public class FilterService : IFilterService, IInstancePerRequest
	{
		#region Private Properties

		//Repos
		private IReadOnlyRepository<Property> PropertyRepo { get; }
		private IReadOnlyRepository<Category> CategoryRepo { get; }
		private IReadOnlyRepository<PropertyBuildingKeyValue> BuildingKeyValueRepo { get; }
		private IReadOnlyRepository<PropertyExternalFeaturesKeyValue> ExternalFeatureKeyValueRepo { get; }
		private IReadOnlyRepository<PropertyOtherFeaturesKeyValue> OtherFeatureKeyValueRepo { get; }
		private IReadOnlyRepository<PropertyOverviewKeyValue> OverviewKeyValueRepo { get; }
		private IReadOnlyRepository<PropertyRoomKeyValue> RoomKeyValueRepo { get; }
		private IReadOnlyRepository<PropertyPicture> ImagesRepo { get; }

		//Other
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private List<int> _categoryIds = new List<int>();
		private string _search;
		private string _sortBy;
		private int _propertyId;

		private int _page;
		private int? _pageSize;
		private int _totalRecords;

		private List<Property> _properties;
		private string _categoryName;
		private string _categoryPath;

		private List<int> _buildingIds = new List<int>();
		private List<int> _externalFeaturesIds = new List<int>();
		private List<int> _roomIds = new List<int>();

		#endregion

		#region Constructor

		public FilterService
		(
			//Repos
			IReadOnlyRepository<Property> propertyRepo,
			IReadOnlyRepository<Category> categoryRepo,
			IReadOnlyRepository<PropertyBuildingKeyValue> buildingKeyValueRepo,
			IReadOnlyRepository<PropertyExternalFeaturesKeyValue> externalFeatureKeyValueRepo,
			IReadOnlyRepository<PropertyOtherFeaturesKeyValue> otherFeatureKeyValueRepo,
			IReadOnlyRepository<PropertyOverviewKeyValue> overviewKeyValueRepo,
			IReadOnlyRepository<PropertyRoomKeyValue> roomKeyValueRepo,
			IReadOnlyRepository<PropertyPicture> imagesRepo,

			//Other
			ICacheManager cache
		)
		{
			//Repos
			PropertyRepo = propertyRepo;
			CategoryRepo = categoryRepo;
			BuildingKeyValueRepo = buildingKeyValueRepo;
			ExternalFeatureKeyValueRepo = externalFeatureKeyValueRepo;
			OtherFeatureKeyValueRepo = otherFeatureKeyValueRepo;
			OverviewKeyValueRepo = overviewKeyValueRepo;
			RoomKeyValueRepo = roomKeyValueRepo;
			ImagesRepo = imagesRepo;

			//Other
			Cache = cache;
		}

		#endregion

		#region IFilterService

		public IFilterService WithPath(string path)
		{
			var category = Cache.Get(CacheKeys.Server.Filter.FilterWithPath(path), CacheKeys.Time.DefaultTime, () => CategoryRepo.Table.Include(c => c.ParentCategory.ParentCategory).Include("SubCategories.SubCategories").FirstOrDefault(c => c.Path.ToLower() == "/" + path.ToLower().Replace("-", " ").Replace("_", "-")));
			_categoryPath = category.Path;

			if (category.NestedLevel == 0)
			{
				_categoryIds = category.SubCategories.SelectMany(c => c.SubCategories.Select(cc => cc.CategoryId)).ToList();
				_categoryName = category.Name;
			}
			else if (category.NestedLevel == 1)
			{
				_categoryIds = category.SubCategories.Select(c => c.CategoryId).ToList();
				_categoryName = $"{category.Name}, {category.ParentCategory.Name}";
			}
			else if (category.NestedLevel == 2)
			{
				_categoryIds = new List<int>() { category.CategoryId };
				_categoryName = $"{category.Name}, {category.ParentCategory.Name}, {category.ParentCategory.ParentCategory.Name}";
			}

			return this;
		}

		public IFilterService WithPropertyId(int propertyId)
		{
			_propertyId = propertyId;
			return this;
		}

		public IFilterService WithSearch(string search)
		{
			_search = search;
			return this;
		}

		public IFilterService WithFilters(string buildingIds = null, string externalFeaturesIds = null, string roomIds = null)
		{
			if (!String.IsNullOrEmpty(buildingIds)) _buildingIds = buildingIds.Split('|').SelectMany(x => x.Split(',').Select(xx => Int32.Parse(xx))).ToList();
			if (!String.IsNullOrEmpty(externalFeaturesIds)) _externalFeaturesIds = externalFeaturesIds.Split('|').SelectMany(x => x.Split(',').Select(xx => Int32.Parse(xx))).ToList();
			if (!String.IsNullOrEmpty(roomIds)) _roomIds = roomIds.Split('|').SelectMany(x => x.Split(',').Select(xx => Int32.Parse(xx))).ToList();
			return this;
		}

		public IFilterService SortBy(string sortBy)
		{
			_sortBy = sortBy;
			return this;
		}

		public IFilterService WithPaging(int page = 0, int pageSize = 50)
		{
			_page = page;
			_pageSize = pageSize;

			return this;
		}

		public FilterList List()
		{
			if (_categoryIds.Any()) _properties = Cache.Get(CacheKeys.Server.Filter.Properties(_categoryIds), CacheKeys.Time.LongTime, () => PropertyQuery());
			else _properties = Cache.Get(CacheKeys.Server.Filter.Properties(_page, (_pageSize.HasValue ? _pageSize.Value : 50)), CacheKeys.Time.LongTime, () => PropertyQuery());

			FilterSearch();
			SortBy();
			FilterIds();

			GetTotalRecords();
			GetPaging();

			var propertyIds = _properties.Select(p => p.PropertyId).ToList();
			var buildingKeyValues = Cache.Get(CacheKeys.Server.Filter.BuildingKeyValues(propertyIds), CacheKeys.Time.LongTime, () => BuildingKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var externalFeatureKeyValues = Cache.Get(CacheKeys.Server.Filter.ExternalFeaturesKeyValues(propertyIds), CacheKeys.Time.LongTime, () => ExternalFeatureKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var otherFeatureKeyValues = Cache.Get(CacheKeys.Server.Filter.OtherFeaturesKeyValues(propertyIds), CacheKeys.Time.LongTime, () => OtherFeatureKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var overviewKeyValues = Cache.Get(CacheKeys.Server.Filter.OverviewKeyValues(propertyIds), CacheKeys.Time.LongTime, () => OverviewKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var roomKeyValues = Cache.Get(CacheKeys.Server.Filter.RoomsKeyValues(propertyIds), CacheKeys.Time.LongTime, () => RoomKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var images = Cache.Get(CacheKeys.Server.Filter.Images(propertyIds), CacheKeys.Time.LongTime, () => ImagesRepo.Table.Include(x => x.Picture).Where(x => propertyIds.Contains(x.PropertyId)).ToList());

			return new FilterList
			{
				Page = _page,
				PageSize = _pageSize ?? _totalRecords,
				TotalRecords = _totalRecords,
				CategoryName = _categoryName,
				CategoryPath = _categoryPath,
				Items = _properties.Select(c => new FilterListItem
				{
					Title = c.Title,
					Description = c.Description,
					Address = c.Address,
					BuildingKeyValues = buildingKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
					ExternalFeaturesKeyValues = externalFeatureKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
					OtherFeaturesKeyValues = otherFeatureKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
					OverviewKeyValues = overviewKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
					RoomsKeyValues = roomKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
					Images = images.Where(x => x.PropertyId == c.PropertyId).Select(i => i.Picture.Path).ToList(),
					Price = c.Price.ToString("N"),
					PriceNumeric = c.Price,
					PropertyId = c.PropertyId,
					PropertyType = c.PropertyType,
					CreatedOn = c.CreatedOn,
					Url = $"/listing-detail/{c.Title.SlugifyUrl()}/{c.PropertyId}",
					CategoryPath = _categoryPath
				}).ToList()
			};
		}

		public FilterListItem Single()
		{
			_properties = Cache.Get(CacheKeys.Server.Filter.Property(_propertyId), CacheKeys.Time.LongTime, () => PropertyQuery());

			var propertyIds = _properties.Select(p => p.PropertyId).ToList();
			var buildingKeyValues = Cache.Get(CacheKeys.Server.Filter.BuildingKeyValues(propertyIds), CacheKeys.Time.LongTime, () => BuildingKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var externalFeatureKeyValues = Cache.Get(CacheKeys.Server.Filter.ExternalFeaturesKeyValues(propertyIds), CacheKeys.Time.LongTime, () => ExternalFeatureKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var otherFeatureKeyValues = Cache.Get(CacheKeys.Server.Filter.OtherFeaturesKeyValues(propertyIds), CacheKeys.Time.LongTime, () => OtherFeatureKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var overviewKeyValues = Cache.Get(CacheKeys.Server.Filter.OverviewKeyValues(propertyIds), CacheKeys.Time.LongTime, () => OverviewKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var roomKeyValues = Cache.Get(CacheKeys.Server.Filter.RoomsKeyValues(propertyIds), CacheKeys.Time.LongTime, () => RoomKeyValueRepo.Table.Where(x => propertyIds.Contains(x.PropertyId)).ToList());
			var images = Cache.Get(CacheKeys.Server.Filter.Images(propertyIds), CacheKeys.Time.LongTime, () => ImagesRepo.Table.Include(x => x.Picture).Where(x => propertyIds.Contains(x.PropertyId)).ToList());

			return _properties.Select(c => new FilterListItem
			{
				Title = c.Title,
				Description = c.Description,
				Address = c.Address,
				BuildingKeyValues = buildingKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
				ExternalFeaturesKeyValues = externalFeatureKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
				OtherFeaturesKeyValues = otherFeatureKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
				OverviewKeyValues = overviewKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
				RoomsKeyValues = roomKeyValues.Where(x => x.PropertyId == c.PropertyId).ToDictionary(x => x.Key, x => x.Value),
				Images = images.Where(x => x.PropertyId == c.PropertyId).Select(i => i.Picture.Path).ToList(),
				Price = c.Price.ToString("N"),
				PriceNumeric = c.Price,
				PropertyId = c.PropertyId,
				PropertyType = c.PropertyType,
				CreatedOn = c.CreatedOn,
				Url = $"/listing-detail/{c.Title.SlugifyUrl()}/{c.PropertyId}"
			}).FirstOrDefault();
		}

		#endregion

		#region Private Methods

		private List<Property> PropertyQuery()
		{
			if (_propertyId > 0)
			{

				return (from p in PropertyRepo.Table
				.Include(x => x.Category)
				.Include(x => x.BuildingKeyValues)
				.Include(x => x.ExternalFeaturesKeyValues)
				.Include(x => x.RoomKeyValues)
						where p.PropertyId == _propertyId
						select p).ToList();
			}
			else if (_categoryIds.Any()) return (from p in PropertyRepo.Table
				.Include(x => x.Category)
				.Include(x => x.BuildingKeyValues)
				.Include(x => x.ExternalFeaturesKeyValues)
				.Include(x => x.RoomKeyValues)
												 where _categoryIds.Contains(p.CategoryId)
												 select p).ToList();
			else return (from p in PropertyRepo.Table
				.Include(x => x.Category)
				.Include(x => x.BuildingKeyValues)
				.Include(x => x.ExternalFeaturesKeyValues)
				.Include(x => x.RoomKeyValues)
						 select p).ToList();
		}

		private void FilterSearch()
		{
			if (IsNullOrWhiteSpace(_search)) return;

			string searchQuery = _search.ToLower().Trim();

			_properties = _properties.Where(c => PropertySearchHelper(c.Title).Contains(searchQuery) ||
												  PropertySearchHelper(c.PrivatePropertyId).Contains(searchQuery)).ToList();
		}

		private void FilterIds()
		{
			if (_buildingIds.Any()) _properties = _properties.Where(x => x.BuildingKeyValues.Any(a => _buildingIds.Contains(a.PropertyBuildingKeyValueId))).ToList();
			if (_externalFeaturesIds.Any()) _properties = _properties.Where(x => x.ExternalFeaturesKeyValues.Any(a => _externalFeaturesIds.Contains(a.PropertyExternalFeaturesKeyValueId))).ToList();
			if (_roomIds.Any()) _properties = _properties.Where(x => x.RoomKeyValues.Any(a => _roomIds.Contains(a.PropertyRoomKeyValueId))).ToList();
		}

		private string PropertySearchHelper(string property) => property?.Replace("/", "").ToLower();

		private void SortBy()
		{
			switch (_sortBy)
			{
				case "MostRecent":
					_properties = _properties.OrderByDescending(c => c.CreatedOn).ToList();
					break;
				case "HighestPrice":
					_properties = _properties.OrderByDescending(c => c.Price).ToList();
					break;
				case "LowestPrice":
					_properties = _properties.OrderBy(c => c.Price).ToList();
					break;
				default:
					_properties = _properties.OrderBy(c => c.PropertyId).ToList();
					break;
			}
		}

		private void GetTotalRecords()
		{
			_totalRecords = _properties.Select(c => c.PropertyId).Count();
		}

		private void GetPaging()
		{
			if (!_pageSize.HasValue) _pageSize = _totalRecords;

			_properties = _properties.Skip(_page * _pageSize.Value).Take(_pageSize.Value).ToList();
		}

		#endregion
	}
}
