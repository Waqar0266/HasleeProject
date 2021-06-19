using System;
using System.Collections.Generic;
using Hasslefree.Core.Domain.Accounts;

namespace Hasslefree.Core.Domain.Security
{
	public class Login : BaseEntity
	{
		public Login()
		{
			CreatedOn = ModifiedOn = DateTime.Now;
			Active = true;
			SecurityGroupLogins = new HashSet<SecurityGroupLogin>();
			Sessions = new HashSet<Session>();
		}

		public int LoginId { get; set; }
		public int PersonId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string PasswordSalt { get; set; }
		public string Salutation { get; set; }
		public bool Active { get; set; }

		// Navigation Properties
		public ICollection<SecurityGroupLogin> SecurityGroupLogins { get; set; }
		public ICollection<Session> Sessions { get; set; }
		public Person Person { get; set; }
	}
}
