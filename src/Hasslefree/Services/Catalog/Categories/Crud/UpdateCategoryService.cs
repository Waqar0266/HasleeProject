using Hasslefree.Core.Configuration;
using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Core.Managers;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.Helpers;
using Hasslefree.Services.Infrastructure.Storage;
using Hasslefree.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.Configuration;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class UpdateCategoryService : IUpdateCategoryService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "CategoryId", "CreatedOnUtc", "Path", "StoreId", "PictureId" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Category> CategoryRepo { get; }
		private IDataRepository<Picture> PictureRepo { get; }

		// Services
		private ICloudStorageService CloudStorage { get; }
		private ISettingsService SettingsService { get; }

		// Other
		private IDataContext Database { get; }
		private IAppSettingsManager AppSettings { get; }

		#endregion

		#region Fields

		private Category _category;
		private Category _parent;
		private List<Category> _siblings;
		private List<Category> _pathChildren;

		private bool _updateSeo;
		private bool _updateKeyValues;
		private bool _removeKeyValues;

		private bool _newPicture; // if true move the new picture to amazon
		private bool _deleteOldPicture;
		private string _oldPictureKey;
		private string _pictureSourceKey;
		private string _pictureDestinationKey;

		private readonly Dictionary<CrudAction, HashSet<int>> _attributes = new Dictionary<CrudAction, HashSet<int>>
		{
			{ CrudAction.Create, new HashSet<int>() },
			{ CrudAction.Remove, new HashSet<int>() },
		};

		private readonly HashSet<int> _withProductIds = new HashSet<int>();
		private readonly HashSet<string> _withProductSkus = new HashSet<string>();

		private readonly HashSet<int> _removeProductIds = new HashSet<int>();
		private readonly HashSet<string> _removeProductSkus = new HashSet<string>();

		private string _defaultUrl;
		private readonly HashSet<string> _removeUrls = new HashSet<string>();

		private MediaSettings _mediaSettings;

		#endregion

		#region Constructor

		public UpdateCategoryService
		(
			IDataRepository<Category> categoryRepo,
			IDataRepository<Picture> pictureRepo,
			ICloudStorageService cloudStorage,
			ISettingsService settingsService,
			IDataContext database,
			IAppSettingsManager appSettings
		)
		{
			// Repos
			CategoryRepo = categoryRepo;
			PictureRepo = pictureRepo;

			// Services
			CloudStorage = cloudStorage;
			SettingsService = settingsService;

			// Other
			Database = database;
			AppSettings = appSettings;
		}

		#endregion

		#region IUpdateCategoryService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<CategoryWarning> Warnings { get; } = new List<CategoryWarning>();

		public IUpdateCategoryService this[int categoryId]
		{
			get
			{
				if (categoryId <= 0)
					return this;

				_category = CategoryQuery(categoryId);

				return this;
			}
		}

		public IUpdateCategoryService WithCategoryId(int categoryId) => this[categoryId];

		public IUpdateCategoryService Set<T>(Expression<Func<Category, T>> lambda, object value)
		{
			_category?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public IUpdateCategoryService SetDescription(string description, bool updateDefault = false)
		{
			if (_category == null)
				return this;

			if (updateDefault)
				_category.Description = description;

			return this;
		}

		public IUpdateCategoryService WithAttribute(int attributeValueId)
		{
			if (attributeValueId <= 0)
				return this;

			if (_attributes[CrudAction.Create].Contains(attributeValueId))
				return this;

			_attributes[CrudAction.Create].Add(attributeValueId);

			return this;
		}

		public IUpdateCategoryService RemoveAttribute(int attributeValueId)
		{
			if (attributeValueId <= 0)
				return this;

			if (_attributes[CrudAction.Remove].Contains(attributeValueId))
				return this;

			_attributes[CrudAction.Remove].Add(attributeValueId);

			return this;
		}

		public IUpdateCategoryService SetKeyValue(string key, string value)
		{
			if (_category == null)
				return this;

			return this;
		}

		public IUpdateCategoryService RemoveKeyValue(string key)
		{
			if (_category == null)
				return this;

			return this;
		}

		public IUpdateCategoryService SetPicture(string picturePath, string altText = null, string transforms = null, bool move = false, bool deleteOldPicture = false)
		{
			// Not Found
			if (_category == null)
				return this;

			// If the picture path is null and the category picture id is null then there is no picture at all
			if (IsNullOrWhiteSpace(picturePath) && !_category.PictureId.HasValue)
				return this;

			// Delete the old image if our new path is empty as in we have no picture
			if (IsNullOrWhiteSpace(picturePath) && _category.Picture != null)
			{
				//Remove the picture from the repo
				if (_category.Picture != null)
				{
					_oldPictureKey = _category.Picture.Path.Replace(AppSettings.CdnRoot, Empty).TrimStart('/');

					PictureRepo.Delete(_category.Picture);

					//Set to remove from the cloud
					_deleteOldPicture = true;
				}

				//Unlink from the category
				_category.Picture = null;

				return this;
			}

			//Is this a new picture?
			if (_category.Picture == null)
			{
				_newPicture = true;
				_category.Picture = new Picture();
			}

			//Set the alt text before hand
			if (altText != null)
				_category.Picture.AltText = altText;

			var oldPicture = new Picture();

			if (_category.PictureId.HasValue && _category.PictureId.Value > 0)
			{
				oldPicture = PictureRepo.Table.FirstOrDefault(p => p.PictureId == _category.PictureId.Value);

				_oldPictureKey = oldPicture?.Path?.Replace(AppSettings.CdnRoot, Empty).TrimStart('/');
			}

			// Ignore if the pictures paths are the same
			if (picturePath == oldPicture?.Path && !_newPicture)
				return this;

			// Get our picture extension 
			var imageExtension = System.IO.Path.GetExtension(picturePath)?.ToLower();

			// Get only the file name
			var fileName = Path.GetFileName(picturePath);

			// TODO: Move out into some kind of extension method DetermineMineType(string extension) (used in the MediaController as well)
			//Assign the picture format and mime type
			var mimeType = "";
			var format = PictureFormat.Jpeg;
			if (!IsNullOrWhiteSpace(imageExtension))
			{
				if (imageExtension.Contains(".bmp"))
				{
					format = PictureFormat.Bitmap;
					mimeType = "image/bmp";
				}
				else if (imageExtension.Contains(".gif"))
				{
					format = PictureFormat.Gif;
					mimeType = "image/gif";
				}
				else if (imageExtension.Contains(".jp")) //jpeg, jpg formats
				{
					format = PictureFormat.Jpeg;
					mimeType = "image/jpeg";
				}
				else if (imageExtension.Contains(".png"))
				{
					format = PictureFormat.Png;
					mimeType = "image/png";
				}
				else if (imageExtension.Contains(".tif"))
				{
					format = PictureFormat.Tif;
					mimeType = "image/tiff";
				}
			}

			// If we have updated this picture should we remove the old one no longer used?
			_deleteOldPicture = !_newPicture && deleteOldPicture;

			// Set the source picture key for use (source)
			_pictureSourceKey = picturePath?.Replace(AppSettings.CdnRoot, Empty).TrimStart('/');

			// Set the old picture to use in the moving code
			if (!_deleteOldPicture)
				_oldPictureKey = _pictureSourceKey;

			// Get the category folder storage from system settings
			var categoryStorageFolder = Format("Categories", "").TrimStart('/').TrimEnd('/');

			// Load media settings if they are empty
			_mediaSettings = _mediaSettings ?? SettingsService.LoadSetting<MediaSettings>();

			// Determine the relative folder path where this picture will be stored (destination)
			var relativeDestinationFolder = $"/{_mediaSettings.StorageRootPath}/{categoryStorageFolder}";

			var nameRegex = new Regex("[^a-zA-Z0-9]");
			var ticks = DateTime.UtcNow.Ticks.ToString();
			var destinationImageName = $"{nameRegex.Replace(_category.Name, "")}_{ticks.Substring(ticks.Length - 5, 5)}";

			// Set the relative destination for the newly uploaded picture (destination)
			_pictureDestinationKey = $"{relativeDestinationFolder}/{destinationImageName}{imageExtension}";

			// Pre-determine the final picture path of the image (destination)
			var path = AppSettings.PrependCdnRoot(_pictureDestinationKey);

			// Assign the old picture (if any)
			if (!_newPicture)
				_category.Picture = oldPicture;

			if (_category.Picture == null)
				return this;

			// Set the picture information
			_category.Picture.Path = path;
			_category.Picture.Folder = relativeDestinationFolder;
			_category.Picture.MimeType = mimeType;
			_category.Picture.Name = fileName;
			_category.Picture.Format = format;
			_category.Picture.Transforms = transforms;

			return this;
		}

		public IUpdateCategoryService RemovePicture() => _category?.PictureId == null ? this : SetPicture(null, null, null, true, true);

		public IUpdateCategoryService WithProduct(int productId)
		{
			if (productId <= 0)
				return this;

			if (_withProductIds.Contains(productId))
				return this;

			_withProductIds.Add(productId);

			return this;
		}

		public IUpdateCategoryService WithProduct(string sku)
		{
			if (IsNullOrWhiteSpace(sku))
				return this;

			if (_withProductSkus.Contains(sku))
				return this;

			_withProductSkus.Add(sku);

			return this;
		}

		public IUpdateCategoryService RemoveProduct(int productId)
		{
			if (productId <= 0)
				return this;

			if (_removeProductIds.Contains(productId))
				return this;

			_removeProductIds.Add(productId);

			return this;
		}

		public IUpdateCategoryService RemoveProduct(string sku)
		{
			if (IsNullOrWhiteSpace(sku))
				return this;

			if (_removeProductSkus.Contains(sku))
				return this;

			_removeProductSkus.Add(sku);

			return this;
		}

		public IUpdateCategoryService SetDefaultUrl(string url)
		{
			// Clean the url
			var cleanUrl = CleanUrl(url);

			if (IsNullOrWhiteSpace(cleanUrl))
				return this;

			_defaultUrl = cleanUrl;

			return this;
		}

		public IUpdateCategoryService WithUrl(string url)
		{
			if (IsNullOrWhiteSpace(url))
				return this;

			var cleanUrl = CleanUrl(url);

			return this;
		}

		public IUpdateCategoryService RemoveUrl(string url)
		{
			if (IsNullOrWhiteSpace(url))
				return this;

			if (_removeUrls.Contains(url))
				return this;

			_removeUrls.Add(url);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings)
				return false;

			UpdateBasics();

			UpdatePicture();

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				// SEO
				if (!UpdateSeo()) return false;

				// Set Meta Data
				if (!UpdateKeyValues()) return false;
				if (!RemoveKeyValues()) return false;

				// Sitemap
				SetDefaultCategoryUrl();

				if (!SetSitemap()) return false;
				if (!RemoveSitemap()) return false;

				UpdateAttributes();
				UpdateProducts();

				// necessary ?
				_category.ModifiedOn = DateTime.Now;
				CategoryRepo.Edit(_category);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private Category CategoryQuery(int categoryId)
		{
			var cFuture = (from c in CategoryRepo.Table
						   where c.CategoryId == categoryId
						   select c).DeferredFirstOrDefault().FutureValue();

			var siblingsFuture = (from c in CategoryRepo.Table
								  where c.CategoryId == categoryId && c.ParentCategoryId.HasValue
								  join pc in CategoryRepo.Table on c.ParentCategoryId.Value equals pc.CategoryId
								  join sc in CategoryRepo.Table on pc.CategoryId equals sc.ParentCategoryId
								  where sc.CategoryId != categoryId
								  select sc).Future();

			var picFuture = (from c in CategoryRepo.Table
							 where c.CategoryId == categoryId && c.PictureId.HasValue
							 join p in PictureRepo.Table on c.PictureId.Value equals p.PictureId
							 select p).DeferredFirstOrDefault().FutureValue();

			var category = cFuture.Value;

			if (category == null)
				return null;

			// GetParent(categoryId);
			category.Picture = category.Picture ?? picFuture.Value;
			_siblings = siblingsFuture.ToList();

			return category;
		}

		private bool IsValid()
		{
			if (_category == null)
			{
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoryNotFound));
				return false;
			}

			// Duplicate Path Name
			if (_siblings?.Any(c => c.Path == _category.Path) ?? false)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.DuplicateCategoryPath, _category.Path));

			if (_parent != null && _category.ParentCategoryId.HasValue && _category.CategoryId == _category.ParentCategoryId.Value)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.SelfParentCategory));

			return !Warnings.Any();
		}

		private static string CleanUrl(string url)
		{
			if (IsNullOrWhiteSpace(url))
				return null;

			url = url.Trim().Replace(' ', '-');
			url = Regex.Replace(url, "[^a-zA-Z0-9-_.,!()@@$/\\/]", "").ToLower();

			if (!url.StartsWith("/"))
				url = $"/{url}";

			return url;
		}

		private void UpdateBasics()
		{
			GetParent(_category.ParentCategoryId);

			UpdatePath();
			UpdateSiblings();
		}

		private void GetParent(int? parentId)
		{
			if (!parentId.HasValue)
				return;

			var parentFuture = (from c in CategoryRepo.Table
								where c.CategoryId == parentId.Value
								select c).DeferredFirstOrDefault().FutureValue();

			var pathChildrenFuture = (from c in CategoryRepo.Table
									  where c.CategoryId == _category.CategoryId
									  join cc in CategoryRepo.Table.Where(c => c.ParentCategoryId.HasValue) on c.CategoryId equals cc.ParentCategoryId.Value
									  select cc).Future();

			_parent = parentFuture.Value;
			_pathChildren = pathChildrenFuture.ToList();
		}

		private void UpdatePath()
		{
			// Category Path
			var currentPath = $"/{_category.Name}";

			// Work out the new path do a comparison if we need to update the child category paths
			var newPath = _parent != null ? _parent.Path + currentPath : currentPath;

			// If the new category path is not the same as the current one update it
			// and update any children with the old path prefix part
			if (_category.Path == newPath)
				return;

			_pathChildren?.ForEach(x => x.Path = x.Path.Replace(_category.Path, newPath));

			_category.Path = newPath;
		}

		private void UpdateSiblings()
		{
			// Sibling Count
			var siblings = _siblings?.Count ?? 0;

			// Nested level, determine the nested level by counting the amount of paths
			_category.NestedLevel = _category.Path.Count(x => x == '/') - 1;

			if (_category.DisplayOrder == 0) // Default Display Order
			{
				_category.DisplayOrder = siblings;
			}
			else if (siblings > 0) // Recalculate Display Order
			{
				if (_category.DisplayOrder > siblings)
					_category.DisplayOrder = siblings;
				else
					for (var c = 0; c < siblings; c++)
					{
						var sibling = _siblings?[c];
						if (sibling == null)
							continue;

						if (sibling.DisplayOrder < _category.DisplayOrder)
							continue;

						if (sibling.DisplayOrder >= _category.DisplayOrder)
							sibling.DisplayOrder = c + 1;
					}
			}
		}

		private void UpdatePicture()
		{
			var bucketName = WebConfigurationManager.AppSettings["BucketName"];

			// Remove the picture
			if (!_newPicture && _deleteOldPicture && _oldPictureKey != null)
				CloudStorage.WithBucket(bucketName)
					.RemoveObject(_oldPictureKey)
					.Process();

			// Move the picture
			if (_pictureSourceKey != null && _pictureDestinationKey != null && (_pictureSourceKey != _pictureDestinationKey))
				CloudStorage.WithBucket(bucketName)
					.MoveObject(new[] { _pictureSourceKey, _pictureDestinationKey })
					.Process();
		}

		private bool UpdateSeo()
		{
			return false;
		}

		private void UpdateAttributes()
		{
			// Remove any attributes set to remove
			RemoveAttributes();

			// Check if there are any to create
			AddAttributes();
		}

		private void RemoveAttributes()
		{
			if (!_attributes[CrudAction.Remove].Any())
				return;
		}

		private void AddAttributes()
		{
			if (!_attributes[CrudAction.Create].Any())
				return;

			var ids = _attributes[CrudAction.Create].AsEnumerable();
		}

		private bool UpdateKeyValues()
		{
			return false;
		}

		private bool RemoveKeyValues()
		{
			return false;
		}

		private bool SetSitemap()
		{
			return false;
		}

		private bool RemoveSitemap()
		{
			if (!_removeUrls?.Any() ?? true)
				return true;

			return false;
		}

		private bool SetDefaultCategoryUrl()
		{
			return true;
		}

		private void UpdateProducts()
		{
			
		}

		#endregion
	}
}