using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using System;

namespace Hasslefree.Web.Models.Security.Sessions.Get
{
	/// <summary>
	/// Account model linked to a session
	/// </summary>
	public class SessionAccount
	{
		/// <summary>
		/// Unique row identifier of the account being used
		/// </summary>
		public int AccountId { get; set; }

		/// <summary>
		/// Name of the account
		/// </summary>
		public string AccountName { get; set; }
	}
}
