using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Services.RentalForms
{
	public interface ICreateRentalFormService
	{
		bool HasWarnings { get; }
		List<RentalFormWarning> Warnings { get; }
		int RentalFormId { get; }

		ICreateRentalFormService New(RentalFormName rentalFormName, int rentalId, int dowloadId);
		bool Create();
	}
}
