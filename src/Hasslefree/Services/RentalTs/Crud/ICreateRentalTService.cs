using Hasslefree.Core.Domain.Rentals;
using System.Collections.Generic;

namespace Hasslefree.Services.RentalTs.Crud
{
	public interface ICreateRentalTService
	{
        bool HasWarnings { get; }
        List<RentalTWarning> Warnings { get; }
        int RentalTId { get; }

        ICreateRentalTService New(LeaseType leaseType, string premises, string standErf, string address, string township);
        ICreateRentalTService WithLandlord(string idNumber, string name, string surname, string email, string mobile);
        ICreateRentalTService WithAgentId(int agentId);
        bool Create();
    }
}
