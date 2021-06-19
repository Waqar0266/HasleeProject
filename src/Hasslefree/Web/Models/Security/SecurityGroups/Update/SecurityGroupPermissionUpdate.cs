using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Update
{
	/// <summary>
	/// Update permissions linked to a security group
	/// </summary>
	[Validator(typeof(SecurityGroupPermissionUpdateValidator))]
	public class SecurityGroupPermissionUpdate
	{
		/// <summary>
		/// List of new permissions (permission ids) that need to be linked to the security group
		/// </summary>
		public List<int> Add { get; set; }

		/// <summary>
		/// List of permissions (permission ids) that need to be unlinked from the security group
		/// </summary>
		public List<int> Remove { get; set; }
	}

	public class SecurityGroupPermissionUpdateValidator : AbstractValidator<SecurityGroupPermissionUpdate>
	{
		public SecurityGroupPermissionUpdateValidator()
		{
			RuleFor(a => a.Add)
				.Must(a => a?.TrueForAll(b => b > 0) ?? true).WithMessage("All new 'Permissions' must be greater than zero (0).");
		}
	}
}
