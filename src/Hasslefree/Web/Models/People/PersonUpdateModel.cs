using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Results;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using System;
using static System.String;

namespace Hasslefree.Web.Models.People
{
	[Validator(typeof(PersonUpdateValidator))]
	public class PersonUpdateModel
	{
		public string Title { get; set; }
		public string FirstName { get; set; }
		public string MiddleNames { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string Alias { get; set; }
		public DateTime? Birthday { get; set; }
		public Gender? Gender { get; set; }

		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Skype { get; set; }
		public string Website { get; set; }

		public PersonStatus? Status { get; set; }
		public string Tag { get; set; }
	}

	public class PersonUpdateValidator : AbstractValidator<PersonUpdateModel>
	{
		public new virtual ValidationResult Validate(PersonUpdateModel instance)
		{
			return instance == null
				? new ValidationResult(new[] { new ValidationFailure("Request", "No values were submitted.") })
				: base.Validate(instance);
		}

		public PersonUpdateValidator()
		{
			When(m => m.FirstName != null, () =>
				RuleFor(m => m.FirstName).MinimumLength(1).WithMessage("'FirstName' must be a minimum of one (1) character long."));

			When(m => m.FirstName != null, () =>
				RuleFor(m => m.Surname).MinimumLength(1).WithMessage("'Surname' must be a minimum of one (1) character long."));

			When(m => m.Email != null,
				() => RuleFor(m => m.Email).EmailAddress().WithMessage("'Email' is not in a valid format."));

			When(m => !IsNullOrWhiteSpace(m.Title), () =>
				RuleFor(m => m.Title).Must(t => Enum.TryParse(t, false, out Titles _)).WithMessage("Invalid value for 'Title'. Please enter a valid value for 'Title'."));

			When(m => m.Gender.HasValue, () =>
				RuleFor(m => m.Gender).Must(g => !g.HasValue || Enum.TryParse(g.Value.ToString(), false, out Titles _)).WithMessage("Invalid value for 'Gender'. Please enter a valid value for 'Gender'."));

			When(m => m.Status.HasValue, () =>
				RuleFor(m => m.Gender).Must(s => !s.HasValue || Enum.TryParse(s.Value.ToString(), false, out Titles _)).WithMessage("Invalid value for 'Status'. Please enter a valid value for 'Status'."));
		}
	}
}