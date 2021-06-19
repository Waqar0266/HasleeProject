using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface IResetCategoryDisplayOrderService : IInstancePerRequest
	{
		IResetCategoryDisplayOrderService this[int categoryId] { get; }
		bool Reset();
	}
}
