using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface ICreateExistingRentalService
	{
        bool HasWarnings { get; }
        List<RentalWarning> Warnings { get; }
        int ExistingRentalId { get; }

        ICreateExistingRentalService New(int rentalId, ExistingRentalType type);
        bool Create();
    }
}
