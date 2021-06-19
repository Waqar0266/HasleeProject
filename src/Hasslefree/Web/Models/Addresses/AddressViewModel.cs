using FluentValidation;
using FluentValidation.Attributes;
using Hasslefree.Core.Domain.Common;

namespace Hasslefree.Web.Models.Addresses
{
	[Validator(typeof(AddressViewModelValidator))]
	public class AddressViewModel
	{
		public AddressViewModel()
		{
			Type = AddressType.Residential;
		}

		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string City { get; set; }
		public int CountryId { get; set; }
		public string Country { get; set; }
		public string RegionName { get; set; }
		public string Code { get; set; }
		public int PersonId { get; set; }
		public int AddressId { get; set; }
		public AddressType Type { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
	}

	public class AddressViewModelValidator : AbstractValidator<AddressViewModel>
	{
		public AddressViewModelValidator()
		{
			RuleFor(m => m.Address1)
					.NotEmpty().WithMessage("'Address 1' may not be empty")
					.MaximumLength(128).WithMessage("'Address 1' may not exceed 128 characters");
			RuleFor(m => m.Address2)
				.MaximumLength(128).WithMessage("'Address 1' may not exceed 128 characters");
			RuleFor(m => m.Address3)
					.NotEmpty().WithMessage("'Suburb' may not be empty")
					.MaximumLength(128).WithMessage("'Suburb' may not exceed 128 characters");
			RuleFor(m => m.City)
					.NotEmpty().WithMessage("'City / Town' may not be empty")
					.MaximumLength(64).WithMessage("'City / Town' may not exceed 64 characters");
			RuleFor(m => m.RegionName)
					.NotEmpty().WithMessage("'Province / State' may not be empty")
					.MaximumLength(32).WithMessage("'Province / State' may not exceed 32 characters");
			RuleFor(m => m.Code)
					.NotEmpty().WithMessage("'Postal Code' may not be empty")
					.MaximumLength(24).WithMessage("'Postal Code' may not exceed 24 characters");
			RuleFor(m => m.CountryId).NotEmpty().WithMessage("'Country' may not be empty");
		}
	}
}