using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Hasslefree.Web.Models.Common;
using System;

namespace Hasslefree.Web.Models.Security.SecurityGroups
{
	[Validator(typeof(SecurityGroupBaseValidator<SecurityGroupBaseModel>))]
	public class SecurityGroupBaseModel
	{
		public int SecurityGroupId { get; set; }

		public DateTime? CreatedOnUtc { get; set; }
		public DateTime? ModifiedOnUtc { get; set; }

		public string SecurityGroupName { get; set; }
		public string SecurityGroupDesc { get; set; }
		public bool IsSystemSecurityGroup { get; set; }

		public int TotalMembers { get; set; }
		public int TotalPermissions { get; set; }

		[JsonIgnore]
		public CrudAction Action { get; set; }
	}

	public class SecurityGroupBaseValidator<T> : AbstractValidator<T> where T : SecurityGroupBaseModel
	{
		public SecurityGroupBaseValidator()
		{
			RuleFor(a => a.SecurityGroupName)
				.NotNull().WithMessage("'SecurityGroupName' cannot be empty. Please provide a value for 'SecurityGroupName'.")
				.MinimumLength(3).WithMessage("'SecurityGroupName' must be a minimum of three (3) characters long.");

			RuleFor(a => a.SecurityGroupDesc)
				.MaximumLength(1024).WithMessage("Maximum character length of 1024 is allowed for 'SecurityGroupDesc'");
		}
	}
}
