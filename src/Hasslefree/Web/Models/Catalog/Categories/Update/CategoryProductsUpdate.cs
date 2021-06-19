using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Web.Models.Catalog.Categories.Create;

namespace Hasslefree.Web.Models.Catalog.Categories.Update
{
	/// <summary>
	/// Update products linked to a category
	/// </summary>
	[Validator(typeof(CategoryProductsUpdateValidator))]
	public class CategoryProductsUpdate
	{
		/// <summary>
		/// Link products to the category
		/// </summary>
		public CategoryProductLinks Add { get; set; }

		/// <summary>
		/// Unlink products from a category
		/// </summary>
		public CategoryProductLinks Remove { get; set; }
	}

	public class CategoryProductsUpdateValidator : AbstractValidator<CategoryProductsUpdate>
	{
	}
}
