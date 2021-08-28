using Hasslefree.Web.Models.Rentals;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IGetRentalService
	{
		IGetRentalService this[int rentalId] { get; }
		IGetRentalService this[string rentalGuid] { get; }
		RentalGet Get();
	}
}
