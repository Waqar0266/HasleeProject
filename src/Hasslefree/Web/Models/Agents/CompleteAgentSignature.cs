using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Agents
{
	[Validator(typeof(CompleteAgentSignatureValidator))]
	public class CompleteAgentSignature
	{
		public string AgentGuid { get; set; }
		public string Signature { get; set; }
		public string Initials { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
	}

	public class CompleteAgentSignatureValidator : AbstractValidator<CompleteAgentSignature>
	{
		public CompleteAgentSignatureValidator()
		{
			RuleFor(m => m.Signature)
				.NotEmpty()
				.WithMessage("Please provide your 'Full Signature'.");

			RuleFor(m => m.Initials)
				.NotEmpty()
				.WithMessage("Please provide your 'Initial Signature'.");
		}
	}
}
