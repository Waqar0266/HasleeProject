using Hasslefree.Web.Models.Security.Login.Get.Person;
using Hasslefree.Web.Models.Security.Login.Get.SecurityGroups;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Login.Get
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
		/// (Optional) UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was last modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

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

		/// <summary>
		/// Person this login record belongs to
		/// </summary>
		public PersonGet Person { get; set; }

		/// <summary>
		/// List of security groups linked to the login
		/// </summary>
		public List<SecurityGroupGet> SecurityGroups { get; set; }
	}
}
