using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Security
{
	public class SecurityGroup : BaseEntity
	{
		public SecurityGroup()
		{
			CreatedOn = ModifiedOn = DateTime.Now;
			IsSystemSecurityGroup = false;
			SecurityGroupLogins = new HashSet<SecurityGroupLogin>();
			Permissions = new HashSet<Permission>();
		}

		public int SecurityGroupId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string SecurityGroupName { get; set; }
		public string SecurityGroupDesc { get; set; }
		public bool IsSystemSecurityGroup { get; set; }

		public ICollection<SecurityGroupLogin> SecurityGroupLogins { get; set; }
		public ICollection<Permission> Permissions { get; set; }
	}
}
