using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Security.SecurityGroups
{
	[Validator(typeof(UpdateSecurityGroupModelValidator))]
	public class SecurityGroupUpdateModel
	{
		public string SecurityGroupName { get; set; }
		public string SecurityGroupDesc { get; set; }
	}

	public class UpdateSecurityGroupModelValidator : AbstractValidator<SecurityGroupUpdateModel>
	{
		public UpdateSecurityGroupModelValidator()
		{
			When(m => m.SecurityGroupName != null, () =>
			{
				RuleFor(m => m.SecurityGroupName)
					.MinimumLength(3)
					.WithMessage("'SecurityGroupName' must be a minimum of three (3) characters long.");
			});
		}
	}
}
