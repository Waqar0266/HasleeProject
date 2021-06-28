using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Agents
{
	[Validator(typeof(CompleteAgentEaabValidator))]
	public class CompleteAgentEaab
	{
		public string AgentGuid { get; set; }
		public int DownloadId { get; set; }
	}

	public class CompleteAgentEaabValidator : AbstractValidator<CompleteAgentEaab>
	{
		public CompleteAgentEaabValidator()
		{
			RuleFor(m => m.DownloadId)
				.GreaterThan(0)
				.WithMessage("Please upload your 'EAAB Proof of Registration'.");
		}
	}
}
