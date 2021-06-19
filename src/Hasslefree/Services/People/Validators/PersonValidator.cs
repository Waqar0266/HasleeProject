using FluentValidation;
using FluentValidation.Results;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Services.People.Warnings;
using System;

namespace Hasslefree.Services.People.Validators
{
	public class PersonValidator : AbstractValidator<Person>
	{
		private bool Create { get; }

		protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result)
		{
			if (context.InstanceToValidate != null) return true;

			if (Create) result.Errors.Add(new ValidationFailure("Person", "Cannot create a new 'Person' as a null object.")
			{
				ErrorCode = $"{PersonWarningCode.NullPerson}"
			});
			else result.Errors.Add(new ValidationFailure("Person", "Person record was not found.")
			{
				ErrorCode = $"{PersonWarningCode.PersonNotFound}"
			});

			return result.IsValid;
		}

		internal PersonValidator(bool create = false)
		{
			Create = create;

			RuleFor(a => a.Title)
				.NotNull().WithMessage("'Title' cannot be empty. Please provide a value for 'Title'.")
				.DependentRules(() => RuleFor(a => a.Title).Must(a => Enum.TryParse(a, false, out Titles _))
										.WithMessage("Invalid value for 'Title'. Please provide a valid value for 'Title'."))
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'Title'.");

			RuleFor(a => a.FirstName)
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'FirstName'.");

			RuleFor(a => a.Surname)
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'Surname'.");

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
		}
	}
}
