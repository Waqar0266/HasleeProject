using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using System;

namespace Hasslefree.Web.Models.People.Update
{
	/// <summary>
	/// Person update model
	/// </summary>
	public class PersonUpdate
	{
		/// <summary>(Optional) String representation of the Titles enum [case sensitive]
		/// - Mr
		/// - Mrs
		/// - Miss
		/// - Dr
		/// - Prof
		/// - Other
		/// </summary>
		public string TitleEnum { get; set; }

		/// <summary></summary>
		[JsonIgnore]
		public Titles? Title
		{
			get => TitleEnum == null ? null : Enum.TryParse(TitleEnum, out Titles titles) ? titles : (Titles?)null;
			set => TitleEnum = value.ToString();
		}

		/// <summary>
		/// (Optional) First name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// (Optional) Middle names, space separated
		/// </summary>
		public string MiddleNames { get; set; }

		/// <summary>
		/// (Optional) Surname 
		/// </summary>
		public string Surname { get; set; }

		/// <summary>
		/// (Optional) Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// (Optional) Alias 
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		/// Indication of whether to update birthday or not
		/// </summary>
		public bool UpdateBirthday { get; set; }
		/// <summary>
		/// (Optional) Birthday
		/// </summary>
		public DateTime? Birthday { get; set; }

		/// <summary>(Optional) String representation of the Gender enum [case sensitive]
		/// - Male
		/// - Female
		/// - Other
		/// - Unspecified
		/// </summary>
		public string GenderEnum { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonIgnore]
		public Gender? Gender
		{
			get => GenderEnum == null ? null : Enum.TryParse(GenderEnum, out Gender gender) ? gender : (Gender?)null;
			set => GenderEnum = value.ToString();
		}

		/// <summary>
		/// (Optional) Phone
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// (Optional) Fax
		/// </summary>
		public string Fax { get; set; }

		/// <summary>
		/// (Optional) Mobile
		/// </summary>
		public string Mobile { get; set; }

		/// <summary>
		/// (Optional) Skype
		/// </summary>
		public string Skype { get; set; }

		/// <summary>
		/// (Optional) Website
		/// </summary>
		public string Website { get; set; }

		/// <summary>(Optional) String representation of the PersonStatus enum [case sensitive]
		/// - Enabled
		/// - Disabled
		/// - Pending
		/// - Deleted
		/// </summary>
		public string PersonStatusEnum { get; set; }

		/// <summary>
		/// </summary>
		[JsonIgnore]
		public PersonStatus? PersonStatus
		{
			get => PersonStatusEnum == null ? null : Enum.TryParse(PersonStatusEnum, out PersonStatus status) ? status : (PersonStatus?) null;
			set => PersonStatusEnum = value.ToString();
		}

		/// <summary>
		/// (Optional) Comma (,) separated tags
		/// </summary>
		public string Tag { get; set; }
	}
}
