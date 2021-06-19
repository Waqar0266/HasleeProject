using System;

namespace Hasslefree.Web.Models.Security.Login.List
{
	/// <summary>
	/// Login listing item
	/// </summary>
	public class LoginListItem
	{
		/// <summary>
		/// Unique row identifier of the login record
		/// </summary>
		public int LoginId { get; set; }
		
		/// <summary>
		/// Unique row identifier of the person record that this login is linked to
		/// </summary>
		public int PersonId { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was last modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Salutation of the login
		/// </summary>
		public string Salutation { get; set; }
		
		/// <summary>
		/// Indication of whether the login is active or not
		/// </summary>
		public bool Active { get; set; }
	}
}
