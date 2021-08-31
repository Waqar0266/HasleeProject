using Hasslefree.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface IUpdateCategoryService
	{
		bool HasWarnings { get; }
		List<CategoryWarning> Warnings { get; }

		IUpdateCategoryService this[int categoryId] { get; }
		IUpdateCategoryService WithCategoryId(int categoryId);

		IUpdateCategoryService Set<T>(Expression<Func<Category, T>> lambda, object value);
		IUpdateCategoryService SetDescription(string description, bool updateDefault = false);

		bool Update(bool saveChanges = true);
	}
}