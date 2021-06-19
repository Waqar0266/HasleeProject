namespace Hasslefree.Web.Models.Security.Login.Get.SecurityGroups
{
	/// <summary>
	/// Security group linked to a login
	/// </summary>
	public class SecurityGroupGet
	{
		/// <summary>
		/// Unique row identifier of the security group record
		/// </summary>
		public int SecurityGroupId { get; set; }

		/// <summary>
		/// Name of the group
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description of the group
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Indication of whether the group is a system group
		/// </summary>
		public bool IsSystemGroup { get; set; }
	}
}
