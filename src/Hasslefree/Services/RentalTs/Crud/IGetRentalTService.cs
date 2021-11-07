using Hasslefree.Web.Models.RentalTs;

namespace Hasslefree.Services.RentalTs.Crud
{
	public interface IGetRentalTService
	{
		IGetRentalTService this[int rentalTId] { get; }
		IGetRentalTService this[string rentalTGuid] { get; }
		RentalTGet Get();
	}
}
