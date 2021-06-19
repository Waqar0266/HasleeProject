using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Catalog.Categories.Create
{
	/// <summary>
	/// Linked products to a category
	/// </summary>
	[Validator(typeof(CategoryProductCreateValidator))]
	public class CategoryProductLinks
	{
		/// <summary>
		/// List of product unique row identifiers
		/// </summary>
		public List<int> ProductIds { get; set; }

		/// <summary>
		/// List of product unique SKU codes
		/// </summary>
		public List<string> Skus { get; set; }
	}

	public class CategoryProductCreateValidator : AbstractValidator<CategoryProductLinks>
	{
	}
}
