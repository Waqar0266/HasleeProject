using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Web.Models.Security.Login.Update.SecurityGroups;

namespace Hasslefree.Web.Models.Security.Login.Update
{
	/// <summary>
	/// Login update model
	/// </summary>
	[Validator(typeof(LoginUpdateValidator))]
	public class LoginUpdate
	{
		/// <summary>
		/// (Optional) Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// (Optional) Salutation
		/// </summary>
		public string Salutation { get; set; }

		/// <summary>
		/// (Optional) Indication of whether the login is active or inactive
		/// </summary>
		public bool? Active { get; set; }

		/// <summary>
		/// (Optional) Password
		/// </summary>
		public LoginPasswordModel Password { get; set; }

		/// <summary>
		/// (Optional) Update which security groups the login is linked to 
		/// </summary>
		public SecurityGroupsUpdate SecurityGroups { get; set; }
	}

	/// <inheritdoc />
	public class LoginUpdateValidator : AbstractValidator<LoginUpdate>
	{
		/// <inheritdoc />
		public LoginUpdateValidator()
		{
			When(m => m.Email != null, () =>
			{
				RuleFor(m => m.Email)
					.EmailAddress().WithMessage("'Email' is not in a valid format.");
			});
		}
	}
}
