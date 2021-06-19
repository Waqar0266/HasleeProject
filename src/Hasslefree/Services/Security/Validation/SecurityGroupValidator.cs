using FluentValidation;
using FluentValidation.Results;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Services.Security.Groups;

namespace Hasslefree.Services.Security.Validation
{
	internal class SecurityGroupValidator : AbstractValidator<SecurityGroup>
	{
		private bool Create { get; }

		protected override bool PreValidate(ValidationContext<SecurityGroup> context, ValidationResult result)
		{
			if (context.InstanceToValidate != null) return true;

			if (Create) result.Errors.Add(new ValidationFailure("SecurityGroup", "Cannot create a new 'SecurityGroup' as a null object.")
			{
				ErrorCode = $"{SecurityGroupWarningCode.NullGroup}"
			});
			else result.Errors.Add(new ValidationFailure("SecurityGroup", "Security Group record was not found.")
			{
				ErrorCode = $"{SecurityGroupWarningCode.GroupNotFound}"
			});

			return false;
		}

		internal SecurityGroupValidator(bool create = false)
		{
			Create = create;

			RuleFor(a => a.SecurityGroupName)
				.NotNull().WithMessage("'Name' cannot be empty. Please provide a value for 'Name'.");
		}
	}
}
