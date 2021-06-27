using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Core.Managers;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class CreateCategoryService : ICreateCategoryService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<Category> CategoryRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private Category _category;

		private Category _parent;
		private List<Category> _siblings;

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

		#endregion
	}
}