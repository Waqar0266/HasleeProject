namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public enum CategoryWarningCode
	{
		// object
		CategoryNotFound,
		CategoriesNotFound,
		ParentCategoryNotFound,
		NullCategory,

		// Basic Properties
		NullCategoryName,
		DuplicateCategoryName,
		CategoryNameMinLengthNotReached,
		CategoryNameMaxLengthExceeded,
		DuplicateCategoryPath,
		CategoryPathLengthExceeded,
		CategoryDescriptionMaxLengthExceeded,
		CategoryDisplayOrderLessThanZero,
		CategoryTagMaxLengthExceeded,
		SelfParentCategory,
		PropertyNotFound,
		RestrictedProperty,
		CannotDeleteParentCategory,

		// Meta Data
		CategoryNullMetaData,

		// SEO
		NullCategorySeo,
		NullCategorySeoTitle,
		CategoryRemoveSeo,
		CategorySetSeo,

		// Sitemap
		CategoryAddSitemap,
		CategoryRemoveSitemap,
		CategorySetSitemap,
	}
}