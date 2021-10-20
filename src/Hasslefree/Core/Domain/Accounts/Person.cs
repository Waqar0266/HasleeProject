using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Security;
using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Accounts
{
	[Serializable]
	public class Person : BaseEntity
	{
		public Person()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
			Gender = Gender.Male;
			PersonGuid = Guid.NewGuid();
			PersonStatus = PersonStatus.Enabled;
			Logins = new HashSet<Login>();
		}

		public int PersonId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public Guid PersonGuid { get; set; }
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string IdNumber { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string GenderEnum { get; set; }
		public string PersonStatusEnum { get; set; }

		public Gender Gender
		{
			get => (Gender)Enum.Parse(typeof(Gender), GenderEnum);
			set => GenderEnum = value.ToString();
		}

		public PersonStatus PersonStatus
		{
			get => (PersonStatus)Enum.Parse(typeof(PersonStatus), PersonStatusEnum);
			set => PersonStatusEnum = value.ToString();
		}

		public ICollection<Login> Logins { get; set; }
	}
}
