using Hasslefree.Core;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.Configuration;
using Hasslefree.Core.Managers;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class CreateCategoryService : ICreateCategoryService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Category> CategoryRepo { get; }

		// Services
		private ICloudStorageService CloudStorageService { get; }
		private ISettingsService SettingsService { get; }

		private IAppSettingsManager AppSettings { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private Category _category;

		private Category _parent;
		private List<Category> _siblings;

		private MediaSettings _mediaSettings;

		private bool _movePicture = true;
		private string _pictureKey;
		private string _pictureDestinationKey;

		private readonly HashSet<int> _attributeValueIds = new HashSet<int>();

		private readonly HashSet<int> _productIds = new HashSet<int>();
		private readonly HashSet<string> _productSkus = new HashSet<string>();

		#endregion

		#region Constructor

		public CreateCategoryService
		(
			IDataRepository<Category> categoryRepo,
			ICloudStorageService cloudStorageService,
			ISettingsService settingsService,
			IAppSettingsManager appSettings,
			IDataContext database)
		{
			// Repos
			CategoryRepo = categoryRepo;

			// Services
			CloudStorageService = cloudStorageService;
			SettingsService = settingsService;

			AppSettings = appSettings;

			// Other
			Database = database;
		}

		#endregion

		#region ICreateCategoryService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<CategoryWarning> Warnings { get; } = new List<CategoryWarning>();

		public int CategoryId { get; private set; }

		public ICreateCategoryService New(string name, string description, bool hidden, int displayOrder = 0, string tag = null, int? parentCategoryId = null)
		{
			_category = new Category
			{
				Name = name,
				Description = description,
				Hidden = hidden,
				DisplayOrder = displayOrder,
				Tag = tag,
				ParentCategoryId = parentCategoryId
			};

			return this;
		}

		public ICreateCategoryService WithAttribute(int attributeValueId)
		{
			if (_attributeValueIds.Contains(attributeValueId)) return this;

			_attributeValueIds.Add(attributeValueId);

			return this;
		}

		public ICreateCategoryService WithAttributes(List<int> attributeValueIds)
		{
			if (!attributeValueIds?.Any() ?? true) return this;

			attributeValueIds.ForEach(id => WithAttribute(id));

			return this;
		}

		public ICreateCategoryService WithKeyValue(string key, string value)
		{
			if (_category == null || IsNullOrWhiteSpace(key)) return this;

			return this;
		}

		public ICreateCategoryService WithPicture(string picturePath, string altText = null, string transforms = null, bool move = true)
		{
			// Get our picture extension 
			var imageExtension = System.IO.Path.GetExtension(picturePath)?.ToLower();

			// Get only the file name
			var fileName = System.IO.Path.GetFileName(picturePath);

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

			// Move Picture
			_movePicture = move;

			// Set the source picture key for use (source)
			_pictureKey = picturePath?.Replace(AppSettings.CdnRoot, "").TrimEnd('/');

			// Get the category folder storage from system settings
			var categoryStorageFolder = Format("Categories", "").TrimStart('/').TrimEnd('/');

			// Load media settings if they are empty
			_mediaSettings = _mediaSettings ?? SettingsService.LoadSetting<MediaSettings>();

			// Determine the relative folder path where this picture will be stored (destination)
			var relativeDestinationFolder = $"/{_mediaSettings.StorageRootPath}/{categoryStorageFolder}";

			var nameRegex = new Regex("[^a-zA-Z0-9]");
			var destinationImageName = $"{nameRegex.Replace(_category.Name, "")}";

			// Set the relative destination for the newly uploaded picture (destination)
			_pictureDestinationKey = $"{relativeDestinationFolder}/{destinationImageName}{imageExtension}";

			// Pre-determine the final picture path of the image (destination)
			var path = _movePicture ? AppSettings.PrependCdnRoot(_pictureDestinationKey) : picturePath;

			// Set Picture
			_category.Picture = new Picture
			{
				Folder = relativeDestinationFolder,
				Format = format,
				MimeType = mimeType,
				AltText = altText,
				Path = path, // pre-determine where the image will be
				Name = fileName,
				Transforms = transforms
			};

			return this;
		}

		public ICreateCategoryService WithProduct(int productId)
		{
			if (productId <= 0) return this;

			if (_productIds.Contains(productId)) return this;

			_productIds.Add(productId);

			return this;
		}

		public ICreateCategoryService WithProduct(string sku)
		{
			if (IsNullOrWhiteSpace(sku)) return this;

			if (_productSkus.Contains(sku)) return this;

			_productSkus.Add(sku);

			return this;
		}

		public ICreateCategoryService WithSeo(string title, string description = null, string keywords = null, string canonicalUrl = null)
		{
			if (_category == null) return this;

			return this;
		}

		public ICreateCategoryService WithUrl(string url)
		{
			if (IsNullOrWhiteSpace(url)) return this;

			var cleanUrl = CleanUrl(url);

			return this;
		}

		public ICreateCategoryService WithUrls(List<string> urls)
		{
			if (!urls?.Any() ?? true) return this;

			foreach (var url in urls) WithUrl(url);

			return this;
		}

		public ICreateCategoryService WithDefaultUrl(string url)
		{
			if (IsNullOrWhiteSpace(url)) return this;

			var cleanUrl = CleanUrl(url);

			return this;
		}

		public bool Create()
		{
			// Fetch related categories i.e. parent, siblings etc
			FetchRelatedCategories();

			// Set the basic entity properties
			BasicProperties();

			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				CategoryRepo.Insert(_category);

				// Add the sitemap
				if (!AddSitemap()) return false;

				// Move the picture
				if (_movePicture && _pictureKey != null && _pictureDestinationKey != null)
				{
					CloudStorageService
						.WithBucket(WebConfigurationManager.AppSettings["BucketName"])
						.MoveObject(new[] { _pictureKey, _pictureDestinationKey })
						.Process();
				}

				// Attributes
				AddAttributes();

				// Products
				LinkProducts();

				// Save
				Database.SaveChanges();

				scope.Complete();
			}



			// Set property object
			CategoryId = _category.CategoryId;

			return true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Fetch related categories.
		/// Only when a parent category was specified.
		/// </summary>m
		private void FetchRelatedCategories()
		{
			if (_category?.ParentCategoryId == null) return;

			// Future Queries
			var queryParent = CategoryRepo.Table
					.DeferredFirstOrDefault(c => c.CategoryId == _category.ParentCategoryId.Value)
					.FutureValue();

			var querySiblings = _category.ParentCategoryId.Value == 0
				? CategoryRepo
					.Table
					.Where(c => !c.ParentCategoryId.HasValue)
					.OrderBy(c => c.DisplayOrder)
					.Future()
				: CategoryRepo
					.Table
					.Where(c => c.ParentCategoryId.Value == _category.ParentCategoryId.Value)
					.OrderBy(c => c.DisplayOrder)
					.Future();

			// Execute Queries
			_parent = queryParent?.Value;
			_siblings = querySiblings?.ToList();
		}

		private void BasicProperties()
		{
			// Category Path
			_category.Path = _parent != null ? $"{_parent.Path}/{_category.Name}" : $"/{_category.Name}";

			// Nested level, determine the nested level by counting the amount of paths
			_category.NestedLevel = _category.Path.Count(x => x == '/') - 1;

			// Sibling Count
			var siblings = _siblings?.Count ?? 0;

			// Default Display Order
			if (_category.DisplayOrder == 0) _category.DisplayOrder = siblings;

			// Recalculate Display Order and Nested Level
			else if (siblings > 0)
			{
				if (_category.DisplayOrder > siblings)
					_category.DisplayOrder = siblings;
				else
					for (var c = 0; c < siblings; c++)
					{
						var sibling = _siblings?[c];
						if (sibling == null) continue;
						if (sibling.DisplayOrder < _category.DisplayOrder) continue;
						if (sibling.DisplayOrder >= _category.DisplayOrder) sibling.DisplayOrder = c + 1;
					}
			}
		}

		private bool IsValid()
		{
			if (_category == null)
			{
				Warnings.Add(new CategoryWarning(CategoryWarningCode.NullCategory));
				return false;
			}

			if (IsNullOrWhiteSpace(_category.Name))
			{
				Warnings.Add(new CategoryWarning(CategoryWarningCode.NullCategoryName));
				return false;
			}

			// Name
			if (_category.Name.Length > 128)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoryNameMaxLengthExceeded));

			// Path
			if (_siblings?.Any(c => c.Path.Equals(_category.Path, StringComparison.CurrentCultureIgnoreCase)) ?? false)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.DuplicateCategoryPath, _category.Path));

			if (_category.Path?.Length > 255)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoryPathLengthExceeded));

			// Description
			if (_category.Description?.Length > 65535)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoryDescriptionMaxLengthExceeded));

			// Display Order
			if (_category.DisplayOrder < 0)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoryDisplayOrderLessThanZero));

			// Tag
			if (_category.Tag?.Length > 1024)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoryTagMaxLengthExceeded));

			if (_parent == null && _category.ParentCategoryId.HasValue || (_parent != null && _category.ParentCategoryId.HasValue && _parent.CategoryId != _category.ParentCategoryId.Value))
				Warnings.Add(new CategoryWarning(CategoryWarningCode.ParentCategoryNotFound));

			// *Check is only valid for API 
			if (_parent != null && _parent.CategoryId == _category.CategoryId)
				Warnings.Add(new CategoryWarning(CategoryWarningCode.SelfParentCategory));

			return !Warnings.Any();
		}

		private static string CleanCanonicalUrl(string canonicalUrl)
		{
			if (IsNullOrWhiteSpace(canonicalUrl)) return null;

			canonicalUrl = canonicalUrl.Trim().Replace(' ', '-');

			if (canonicalUrl.StartsWith("http://") || canonicalUrl.StartsWith("https://") || canonicalUrl.StartsWith("//")) return canonicalUrl;

			return CleanUrl(canonicalUrl);
		}

		private static string CleanUrl(string url)
		{
			if (IsNullOrWhiteSpace(url)) return null;

			url = url.Trim().Replace(' ', '-');

			url = Regex.Replace(url, "[^a-zA-Z0-9-_.,!()@@$/\\/]", "").ToLower();

			if (!url.StartsWith("/")) url = $"/{url}";

			return url;
		}

		private void AddAttributes()
		{
			if (!_attributeValueIds?.Any() ?? true) return;
		}

		private void LinkProducts()
		{
			if (!_productIds.Any() && !_productSkus.Any()) return;
		}

		private bool AddSitemap()
		{
			return false;
		}

		#endregion
	}
}