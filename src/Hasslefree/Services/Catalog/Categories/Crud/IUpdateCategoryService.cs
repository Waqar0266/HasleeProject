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

		IUpdateCategoryService WithAttribute(int attributeValueId);
		IUpdateCategoryService RemoveAttribute(int attributeValueId);

		IUpdateCategoryService SetKeyValue(string key, string value);
		IUpdateCategoryService RemoveKeyValue(string key);

		IUpdateCategoryService SetPicture(string picturePath, string altText = null, string transforms = null, bool move = false, bool deleteOldPicture = false);
		IUpdateCategoryService RemovePicture();

		IUpdateCategoryService WithProduct(int productId);
		IUpdateCategoryService WithProduct(string sku);
		IUpdateCategoryService RemoveProduct(int productId);
		IUpdateCategoryService RemoveProduct(string sku);

		IUpdateCategoryService SetDefaultUrl(string url);
		IUpdateCategoryService WithUrl(string url);
		IUpdateCategoryService RemoveUrl(string url);


		bool Update(bool saveChanges = true);
	}
}