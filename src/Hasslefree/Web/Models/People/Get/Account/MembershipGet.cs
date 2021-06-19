using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using System;

namespace Hasslefree.Web.Models.People.Get.Account
{
	/// <summary>
	/// Account and membership model
	/// </summary>
	public class MembershipGet
	{
		/// <summary>
		/// Unique identifier of the account record
		/// </summary>
		public int AccountId { get; set; }

		/// <summary>
		/// UTC DateTime of when the person became a member of the account
		/// </summary>
		public DateTime? CreatedOnUtc { get; set; }

		/// <summary>
		/// Name of the account
		/// </summary>
		public string AccountName { get; set; }

		/// <summary>String representation of the AccountType enum [case sensitive]
		/// - Individual
		/// - Corporate
		/// </summary>
		public string AccountTypeEnum { get; set; }

		/// <summary>
		/// Budget allocated for this account [only applicable to "Corporate"]
		/// </summary>
		public decimal Budget { get; set; }
	}
}
