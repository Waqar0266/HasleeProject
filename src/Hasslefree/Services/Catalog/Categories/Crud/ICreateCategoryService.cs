using System.Collections.Generic;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface ICreateCategoryService
	{
		bool HasWarnings { get; }
		List<CategoryWarning> Warnings { get; }
		int CategoryId { get; }

		ICreateCategoryService New(string name, string description, bool hidden, int displayOrder = 0, string tag = null, int? parentCategoryId = null);

		bool Create();
	}
}
