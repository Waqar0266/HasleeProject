using FluentValidation;
using Nop.Plugin.Widgets.HassleFree.Models.Agents;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.HassleFree.Validators
{
    public class AgentValidator : BaseNopValidator<AgentViewModel>
    {
        public AgentValidator()
        {
            //login by email
            RuleFor(x => x.Name).NotEmpty().WithMessage("The agent name is required");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("The agent surname is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("The agent email address is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("The agent email address must be valid");
            RuleFor(x => x.IdNumber).NotEmpty().WithMessage("The agent ID number is required");
            RuleFor(x => x.Mobile).NotEmpty().WithMessage("The agent cellphone number is required");
            RuleFor(x => x.Province).NotEmpty().WithMessage("The agent province is required");
        }
    }
}
