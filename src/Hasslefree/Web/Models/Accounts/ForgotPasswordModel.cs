using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Accounts
{
	[Validator(typeof(ForgotPasswordModelValidator))]
	public class ForgotPasswordModel
	{
		public string Email { get; set; }
	}

	/* Validator */
	public class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
	{
		public ForgotPasswordModelValidator()
		{
			RuleFor(r => r.Email)
				.NotEmpty()
				.EmailAddress()
				.WithMessage("The 'Email' field contains an invalid email address.");
		}
	}
}
