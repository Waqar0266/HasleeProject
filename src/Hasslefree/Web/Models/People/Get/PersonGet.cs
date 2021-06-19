using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Web.Models.People.Get.Account;
using Hasslefree.Web.Models.People.Get.Login;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.People.Get
{
	/// <summary>
	/// Person get model
	/// </summary>
	public class PersonGet
	{
		/// <summary>
		/// Unique row identifier of the person record
		/// </summary>
		public int PersonId { get; set; }

		/// <summary>
		/// UTC DateTime of when the person was created
		/// </summary>
		public DateTime? CreatedOn { get; set; }

		/// <summary>
		/// UTC DateTime of when the person's details were last updated
		/// </summary>
		public DateTime? ModifiedOn { get; set; }

		/// <summary>
		/// Unique GUID for person
		/// </summary>
		public Guid PersonGuid { get; set; }

		/// <summary>String representation of the Titles enum [case sensitive]
		/// - Mr
		/// - Mrs
		/// - Miss
		/// - Dr
		/// - Prof
		/// - Other
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// </summary>
		[JsonIgnore]
		public Titles TitleEnum
		{
			get => Enum.TryParse(Title, false, out Titles title) ? title : Titles.Other;
			set => Title = value.ToString();
		}

		/// <summary>
		/// First name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Surname
		/// </summary>
		public string Surname { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// (Optional) Birthday
		/// </summary>
		public DateTime? Birthday { get; set; }

		/// <summary>String representation of the Gender enum [case sensitive]
		/// - Male
		/// - Female
		/// - Other
		/// - Unspecified
		/// </summary>
		public string GenderEnum { get; set; }

		/// <summary>
		/// </summary>
		[JsonIgnore]
		public Gender Gender
		{
			get => (Gender)Enum.Parse(typeof(Gender), GenderEnum);
			set => GenderEnum = value.ToString();
		}

		/// <summary>
		/// Phone
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// Fax
		/// </summary>
		public string Fax { get; set; }

		/// <summary>
		/// Mobile
		/// </summary>
		public string Mobile { get; set; }

		/// <summary>String representation of the PersonStatus enum [case sensitive]
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
		/// Indication of whether person has admin privileges or not
		/// </summary>
		public bool HasAdminPrivileges { get; set; }

		/// <summary>
		/// Login details for person
		/// </summary>
		public LoginGet Login { get; set; }

		/// <summary>
		/// List of accounts the person is a member of
		/// </summary>
		public List<MembershipGet> Memberships { get; set; }
	}
}
