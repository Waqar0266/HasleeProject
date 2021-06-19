using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Core.Domain.Agents;

namespace Hasslefree.Web.Models.Agents
{
	[Validator(typeof(AgentCreateValidator))]
	public class AgentCreate
	{
		public int AgentId { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string IdNumber { get; set; }
		public AgentType AgentType { get; set; }
	}

	public class AgentCreateValidator : AbstractValidator<AgentCreate>
	{
		public AgentCreateValidator()
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

			RuleFor(m => m.Mobile)
			.NotEmpty()
			.WithMessage("Please enter a 'Mobile'.");

			RuleFor(m => m.IdNumber)
			.NotEmpty()
			.WithMessage("Please enter an 'ID Number'.");

			RuleFor(m => m.Email)
				.NotEmpty()
				.WithMessage("Please enter an 'Email'.")
				.EmailAddress()
				.WithMessage("'Email' is not a valid email address.");
		}
	}
}
