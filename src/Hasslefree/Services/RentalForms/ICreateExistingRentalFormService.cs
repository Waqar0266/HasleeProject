using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Services.RentalForms
{
	public interface ICreateExistingRentalFormService
	{
		bool HasWarnings { get; }
		List<RentalFormWarning> Warnings { get; }
		int ExistingRentalFormId { get; }

		ICreateExistingRentalFormService New(ExistingRentalFormName existingRentalFormName, int existingRentalId, int dowloadId);
		bool Create();
	}
}
