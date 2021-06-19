using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Accounts
{
	[Validator(typeof(LoginModelValidator))]
	public class LoginModel
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}

	public class LoginModelValidator : AbstractValidator<LoginModel>
	{
		public LoginModelValidator()
		{
			CascadeMode = CascadeMode.StopOnFirstFailure;

			RuleFor(m => m.Email)
				.NotEmpty()
				.WithMessage("Please enter an 'Email'.")
				.EmailAddress()
				.WithMessage("'Email' is not a valid email address.");
				
			RuleFor(m => m.Password)
				.NotEmpty()
				.WithMessage("Please enter a 'Password'.");
		}
	}
}