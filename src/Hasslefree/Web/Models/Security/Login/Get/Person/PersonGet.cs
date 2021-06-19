using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using System;

namespace Hasslefree.Web.Models.Security.Login.Get.Person
{
	/// <summary>
	/// Person a login is linked to
	/// </summary>
	public class PersonGet
	{
		/// <summary>
		/// Unique row identifier of the person record
		/// </summary>
		public int PersonId { get; set; }

		/// <summary>
		/// First name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Surname
		/// </summary>
		public string Surname { get; set; }

		/// <summary>String representation of the PersonStatus enum [case sensitive]
		/// - Enabled,
		/// - Disabled
		/// - Pending,
		/// - Deleted
		/// </summary>
		public string PersonStatusEnum { get; set; }

		/// <summary></summary>
		[JsonIgnore]
		public PersonStatus PersonStatus
		{
			get => (PersonStatus)Enum.Parse(typeof(PersonStatus), PersonStatusEnum);
			set => PersonStatusEnum = value.ToString();
		}
	}
}
