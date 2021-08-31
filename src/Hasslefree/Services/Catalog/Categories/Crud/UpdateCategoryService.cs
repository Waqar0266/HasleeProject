using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Transactions;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class UpdateCategoryService : IUpdateCategoryService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "CategoryId", "CreatedOnUtc", "Path", "PictureId" };

		#endregion

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
		private List<Category> _pathChildren;

		#endregion

		#region Constructor

		public UpdateCategoryService
		(
			IDataRepository<Category> categoryRepo,
			IDataContext database
		)
		{
			// Repos
			CategoryRepo = categoryRepo;

			// Other
			Database = database;
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

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings)
				return false;

			UpdateBasics();

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
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

			var category = cFuture.Value;

			if (category == null)
				return null;

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

		#endregion
	}
}