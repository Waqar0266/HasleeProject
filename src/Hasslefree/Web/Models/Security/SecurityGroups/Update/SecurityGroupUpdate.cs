using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Update
{
	/// <summary>
	/// Security group update model
	/// </summary>
	[Validator(typeof(SecurityGroupUpdateValidator))]
	public class SecurityGroupUpdate
	{
		/// <summary>
		/// (Optional) New name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// (Optional) New description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Update permissions linked to the security group
		/// </summary>
		public SecurityGroupPermissionUpdate Permissions { get; set; }

		/// <summary>
		/// Update users linked to the security group
		/// </summary>
		public SecurityGroupUsersUpdate Users { get; set; }
	}

	public class SecurityGroupUpdateValidator : AbstractValidator<SecurityGroupUpdate>
	{
	}
}
