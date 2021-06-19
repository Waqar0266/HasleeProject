using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Accounts
{
	[Validator(typeof(ChangePasswordModelValidator))]
	public class ChangePasswordModel
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }
	}

	public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
	{
		public ChangePasswordModelValidator()
		{
			RuleFor(m => m.OldPassword)
				.NotEmpty().WithMessage("'Current Password' may not be empty.");

			RuleFor(m => m.NewPassword)
				.NotEmpty().WithMessage("'New Password' may not be empty.")
				.Length(6, 32).WithMessage("Passwords have to be at least 6 characters.");

			RuleFor(m => m.ConfirmPassword)
				.NotEmpty().WithMessage("'Confirm Password' may not be empty.")
				.Equal(m => m.NewPassword).WithMessage("Passwords do not match.");
		}
	}
}
