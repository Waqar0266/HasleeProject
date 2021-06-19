using System;

namespace Hasslefree.Web.Models.Security.Sessions.Get
{
	/// <summary>
	/// Session get model
	/// </summary>
	public class SessionGet
	{
		/// <summary>
		/// Unique row identifier of the session record
		/// </summary>
		public int SessionId { get; set; }

		/// <summary>
		/// UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// UTC DateTime of when last the record was modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Unique reference
		/// </summary>
		public string Reference { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the session expires
		/// </summary>
		public DateTime? ExpiresOn { get; set; }

		/// <summary>
		/// Quote currency
		/// </summary>
		public string QuoteCurrency { get; set; }

		/// <summary>
		/// Exchange rate
		/// </summary>
		public decimal ExchangeRate { get; set; }

		/// <summary>
		/// Location where the session is accessed from
		/// </summary>
		public SessionLocation Location { get; set; }

		/// <summary>
		/// Account linked to the session
		/// </summary>
		public SessionAccount Account { get; set; }

		/// <summary>
		/// User linked to the session
		/// </summary>
		public SessionUser User { get; set; }

		/// <summary>
		/// Cart linked to the session
		/// </summary>
		public SessionCart Cart { get; set; }
	}
}
