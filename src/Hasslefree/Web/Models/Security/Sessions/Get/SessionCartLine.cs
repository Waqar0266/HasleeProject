using Newtonsoft.Json;
using Hasslefree.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Sessions.Get
{
	public class SessionCartLine
	{
		/// <summary>
		/// Unique row identifier of the cart line record
		/// </summary>
		public int LineId { get; set; }

		/// <summary>
		/// UTC DateTime of when the item was added to teh cart
		/// </summary>
		public DateTime? CreatedOnUtc { get; set; }

		/// <summary>
		/// Display order
		/// </summary>
		public int? DisplayOrder { get; set; }

		/// <summary>
		/// Unique row identifier of the parent line record that this line is linked to
		/// </summary>
		public int? ParentLineId { get; set; }

		/// <summary>
		/// Unique row identifier of the product record this line is linked to
		/// </summary>
		public int? ProductId { get; set; }

		/// <summary>
		/// SKU of the product
		/// </summary>
		public string Sku { get; set; }

		/// <summary>
		/// Name of the product
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Quantity of the product
		/// </summary>
		public decimal? Qty { get; set; }

		/// <summary>
		/// Discount
		/// </summary>
		public decimal? Discount { get; set; }

		/// <summary>
		/// Unit price of the product excluding tax
		/// </summary>
		public decimal? UnitPriceExcl { get; set; }

		/// <summary>
		/// Unit price of the product including tax
		/// </summary>
		public decimal? UnitPriceIncl { get; set; }

		/// <summary>
		/// Total, excluding tax, before deductions
		/// </summary>
		public decimal? SubTotalExcl { get; set; }

		/// <summary>
		/// Total, including tax, before deductions
		/// </summary>
		public decimal? SubTotalIncl { get; set; }

		/// <summary>
		/// Tax percentage
		/// </summary>
		public decimal? TaxPercentage { get; set; }

		/// <summary>
		/// Tax amount
		/// </summary>
		public decimal? TaxAmount { get; set; }

		/// <summary>
		/// Indication of whether the cart line is a gift or not
		/// </summary>
		public bool? IsGift { get; set; }

		/// <summary>
		/// Indication of whether the cart line is an enquiry or not
		/// </summary>
		public bool? IsEnquiry { get; set; }

		/// <summary>
		/// Cart lines linked to this line
		/// </summary>
		public List<SessionCartLine> Children { get; set; }
	}
}
