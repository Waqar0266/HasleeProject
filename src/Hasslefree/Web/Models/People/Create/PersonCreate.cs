using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Web.Models.Security.Login;
using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.People.Create
{
	/// <summary>
	/// Person create model
	/// </summary>
	[Validator(typeof(PersonCreateValidator))]
	public class PersonCreate
	{
		/// <summary>String representation of the Titles enum [case sensitive]
		/// - Mr
		/// - Mrs
		/// - Miss
		/// - Dr
		/// - Prof
		/// - Other
		/// </summary>
		public string TitleEnum { get; set; }

		/// <summary>
		/// </summary>
		[JsonIgnore]
		public Titles Title
		{
			get
			{
				if (Enum.TryParse(TitleEnum, out Titles titles)) return titles;
				TitleEnum = string.IsNullOrWhiteSpace(TitleEnum) ? null : "invalid";
				return Titles.Other;
			}
			set => TitleEnum = value.ToString();
		}

		/// <summary>
		/// First name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Middle names, space separated
		/// </summary>
		public string MiddleNames { get; set; }

		/// <summary>
		/// Surname 
		/// </summary>
		public string Surname { get; set; }

		/// <summary>
		/// Email address
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// (Optional) Alias 
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		/// (Optional) Birthday
		/// </summary>
		public DateTime? Birthday { get; set; }

		/// <summary>String representation of the Gender enum [case sensitive]
		/// - Male
		/// - Female
		/// - Other
		/// - Unspecified
		/// </summary>
		public string GenderEnum { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[JsonIgnore]
		public Gender Gender
		{
			get => Enum.TryParse(GenderEnum, out Gender gender) ? gender : Gender.Male;
			set => GenderEnum = value.ToString();
		}

		/// <summary>
		/// Phone
		/// </summary>
		public string Phone { get; set; }

		/// <summary>
		/// Fax
		/// </summary>
		public string Fax { get; set; }

		/// <summary>
		/// Mobile
		/// </summary>
		public string Mobile { get; set; }

		/// <summary>
		/// Skype
		/// </summary>
		public string Skype { get; set; }

		/// <summary>
		/// Website
		/// </summary>
		public string Website { get; set; }

		/// <summary>String representation of the PersonStatus enum [case sensitive]
		/// - Enabled
		/// - Disabled
		/// - Pending
		/// - Deleted
		/// </summary>
		public string PersonStatusEnum { get; set; }

		/// <summary>
		/// </summary>
		[JsonIgnore]
		public PersonStatus PersonStatus
		{
			get => Enum.TryParse(PersonStatusEnum, out PersonStatus status) ? status : PersonStatus.Enabled;
			set => PersonStatusEnum = value.ToString();
		}

		/// <summary>
		/// Comma (,) separated tags
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// Indication if an account should be created along with the person
		/// </summary>
		public bool CreateAccount { get; set; }

		/// <summary>
		/// (Optional) Login password model 
		/// </summary>
		public LoginPasswordModel Password { get; set; }
	}

	/// <inheritdoc />
	public class PersonCreateValidator : AbstractValidator<PersonCreate>
	{
		/// <inheritdoc />
		public PersonCreateValidator()
		{
			RuleFor(a => a.TitleEnum)
				.NotNull().WithMessage("'TitleEnum' cannot be empty. Please provide a value for 'TitleEnum'.")
				.DependentRules(() => RuleFor(a => a.TitleEnum).Must(a => Enum.TryParse(a, false, out Titles _))
										.WithMessage("Invalid value for 'TitleEnum'. Please provide a valid value for 'TitleEnum'."))
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'TitleEnum'.");

			RuleFor(a => a.FirstName)
				.NotNull().WithMessage("'FirstName' cannot be empty. Please provide a value for 'FirstName'.")
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'FirstName'.");

			RuleFor(a => a.MiddleNames)
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'MiddleNames'.");

			RuleFor(a => a.Surname)
				.NotNull().WithMessage("'Surname' cannot be empty. Please provide a value for 'Surname'.")
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'Surname'.");

			RuleFor(a => a.Alias)
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'Alias'.");

			RuleFor(a => a.Phone)
				.MaximumLength(16).WithMessage("Maximum character length of 16 is allowed for 'Phone'.");

			RuleFor(a => a.Fax)
				.MaximumLength(16).WithMessage("Maximum character length of 16 is allowed for 'Fax'.");

			RuleFor(a => a.Mobile)
				.MaximumLength(16).WithMessage("Maximum character length of 16 is allowed for 'Mobile'.");

			RuleFor(a => a.Email)
				.NotNull().WithMessage("'Email' cannot be empty. Please provide a value for 'Email'.")
				.EmailAddress().WithMessage("'Email' is not in a valid email address format.")
				.MaximumLength(64).WithMessage("Maximum character length of 64 is allowed for 'Email'.");

			RuleFor(a => a.Skype)
				.MaximumLength(24).WithMessage("Maximum character length of 24 is allowed for 'Skype'.");

			RuleFor(a => a.Website)
				.MaximumLength(64).WithMessage("Maximum character length of 64 is allowed for 'Website'.");

			RuleFor(a => a.GenderEnum)
				.NotEmpty().WithMessage("'GenderEnum' cannot be empty. Please provide a value for 'GenderEnum'.")
				.DependentRules(() => RuleFor(a => a.GenderEnum).Must(a => Enum.TryParse(a, false, out Gender _))
										.WithMessage("Invalid value for 'GenderEnum'. Please provide a valid value for 'GenderEnum'."))
				.MaximumLength(64).WithMessage("Maximum character length of 16 is allowed for 'GenderEnum'.");

			RuleFor(a => a.PersonStatusEnum)
				.NotEmpty().WithMessage("'PersonStatusEnum' cannot be empty. Please provide a value for 'PersonStatusEnum'.")
				.DependentRules(() => RuleFor(a => a.PersonStatusEnum).Must(a => Enum.TryParse(a, false, out PersonStatus _))
										.WithMessage("Invalid value for 'PersonStatusEnum'. Please provide a valid value for 'PersonStatusEnum'."))
				.MaximumLength(64).WithMessage("Maximum character length of 16 is allowed for 'PersonStatusEnum'.");

			RuleFor(a => a.Tag)
				.MaximumLength(1024).WithMessage("Maximum character length of 1024 is allowed for 'Tag'.");
		}
	}
}
