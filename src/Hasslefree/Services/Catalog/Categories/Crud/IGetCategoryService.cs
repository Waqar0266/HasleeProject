using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Web.Models.Catalog.Categories.Get;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface IGetCategoryService
	{
		CategoryWarning Warning { get; }

		CategoryGet this[int categoryId, bool includeDates = true, bool includeProducts = false] { get; }

		QueryFutureValue<Category> FutureValue(int categoryId);
	}
}
