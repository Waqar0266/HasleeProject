using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.SecurityGroups.Create
{
	/// <summary>
	/// SecurityGroup create model
	/// </summary>
	[Validator(typeof(SecurityGroupCreateValidator))]
	public class SecurityGroupCreate
	{
		/// <summary>
		/// Unique group name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// List of permissions (permission ids) to link to the group
		/// </summary>
		public List<int> Permissions { get; set; }

		/// <summary>
		/// List of users (login ids) to link to the group
		/// </summary>
		public List<int> Users { get; set; }
	}

	/// <inheritdoc/>
	public class SecurityGroupCreateValidator : AbstractValidator<SecurityGroupCreate>
	{
		/// <inheritdoc/>
		public SecurityGroupCreateValidator()
		{
			RuleFor(a => a.Name)
				.NotNull().WithMessage("'Name' cannot be empty. Please provide a value for 'Name'.");

			RuleFor(a => a.Permissions)
				.Must(a => a?.TrueForAll(b => b > 0) ?? true).WithMessage("All 'Permissions' must be greater than zero (0).");

			RuleFor(a => a.Users)
				.Must(a => a?.TrueForAll(b => b > 0) ?? true).WithMessage("All 'Users' must be greater than zero (0).");
		}
	}
}
