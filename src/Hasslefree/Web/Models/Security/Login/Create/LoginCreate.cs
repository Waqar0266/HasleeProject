using FluentValidation;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Security.Login.Create
{
	/// <summary>
	/// Login create model
	/// </summary>
	[Validator(typeof(LoginCreateValidator))]
	public class LoginCreate
	{
		/// <summary>
		/// Unique row identifier of the person record this login should be linked to
		/// </summary>
		public int PersonId { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Salutation
		/// </summary>
		public string Salutation { get; set; }

		/// <summary>
		/// Indication of whether the login record should be active or inactive
		/// </summary>
		public bool Active { get; set; }

		/// <summary>
		/// Password model
		/// </summary>
		public LoginPasswordModel Password { get; set; }

		/// <summary>
		/// List of unique row identifiers of the security groups the login should be linked to
		/// </summary>
		public List<int> SecurityGroups { get; set; }
	}

	/// <inheritdoc />
	public class LoginCreateValidator : AbstractValidator<LoginCreate>
	{
		/// <inheritdoc />
		public LoginCreateValidator()
		{
			RuleFor(m => m.PersonId)
				.GreaterThan(0).WithMessage("'PersonId' must be greater than 0.");

			RuleFor(m => m.Email)
				.NotNull().WithMessage("'Email' cannot be null. Please provide a value for 'Email'.")
				.EmailAddress().WithMessage("'Email' is not in a valid email address format.");

			RuleFor(a => a.SecurityGroups)
				.Must(a => a?.TrueForAll(b => b > 0) ?? true)
				.WithMessage("All 'SecurityGroup' identifiers must be greater than 0.");
		}
	}
}
