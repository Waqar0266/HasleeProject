using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Services.Rentals.Crud
{
    public interface ICreateRentalService
    {
        bool HasWarnings { get; }
        List<RentalWarning> Warnings { get; }
        int RentalId { get; }
        List<RentalLandlord> Landlords { get; }

        ICreateRentalService New(RentalType type, LeaseType leaseType, string premises, string standErf);
        ICreateRentalService WithLandlord(string idNumber, string name, string surname, string email, string mobile);
        ICreateRentalService WithAgentId(int agentId);
        bool Create();
    }
}
