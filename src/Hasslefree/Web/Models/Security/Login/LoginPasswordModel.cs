using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Security.Login
{
	/// <summary>
	/// Password reset model
	/// </summary>
	[Validator(typeof(LoginPasswordResetValidator))]
	public class LoginPasswordModel
	{
		/// <summary>
		/// Password
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Confirmed password
		/// </summary>
		public string ConfirmPassword { get; set; }

		/// <summary>
		/// (Optional) Custom salt to be used 
		/// </summary>
		public string PasswordSalt { get; set; }
	}

	/// <inheritdoc />
	public class LoginPasswordResetValidator : AbstractValidator<LoginPasswordModel>
	{
		/// <inheritdoc />
		public LoginPasswordResetValidator()
		{
			RuleFor(m => m.Password)
				.NotNull().WithMessage("'Password' cannot be null. Please provide a value for 'Password'.")
				.MinimumLength(6).WithMessage("'Password' must be a minimum of six (6) characters long.");

			RuleFor(m => m.ConfirmPassword)
				.NotNull().WithMessage("'ConfirmPassword' cannot be null. Please provide a value for 'ConfirmPassword'.");
			When(m => m.ConfirmPassword != null,
				() => RuleFor(m => m.ConfirmPassword).Equal(m => m.Password).WithMessage("Your passwords do not match.").When(m => !string.IsNullOrEmpty(m.Password)));
		}
	}
}
