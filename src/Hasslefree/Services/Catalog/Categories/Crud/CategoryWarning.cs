namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class CategoryWarning
	{
		public CategoryWarning(CategoryWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public CategoryWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case CategoryWarningCode.CategoryNotFound:
						return "Category record was not found.";
					case CategoryWarningCode.CategoriesNotFound:
						return "Category record(s) were not found.";
					case CategoryWarningCode.ParentCategoryNotFound:
						return "Parent Category record was not found";
					case CategoryWarningCode.NullCategory:
						return "Cannot create a new 'Category' as a null object.";

					// Basic properties
					case CategoryWarningCode.NullCategoryName:
						return "'Name' cannot be empty. Please provide a value for 'Name'.";
					case CategoryWarningCode.DuplicateCategoryName:
						return $"A Category record already exists with the name, '{CustomMessage}'.";
					case CategoryWarningCode.CategoryNameMaxLengthExceeded:
						return "Maximum character length of 128 is allowed for 'Name'";
					case CategoryWarningCode.DuplicateCategoryPath:
						return $"A Category with path, {CustomMessage}, already exists.";
					case CategoryWarningCode.CategoryPathLengthExceeded:
						return "The category path has exceeded the max length of 255 characters.";
					case CategoryWarningCode.CategoryDescriptionMaxLengthExceeded:
						return "Maximum character length of 65535 is allowed for 'Description'.";
					case CategoryWarningCode.CategoryDisplayOrderLessThanZero:
						return "'DisplayOrder' must be greater than zero (0).";
					case CategoryWarningCode.CategoryTagMaxLengthExceeded:
						return "Maximum character length of 1024 is allowed for 'Tag'.";
					case CategoryWarningCode.SelfParentCategory:
						return "Category cannot be its own Parent Category.";
					case CategoryWarningCode.PropertyNotFound:
						return $"Property, '{CustomMessage}', does not exist for the Category Entity.";
					case CategoryWarningCode.RestrictedProperty:
						return "You are trying to update a restricted property!";
					case CategoryWarningCode.CannotDeleteParentCategory:
						return "Cannot delete the Parent Category.";

					// Meta Data
					case CategoryWarningCode.CategoryNullMetaData:
						return "Category record does not have any Meta Data.";

					// SEO
					case CategoryWarningCode.NullCategorySeo:
						return "Cannot create a new 'Category' record with 'Seo' as a null object.";
					case CategoryWarningCode.NullCategorySeoTitle:
						return "SEO 'Title' cannot be empty. Please provide a value for SEO 'Title'.";
					case CategoryWarningCode.CategorySetSeo:
						return $"[Update SEO service error]: {CustomMessage}";
					case CategoryWarningCode.CategoryRemoveSeo:
						return $"[Remove SEO service error]: {CustomMessage}";

					// Sitemap
					case CategoryWarningCode.CategoryAddSitemap:
						return $"[Add Sitemap service error]: {CustomMessage}";
					case CategoryWarningCode.CategoryRemoveSitemap:
						return $"[Remove Sitemap service error]: {CustomMessage}";
					case CategoryWarningCode.CategorySetSitemap:
						return $"[Set Sitemap service error]: {CustomMessage}";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}