using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Agents
{
	[Validator(typeof(CompleteDocumentationValidator))]
	public class CompleteDocumentation
	{
		public string AgentGuid { get; set; }
		public string UploadIds { get; set; }
	}

	public class CompleteDocumentationValidator : AbstractValidator<CompleteDocumentation>
	{
		public CompleteDocumentationValidator()
		{
			RuleFor(m => m.UploadIds)
				.NotEmpty()
				.WithMessage("Please upload your documentation");
		}
	}
}
