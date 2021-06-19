using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Web.Models.Media.Pictures;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Catalog.Categories.Create
{
	[Validator(typeof(CategoryCreateValidator))]
	public class CategoryCreate
	{
		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Indication of whether the category is hidden or not
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// Comma (,) separated list of tags
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Unique row identifier of the parent category record
		/// </summary>
		public int? ParentCategoryId { get; set; }

		/// <summary>
		/// Display order
		/// </summary>
		public int DisplayOrder { get; set; }

		/// <summary>
		/// Picture model
		/// </summary>
		public PictureModel Picture { get; set; }

		/// <summary>
		/// List of products to be linked to the category
		/// </summary>
		public CategoryProductLinks Products { get; set; }
	}

	/// <inheritdoc />
	public class CategoryCreateValidator : AbstractValidator<CategoryCreate>
	{
		/// <inheritdoc />
		public CategoryCreateValidator()
		{
			RuleFor(a => a.Name)
				.NotNull().WithMessage("'Name' cannot be empty. Please provide a value for 'Name'.")
				.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for 'Name'.");

			RuleFor(p => p.DisplayOrder)
				.GreaterThanOrEqualTo(0).WithMessage("'DisplayOrder' must be greater than zero (0).");

			When(m => m.ParentCategoryId.HasValue,
				() => RuleFor(m => m.ParentCategoryId)
					.GreaterThan(0).WithMessage("'ParentCategoryId' must be greater than zero (0)."));

			RuleFor(m => m.Description)
				.MaximumLength(65535).WithMessage("Maximum character length of 65535 is allowed for 'Description'.");
		}
	}
}
