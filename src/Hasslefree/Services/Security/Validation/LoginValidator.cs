using FluentValidation;
using FluentValidation.Results;
using Hasslefree.Services.Security.Login;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Validation
{
	internal class LoginValidator : AbstractValidator<LoginDb>
	{
		private bool Create { get; }

		protected override bool PreValidate(ValidationContext<LoginDb> context, ValidationResult result)
		{
			if (context.InstanceToValidate != null) return true;

			if (Create) result.Errors.Add(new ValidationFailure("Login", "Cannot create a new 'Login' as a null object.")
			{
				ErrorCode = $"{LoginWarningCode.NullLogin}"
			});
			else result.Errors.Add(new ValidationFailure("Login", "Login record was not found.")
			{
				ErrorCode = $"{LoginWarningCode.LoginNotFound}"
			});

			return false;
		}

		internal LoginValidator(bool create = false)
		{
			Create = create;

			RuleFor(a => a.PersonId)
				.GreaterThan(0).WithMessage("Person record was not found");

			RuleFor(a => a.Email)
				.NotNull().WithMessage("'Email' cannot be empty. Please provide a value for 'Email'.")
				.EmailAddress().WithMessage("'Email' is not in a valid email address format.")
				.MaximumLength(64).WithMessage("Maximum character length of 64 is allowed for 'Email'.");

			RuleFor(a => a.Password)
				.NotEmpty().WithMessage("'Password' cannot be empty. Please provide a value for 'Password'.")
				.MinimumLength(6).WithMessage("'Password' must be a minimum of six (6) characters long.")
				.MaximumLength(255).WithMessage("Maximum character length of 255 is allowed for 'Password'.");

			RuleFor(a => a.PasswordSalt)
				.NotEmpty().WithMessage("'PasswordSalt' cannot be empty. Please provide a value for 'PasswordSalt'.")
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'PasswordSalt'");
		}
	}
}
