using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Services.RentalTs.Crud
{
	public interface ICreateRentalTService
	{
        bool HasWarnings { get; }
        List<RentalTWarning> Warnings { get; }
        int RentalTId { get; }
        List<Tenant> Tenants { get; }

        ICreateRentalTService New(int rentalId, RentalTType type);
        ICreateRentalTService WithTenant(string idNumber, string name, string surname, string email, string mobile);
        bool Create();
    }
}
