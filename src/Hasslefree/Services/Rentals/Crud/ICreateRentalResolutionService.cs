using System;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface ICreateRentalResolutionService
	{
		int RentalResolutionId { get; }
		ICreateRentalResolutionService WithRentalId(int rentalId);

		ICreateRentalResolutionService New(string heldAt, DateTime heldOn, string leaseName, string authorizedName, string authorizedSurname);
		ICreateRentalResolutionService WithMember(string name, string surname, string email, string idNumber);

		bool Create(bool saveChanges = true);
	}
}
