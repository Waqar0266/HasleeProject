using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Web.Models.Media.Pictures;

namespace Hasslefree.Web.Models.Catalog.Categories.Update
{
	/// <summary>
	/// Category update model
	/// </summary>
	[Validator(typeof(CategoryUpdateValidator))]
	public class CategoryUpdate
	{
		/// <summary>
		/// (Optional) Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// (Optional) Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// (Optional) Update default description
		/// </summary>
		public bool UpdateDefaultDescription { get; set; }

		/// <summary>
		/// (Optional) Display Order
		/// </summary>
		public int? DisplayOrder { get; set; }

		/// <summary>
		/// (Optional) Unique row identifier of the parent category record
		/// </summary>
		public int? ParentCategoryId { get; set; }

		/// <summary>
		/// (Optional) Update parent category Id
		/// </summary>
		public bool UpdateParentCategoryId { get; set; }

		/// <summary>
		/// (Optional) Indication of whether the category is hidden or not
		/// </summary>
		public bool? Hidden { get; set; }

		/// <summary>
		/// Picture
		/// </summary>
		public PictureModel Picture { get; set; }

		/// <summary>
		/// Update products linked to the category
		/// </summary>
		public CategoryProductsUpdate Products { get; set; }

		/// <summary>
		/// Indication of whether the picture should be removed
		/// </summary>
		public bool RemovePicture { get; set; }
	}

	public class CategoryUpdateValidator : AbstractValidator<CategoryUpdate>
	{
		public CategoryUpdateValidator()
		{
			When(m => m.Name != null, () =>
			{
				RuleFor(a => a.Name)
					.MinimumLength(3).WithMessage("'Name' must be a minimum of three (3) characters long.")
					.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for 'Name'");
			});

			When(m => m.Description != null, () =>
			{
				RuleFor(m => m.Description)
					.MaximumLength(1024).WithMessage("Maximum character length of 1024 is allowed for 'Description'");
			});

			When(m => m.DisplayOrder.HasValue, () =>
			{
				RuleFor(p => p.DisplayOrder)
					.GreaterThanOrEqualTo(0).WithMessage("'DisplayOrder' must be greater than zero (0).");
			});

			When(m => m.UpdateParentCategoryId, () =>
			{
				When(m => m.ParentCategoryId.HasValue,
					() => RuleFor(m => m.ParentCategoryId).GreaterThan(0).WithMessage("'ParentCategoryId' must be greater than zero (0)."));
			});
		}
	}
}