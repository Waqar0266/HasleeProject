using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Update
{
	/// <summary>
	/// Update users linked to a security group
	/// </summary>
	[Validator(typeof(SecurityGroupUserUpdateValidator))]
	public class SecurityGroupUsersUpdate
	{
		/// <summary>
		/// List of new users (login ids) that need to be linked to the security group
		/// </summary>
		public List<int> Add { get; set; }

		/// <summary>
		/// List of users (login ids) that need to be unlinked from the security group
		/// </summary>
		public List<int> Remove { get; set; }
	}

	public class SecurityGroupUserUpdateValidator : AbstractValidator<SecurityGroupUsersUpdate>
	{
		public SecurityGroupUserUpdateValidator()
		{
			RuleFor(a => a.Add)
				.Must(a => a?.TrueForAll(b => b > 0) ?? true).WithMessage("All new 'Users' must be greater than zero (0).");
		}
	}
}
