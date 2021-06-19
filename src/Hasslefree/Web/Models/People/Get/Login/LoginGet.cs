namespace Hasslefree.Web.Models.People.Get.Login
{
	/// <summary>
	/// Login get model
	/// </summary>
	public class LoginGet
	{
		/// <summary>
		/// Unique row identifier of the login record
		/// </summary>
		public int LoginId { get; set; }

		/// <summary>
		/// Email address used to login
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Alias displayed once logged in
		/// </summary>
		public string Salutation { get; set; }

		/// <summary>
		/// Indication of whether the login is active or not
		/// </summary>
		public bool Active { get; set; }
	}
}
