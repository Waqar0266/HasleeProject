using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
using System;

namespace Hasslefree.Web.Models.Agents
{
	[Validator(typeof(CompleteAgentValidator))]
	public class CompleteAgent
	{
		public string AgentGuid { get; set; }
		public int AgentId { get; set; }
		public AgentStatus AgentStatus { get; set; }

		public string Title { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public Gender Gender { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }

		//Agent specifics
		public string IdNumber { get; set; }
		public string Nationality { get; set; }
		public string Race { get; set; }
		public string PreviousEmployer { get; set; }
		public string FfcNumber { get; set; }
		public DateTime? FfcIssueDate { get; set; }
		public string EaabReference { get; set; }
		public bool Dismissed { get; set; }
		public bool Convicted { get; set; }
		public bool Insolvent { get; set; }
		public bool Withdrawn { get; set; }

		//Residential Address
		public string ResidentialAddress1 { get; set; }
		public string ResidentialAddress2 { get; set; }
		public string ResidentialAddress3 { get; set; }
		public string ResidentialAddressTown { get; set; }
		public string ResidentialAddressCode { get; set; }
		public string ResidentialAddressCountry { get; set; }
		public string ResidentialAddressProvince { get; set; }

		//Postal Address
		public string PostalAddress1 { get; set; }
		public string PostalAddress2 { get; set; }
		public string PostalAddress3 { get; set; }
		public string PostalAddressTown { get; set; }
		public string PostalAddressCode { get; set; }
		public string PostalAddressCountry { get; set; }
		public string PostalAddressProvince { get; set; }
	}

	public class CompleteAgentValidator : AbstractValidator<CompleteAgent>
	{
		public CompleteAgentValidator()
		{
			RuleFor(m => m.Title)
				.NotEmpty()
				.WithMessage("Please select a 'Title'.");

			RuleFor(m => m.Name)
				.NotEmpty()
				.WithMessage("Please enter a 'Name'.");

			RuleFor(m => m.Surname)
				.NotEmpty()
				.WithMessage("Please enter a 'Surname'.");

			RuleFor(m => m.Race)
				.NotEmpty()
				.WithMessage("Please select a 'Race'.");

			RuleFor(m => m.IdNumber)
				.NotEmpty()
				.WithMessage("Please enter an 'ID Number'.");

			RuleFor(m => m.Nationality)
				.NotEmpty()
				.WithMessage("Please select a 'Nationality'.");

			RuleFor(m => m.Mobile)
				.NotEmpty()
				.WithMessage("Please enter a 'Mobile Number'.");

			RuleFor(m => m.Email)
				.NotEmpty()
				.WithMessage("Please enter an 'Email'.")
				.EmailAddress()
				.WithMessage("'Email' is not a valid email address.");

			RuleFor(m => m.Password)
				.NotEmpty()
				.WithMessage("Please enter a 'Password'.");

			RuleFor(m => m.ConfirmPassword)
				.NotEmpty()
				.WithMessage("Please enter a 'Confirm Password'.")
				.Equal(a => a.Password)
				.WithMessage("Your passwords does not match");

			//Addresses
			RuleFor(m => m.ResidentialAddress1)
				.NotEmpty()
				.WithMessage("Please enter a 'Residential Address'.");

			RuleFor(m => m.ResidentialAddressTown)
				.NotEmpty()
				.WithMessage("Please enter a 'Residential Address City'.");

			RuleFor(m => m.ResidentialAddressProvince)
				.NotEmpty()
				.WithMessage("Please enter a 'Residential Address Province'.");

			RuleFor(m => m.ResidentialAddressCountry)
				.NotEmpty()
				.WithMessage("Please enter a 'Residential Address Country'.");

			RuleFor(m => m.ResidentialAddressCode)
				.NotEmpty()
				.WithMessage("Please enter a 'Residential Address Code'.");

			RuleFor(m => m.PostalAddress1)
				.NotEmpty()
				.WithMessage("Please enter a 'Postal Address'.");

			RuleFor(m => m.PostalAddressTown)
				.NotEmpty()
				.WithMessage("Please enter a 'Postal Address City'.");

			RuleFor(m => m.PostalAddressProvince)
				.NotEmpty()
				.WithMessage("Please enter a 'Postal Address Province'.");

			RuleFor(m => m.PostalAddressCountry)
				.NotEmpty()
				.WithMessage("Please enter a 'Postal Address Country'.");

			RuleFor(m => m.PostalAddressCode)
				.NotEmpty()
				.WithMessage("Please enter a 'Postal Address Code'.");
		}
	}
}
