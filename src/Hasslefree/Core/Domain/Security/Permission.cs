using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Domain.Security
{
	public class Permission : BaseEntity
	{
		public Permission()
		{
			CreatedOn = ModifiedOn = DateTime.Now;
			SecurityGroups = new HashSet<SecurityGroup>();
		}

		public int PermissionId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string PermissionUniqueName { get; set; }
		public string PermissionDisplayName { get; set; }
		public string PermissionDescription { get; set; }
		public string PermissionGroupName { get; set; }

		// Navigation Properties
		public ICollection<SecurityGroup> SecurityGroups { get; set; }
	}
}
