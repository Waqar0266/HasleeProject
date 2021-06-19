using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using System;

namespace Hasslefree.Web.Models.People
{
	[Validator(typeof(PersonBaseValidator<PersonBaseModel>))]
	public class PersonBaseModel
	{
		public PersonBaseModel()
		{
			Title = Titles.Mr.ToString();
			Gender = Gender.Male;
			PersonStatus = PersonStatus.Enabled;
		}

		public int PersonId { get; set; }

		public DateTime? CreatedOnUtc { get; set; }
		public DateTime? ModifiedOnUtc { get; set; }
		public Guid PersonGuid { get; set; }

		public string Title { get; set; }
		public string FirstName { get; set; }
		public string MiddleNames { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string Alias { get; set; }
		public DateTime? Birthday { get; set; }

		[JsonIgnore]
		public Titles TitleEnum => Enum.TryParse(Title, false, out Titles title) ? title : Titles.Mr;

		[JsonIgnore]
		public Gender Gender
		{
			get => (Gender)Enum.Parse(typeof(Gender), GenderEnum);
			set => GenderEnum = value.ToString();
		}
		public string GenderEnum { get; set; }

		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Skype { get; set; }
		public string Website { get; set; }

		[JsonIgnore]
		public PersonStatus PersonStatus
		{
			get => (PersonStatus)Enum.Parse(typeof(PersonStatus), StatusEnum);
			set => StatusEnum = value.ToString();
		}
		public string StatusEnum { get; set; }

		public string Tag { get; set; }
	}

	public class PersonBaseValidator<T> : AbstractValidator<T> where T : PersonBaseModel
	{
		public PersonBaseValidator()
		{
			RuleFor(m => m.FirstName)
				.NotNull().WithMessage("'FirstName' cannot be empty. Please provide a value for 'FirstName'.")
				.MinimumLength(1).WithMessage("'FirstName' must be a minimum of one (1) character long.");

			RuleFor(m => m.Surname)
				.NotNull().WithMessage("'Surname' cannot be empty. Please provide a value for 'Surname'.")
				.MinimumLength(1).WithMessage("'Surname' must be a minimum of one (1) character long.");

			RuleFor(m => m.Email)
				.NotNull().WithMessage("'Email' cannot be empty. Please provide a value for 'Email'.")
				.EmailAddress().WithMessage("'Email' is not in a valid email address format.");

			RuleFor(m => m.Title)
				.NotNull().WithMessage("'Title' cannot be empty. Please provide a value for 'Title'.")
				.Must(a => Enum.TryParse(a, false, out Titles _)).WithMessage("Invalid value for 'Title'. Please provide a valid value for 'Title'.");

			RuleFor(m => m.Gender)
				.NotNull().WithMessage("'Gender' cannot be empty. Please provide a value for 'Gender'.")
				.Must(a => Enum.TryParse(a.ToString(), false, out Gender _)).WithMessage("Invalid value for 'Gender'. Please provide a valid value for 'Gender'.");

			RuleFor(m => m.StatusEnum)
				.NotNull().WithMessage("'Status' cannot be empty. Please provide a value for 'Status'.")
				.Must(a => Enum.TryParse(a.ToString(), false, out PersonStatus _)).WithMessage("Invalid value for 'Status'. Please provide a valid value for 'Status'.");
		}
	}
}