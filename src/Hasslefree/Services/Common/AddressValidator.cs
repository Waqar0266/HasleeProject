using FluentValidation;

namespace Hasslefree.Services.Common
{
	internal class AddressValidator : AbstractValidator<Core.Domain.Common.Address>
	{
		internal AddressValidator()
		{
			RuleFor(a => a.Country)
				.MaximumLength(64).WithMessage("Maximum character length of 64 is allowed for Address 'Country'.");

			RuleFor(a => a.Address1)
				.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for Address 'Address1'.");

			RuleFor(a => a.Address2)
				.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for Address 'Address2'.");

			RuleFor(a => a.Address3)
				.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for Address 'Address3'.");

			RuleFor(a => a.Town)
				.MaximumLength(64).WithMessage("Maximum character length of 64 is allowed for Address 'Town'.");

			RuleFor(a => a.Code)
				.MaximumLength(24).WithMessage("Maximum character length of 24 is allowed for Address 'Code'.");

			RuleFor(a => a.RegionName)
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for Address 'RegionName'.");

			RuleFor(a => a.Latitude)
				.MaximumLength(24).WithMessage("Maximum character length of 24 is allowed for Address 'Latitude'.");

			RuleFor(a => a.Longitude)
				.MaximumLength(24).WithMessage("Maximum character length of 24 is allowed for Address 'Longitude'.");
		}
	}
}
