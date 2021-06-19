using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using System;

namespace Hasslefree.Web.Models.People.List
{
	/// <summary>
	/// Persons listing item
	/// </summary>
	public class PersonListItem
	{
		/// <summary>
		/// Unique row identifier of the person record
		/// </summary>
		public int PersonId { get; set; }

		/// <summary>
		/// (Optional) Unique row identifier of the login record
		/// </summary>
		public int? LoginId { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// (Optional) UTC DateTime of when the record was last modified
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Unique Guid generated
		/// </summary>
		public Guid PersonGuid { get; set; }
		
		/// <summary>
		/// Surname
		/// </summary>
		public string Surname { get; set; }

		/// <summary>
		/// Initials
		/// </summary>
		public string Initials { get; set; }

		/// <summary>
		/// First name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>String representation of thee PersonStatus enum [case sensitive]
		/// - Enabled
		/// - Disabled
		/// - Pending
		/// - Deleted
		/// </summary>
		public string PersonStatusEnum { get; set; }

		/// <summary>
		/// </summary>
		[JsonIgnore]
		public PersonStatus PersonStatus
		{
			get => (PersonStatus)Enum.Parse(typeof(PersonStatus), PersonStatusEnum);
			set => PersonStatusEnum = value.ToString();
		}

		/// <summary>
		/// Total accounts linked to this person
		/// </summary>
		public int TotalMemberships { get; set; }

		/// <summary>
		/// Indication of whether person has admin privileges or not
		/// </summary>
		public bool HasAdminPrivileges { get; set; }
	}
}
