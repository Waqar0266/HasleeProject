using System;

namespace Hasslefree.Web.Models.Emails
{
	/// <summary>
	/// The CRUD model for the email
	/// </summary>
	public class EmailCrud
	{
		/// <summary>
		/// The unique identifier of the email
		/// </summary>
		public Int32 Id { get;set; }

		public String Title { get; set; }

		/// <summary>
		/// How the email is built (None (don't send the email), Url (send from a generated page, default), Template (use the template and scriban))
		/// </summary>
		public String Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Boolean Send { get; set; }
		
		/// <summary>
		/// The URL to query for the email when sending via Url
		/// </summary>
		public String Url { get; set; }

		/// <summary>
		/// The "From" name for the email
		/// </summary>
		public String From { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public String FromAddress { get; set; }

		/// <summary>
		/// The subject of the email
		/// </summary>
		public String Subject { get; set; }

		/// <summary>
		/// The recipient of the email
		/// </summary>
		public String Recipient { get; set; }

		/// <summary>
		/// The full template of the email when sending via Template, compiled with scriban
		/// </summary>
		public String Template { get; set; }

		/// <summary>
		/// The model for the email
		/// </summary>
		public Object Model { get; set; }

		
	}
}
