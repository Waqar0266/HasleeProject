using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Services.Infrastructure.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Configuration;
using Hasslefree.Core.Managers;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class DeleteCategoryService : IDeleteCategoryService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Category> CategoryRepo { get; }
		private IDataRepository<Picture> PictureRepo { get; }

		// Services
		private ICloudStorageService CloudStorageService { get; }

		private IAppSettingsManager AppSettings { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private bool _removeImages;
		private bool _removeFiles;

		private readonly HashSet<int> _categoryIds = new HashSet<int>();

		#endregion

		#region Constructor

		public DeleteCategoryService
		(
			IDataRepository<Category> categoryRepo,
			IDataRepository<Picture> pictureRepo,
			ICloudStorageService cloudStorageService,
			IAppSettingsManager appSettings,
			IDataContext database
		)
		{
			// Repos
			CategoryRepo = categoryRepo;
			PictureRepo = pictureRepo;

			// Services
			CloudStorageService = cloudStorageService;

			AppSettings = appSettings;

			// Other
			Database = database;
		}

		#endregion

		#region IDeleteCategoryService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<CategoryWarning> Warnings { get; } = new List<CategoryWarning>();

		public IDeleteCategoryService this[int categoryId]
		{
			get
			{
				if (categoryId <= 0) return this;

				if (_categoryIds.Contains(categoryId)) return this;

				_categoryIds.Add(categoryId);

				return this;
			}
		}

		public IDeleteCategoryService this[List<int> categoryIds]
		{
			get
			{
				if (!categoryIds?.Any() ?? true) return this;

				foreach (var categoryId in categoryIds)
				{
					if (_categoryIds.Contains(categoryId)) continue;

					_categoryIds.Add(categoryId);
				}

				return this;
			}
		}

		public IDeleteCategoryService RemoveImages(bool removeFiles = false)
		{
			_removeImages = true;
			_removeFiles = removeFiles;

			return this;
		}

		public bool Remove(bool saveChanges = true)
		{
			var categories = GetCategories();

			if (HasWarnings) return Clear(false);

			if (!categories.Any()) return Clear(true);

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				// Remove the categories
				CategoryRepo.Remove(categories);

				// Remove the SEO records
				if (!RemoveSeoRecords(categories.Select(c => c.SeoId).Distinct().ToList())) return Clear(false);

				// Remove the Sitemap records
				if (!RemoveSitemapRecords(categories.Select(c => c.CategoryId).Distinct().ToList())) return Clear(false);

				// Remove the pictures
				if (_removeImages) RemoveCategoryPictures(categories.Where(c => c.PictureId.HasValue).Select(c => c.PictureId.Value).ToList());

				// Return if the changes mustn't be saved
				if (!saveChanges) return Clear(true);

				// Save the changes to the database
				Database.SaveChanges();

				scope.Complete();
			}

			return Clear(true);
		}

		#endregion

		#region Private Methods

		private List<Category> GetCategories()
		{
			return (from c in CategoryRepo.Table
					where _categoryIds.Contains(c.CategoryId)
					from sc in CategoryRepo.Table
					where sc.Path.Equals(c.Path)
						  || sc.Path.StartsWith(c.Path + "/")
						  || sc.ParentCategoryId.HasValue
						  && _categoryIds.Contains(sc.ParentCategoryId.Value)
					select sc).ToList();
		}

		private bool IsValid()
		{
			if (!_categoryIds.Any())
				Warnings.Add(new CategoryWarning(CategoryWarningCode.CategoriesNotFound));

			return !Warnings.Any();
		}

		private void RemoveCategoryPictures(ICollection<int> pictureIds)
		{
			var (pictures, paths) = GetCategoryPictures(pictureIds);

			if (!pictures.Any()) return;

			PictureRepo.Remove(pictures);

			if (!_removeFiles || !(paths?.Any() ?? false)) return;

			foreach (var path in paths)
			{
				var pictureKey = path.Replace(AppSettings.CdnRoot, "").TrimEnd('/');

				if (IsNullOrWhiteSpace(pictureKey)) continue;

				// Remove the picture from cloud storage
				CloudStorageService
					.WithBucket(WebConfigurationManager.AppSettings["BucketName"])
					.RemoveObject(pictureKey)
					.Process();
			}
		}

		private (List<Picture> pictures, List<string> paths) GetCategoryPictures(ICollection<int> pictureIds)
		{
			var pictures = PictureRepo.Table.Where(p => pictureIds.Contains(p.PictureId)).ToList();

			return !_removeFiles ? (pictures, null) : (pictures, pictures.Select(p => p.Path).ToList());
		}

		private bool RemoveSeoRecords(List<int> seoIds)
		{
			if (!seoIds?.Any() ?? true) return true;

			return false;
		}

		private bool RemoveSitemapRecords(List<int> categoryIds)
		{
			return false;
		}

		private bool Clear(bool success)
		{
			_removeFiles = _removeImages = false;
			_categoryIds.Clear();

			return success;
		}

		#endregion
	}
}