using FluentValidation;
using FluentValidation.Attributes;

namespace Hasslefree.Web.Models.Common
{
	[Validator(typeof(FirmModelValidator))]
	public class FirmModel
	{
		public string BusinessName { get; set; }
		public string TradeName { get; set; }
		public string Phone { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public string ReferenceNumber { get; set; }
		public string AiNumber { get; set; }

		public string PhysicalAddress1 { get; set; }
		public string PhysicalAddress2 { get; set; }
		public string PhysicalAddress3 { get; set; }
		public string PhysicalAddressTown { get; set; }
		public string PhysicalAddressCode { get; set; }
		public string PhysicalAddressCountry { get; set; }
		public string PhysicalAddressRegion { get; set; }

		public string PostalAddress1 { get; set; }
		public string PostalAddress2 { get; set; }
		public string PostalAddress3 { get; set; }
		public string PostalAddressTown { get; set; }
		public string PostalAddressCode { get; set; }
		public string PostalAddressCountry { get; set; }
		public string PostalAddressRegion { get; set; }
	}

	public class FirmModelValidator : AbstractValidator<FirmModel>
	{
		/// <inheritdoc />
		public FirmModelValidator()
		{
			RuleFor(a => a.BusinessName)
				.NotNull().WithMessage("'Business Name' cannot be empty. Please provide a value for 'Business Name'.")
				.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for 'Business Name'.");

			RuleFor(a => a.TradeName)
			.NotNull().WithMessage("'Trade Name' cannot be empty. Please provide a value for 'Trade Name'.")
			.MaximumLength(128).WithMessage("Maximum character length of 128 is allowed for 'Trade Name'.");

			RuleFor(a => a.Phone)
				.NotNull().WithMessage("'Phone' cannot be empty. Please provide a value for 'Phone'.")
				.MaximumLength(32).WithMessage("Maximum character length of 32 is allowed for 'Phone'.");
		}
	}
}
