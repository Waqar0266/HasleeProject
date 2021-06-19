
namespace Hasslefree.Core.Domain.Security
{
	public class SecurityGroupLogin : BaseEntity
	{
		public int LoginId { get; set; }
		public int SecurityGroupId { get; set; }

		// Navigation Properties
		public Login Login { get; set; }
		public SecurityGroup SecurityGroup { get; set; }
	}
}
