using System;

namespace Hasslefree.Web.Models.Security.Sessions.List
{
	/// <summary>
	/// Session listing item
	/// </summary>
	public class SessionListItem
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
		/// IP address
		/// </summary>
		public string IpAddress { get; set; }

		/// <summary>
		/// Latitude
		/// </summary>
		public double Latitude { get; set; }

		/// <summary>
		/// Longitude
		/// </summary>
		public double Longitude { get; set; }

		/// <summary>
		/// (Optional) Unique row identifier of the login record linked to the session
		/// </summary>
		public int? LoginId { get; set; }
		
		/// <summary>
		/// (Optional) UTC DateTime of when the session expires
		/// </summary>
		public DateTime? ExpiresOn { get; set; }
	}
}
