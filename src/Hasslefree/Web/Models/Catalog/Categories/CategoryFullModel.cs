using FluentValidation.Attributes;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Web.Models.Media.Pictures;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Catalog.Categories
{
	/// <inheritdoc />
	/// <summary>
	/// Extended details of a category
	/// </summary>
	[Validator(typeof(CategoryFullValidator))]
	public class CategoryFullModel : CategoryBaseModel
	{
		public CategoryFullModel()
		{
		}

		/// <summary>
		/// Picture
		/// </summary>
		public PictureModel Picture { get; set; }
	}

	/// <inheritdoc />
	public class CategoryFullValidator : CategoryBaseValidator<CategoryFullModel>
	{
	}
}