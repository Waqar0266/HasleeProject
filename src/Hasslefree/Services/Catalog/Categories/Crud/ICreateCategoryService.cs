using System.Collections.Generic;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public interface ICreateCategoryService
	{
		bool HasWarnings { get; }
		List<CategoryWarning> Warnings { get; }
		int CategoryId { get; }

		ICreateCategoryService New(string name, string description, bool hidden, int displayOrder = 0, string tag = null, int? parentCategoryId = null);

		ICreateCategoryService WithAttribute(int attributeValueId);
		ICreateCategoryService WithAttributes(List<int> attributeValueIds);

		ICreateCategoryService WithSeo(string title, string description = null, string keywords = null, string canonicalUrl = null);

		ICreateCategoryService WithKeyValue(string key, string value);

		ICreateCategoryService WithPicture(string picturePath, string altText = null, string transforms = null, bool move = false);

		ICreateCategoryService WithProduct(int productId);
		ICreateCategoryService WithProduct(string sku);

		ICreateCategoryService WithUrl(string url);
		ICreateCategoryService WithUrls(List<string> urls);
		ICreateCategoryService WithDefaultUrl(string url);

		bool Create();
	}
}
