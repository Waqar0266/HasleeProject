using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using System;

namespace Hasslefree.Web.Models.Catalog.Categories
{
	/// <summary>
	/// Category base model
	/// </summary>
	[Validator(typeof(CategoryBaseValidator<CategoryBaseModel>))]
	public class CategoryBaseModel
	{
		/// <summary>
		/// Unique row identifier
		/// </summary>
		[JsonProperty(Order = -2)]
		public int CategoryId { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was created
		/// </summary>
		[JsonProperty(Order = -2)]
		public DateTime? CreatedOnUtc { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was last modified
		/// </summary>
		[JsonProperty(Order = -2)]
		public DateTime? ModifiedOnUtc { get; set; }

		/// <summary>
		/// Unique category path
		/// </summary>
		[JsonProperty(Order = -2)]
		public string Path { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[JsonProperty(Order = -2)]
		public string Name { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		[JsonProperty(Order = -2)]
		public string Description { get; set; }

		/// <summary>
		/// Indication of whether to update default description
		/// </summary>
		[JsonIgnore]
		public bool UpdateDefaultDescription { get; set; }

		/// <summary>
		/// Nest leveled
		/// </summary>
		[JsonProperty(Order = -2)]
		public int NestedLevel { get; set; }

		/// <summary>
		/// Display order
		/// </summary>
		[JsonProperty(Order = -2)]
		public int DisplayOrder { get; set; }

		/// <summary>
		/// (Optional) Unique row identifier of the parent category
		/// </summary>
		[JsonProperty(Order = -2)]
		public int? ParentCategoryId { get; set; }
		[JsonIgnore]
		public bool UpdateParentCategoryId { get; set; }

		/// <summary>
		/// Indication of whether the category should be hidden or not
		/// </summary>
		[JsonProperty(Order = -2)]
		public bool Hidden { get; set; }

		/// <summary>
		/// Tags
		/// </summary>
		[JsonProperty(Order = -2)]
		public string Tag { get; set; }

		/// <summary>
		/// Total number of products linked to the category
		/// </summary>
		[JsonProperty(Order = -2)]
		public int ProductCount { get; set; }

		// Picture, only to be used by BackOffice controller, not API
		[JsonIgnore]
		public string PictureUrl { get; set; }
	}

	/// <inheritdoc />
	public class CategoryBaseValidator<T> : AbstractValidator<T> where T : CategoryBaseModel
	{
		/// <inheritdoc />
		public CategoryBaseValidator()
		{
			RuleFor(a => a.Name)
				.NotNull().WithMessage("'Name' cannot be empty. Please provide a value for 'Name'.")
				.MinimumLength(3).WithMessage("'Name' must be a minimum of three (3) characters long.")
				.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for 'Name'.");

			RuleFor(p => p.DisplayOrder)
				.GreaterThanOrEqualTo(0).WithMessage("'DisplayOrder' must be greater than zero (0).");

			When(m => m.ParentCategoryId.HasValue,
				() => RuleFor(m => m.ParentCategoryId)
					.GreaterThan(0).WithMessage("'ParentCategoryId' must be greater than zero (0)."));

			When(m => m.UpdateParentCategoryId, () =>
			{
				When(m => m.ParentCategoryId.HasValue,
					() => RuleFor(m => m.ParentCategoryId)
						.GreaterThan(0).WithMessage("'ParentCategoryId' must be greater than zero (0)."));
			});

			RuleFor(m => m.Description)
				.MaximumLength(65535).WithMessage("Maximum character length of 65535 is allowed for 'Description'.");
		}
	}
}

