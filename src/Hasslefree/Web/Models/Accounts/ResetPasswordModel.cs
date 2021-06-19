using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Accounts
{
	[Validator(typeof(ResetPasswordModelValidator))]
	public class ResetPasswordModel
	{
		public string Email { get; set; }
		public string Hash { get; set; }
		public string Otp { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
	}

	/* Validator */
	public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
	{
		public ResetPasswordModelValidator()
		{
			CascadeMode = CascadeMode.StopOnFirstFailure;

			RuleFor(r => r.Email)
				.NotEmpty()
				.EmailAddress()
				.WithMessage("The email field is empty or contains an invalid value.");

			RuleFor(r => r.Otp)
				.NotEmpty()
				.WithMessage("The OTP field is empty or contains an invalid value.");

			RuleFor(m => m.Password)
				.NotEmpty()
				.WithMessage("The password field is empty.");

			RuleFor(m => m.ConfirmPassword)
				.NotEmpty()
				.Equal(m => m.Password)
				.WithMessage("Your passwords do not match.")
				.When(m => !string.IsNullOrEmpty(m.Password));
		}
	}
}
