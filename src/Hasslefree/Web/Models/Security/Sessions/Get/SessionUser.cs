namespace Hasslefree.Web.Models.Security.Sessions.Get
{
	/// <summary>
	/// User linked to a session
	/// </summary>
	public class SessionUser
	{
		/// <summary>
		/// Unique row identifier of the login record used to log in
		/// </summary>
		public int LoginId { get; set; }

		/// <summary>
		/// (Optional) Unique row identifier of the person record 
		/// </summary>
		public int? PersonId { get; set; }

		/// <summary>
		/// (Optional) Full name of the person
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		public string Email { get; set; }
	}
}
