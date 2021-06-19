using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class ResetCategoryDisplayService : IResetCategoryDisplayOrderService
	{

		#region Dependencies

		private IDataRepository<Category> CategoryRepo { get; }

		#endregion

		#region Fields

		private int? _categoryId;


		#endregion

		#region Constructor

		public ResetCategoryDisplayService
		(
			IDataRepository<Category> categoryRepo
		)
		{
			//Repos
			CategoryRepo = categoryRepo;
		}

		#endregion

		#region IResetCategoryDisplayOrderService

		public IResetCategoryDisplayOrderService this[int categoryId]
		{
			get
			{
				_categoryId = categoryId;
				return this;
			}
		}

		public bool Reset()
		{
			try
			{
				//Get the Categories
				var categories = GetCategories();

				//Reset Display Order
				ResetCategoryDisplayOrder(categories);

				//Db Save Changes
				CategoryRepo.BulkUpdate(categories);

				return true;
			}
			catch (Exception ex)
			{
				while (ex.InnerException != null) ex = ex.InnerException;
				Core.Logging.Logger.LogError(ex, ex.Message);
			}

			return false;
		}

		#endregion

		#region Private Actions

		private List<Category> GetCategories()
		{
			var path = _categoryId == 0 ? "/" : $"{CategoryRepo.Table.FirstOrDefault(x => x.CategoryId == _categoryId)?.Path}/" ?? "/";
			return (from c in CategoryRepo.Table
					where !c.Hidden &&
					c.Path.StartsWith(path)
					orderby c.Path
					select c).ToList();
		}

		private void ResetCategoryDisplayOrder(List<Category> categories)
		{
			Dictionary<string, int> paths = new Dictionary<string, int>();
			foreach (var category in categories)
			{
				var parentPath = category.Path.Substring(0, category.Path.LastIndexOf('/'));
				if (!paths.ContainsKey(parentPath))
				{
					category.DisplayOrder = 0;
					paths.Add(parentPath, 0);
				}
				else
				{
					var index = paths[parentPath] + 1;
					paths[parentPath] = index;
					category.DisplayOrder = index;
				}
			}
		}

		#endregion
	}
}
