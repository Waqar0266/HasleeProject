using System.Collections.Generic;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface IDeleteCategoryService
	{
		bool HasWarnings { get; }
		List<CategoryWarning> Warnings { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryId"></param>
		/// <returns></returns>
		IDeleteCategoryService this[int categoryId] { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryIds"></param>
		/// <returns></returns>
		IDeleteCategoryService this[List<int> categoryIds] { get; }

		IDeleteCategoryService RemoveImages(bool removeFiles = false);

		bool Remove(bool saveChanges = true);
	}
}