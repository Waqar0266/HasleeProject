using Hasslefree.Web.Models.Rentals;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IGetExistingRentalService
	{
		IGetExistingRentalService this[int existingRentalId] { get; }
		IGetExistingRentalService this[string existingRentalGuid] { get; }
		ExistingRentalGet Get();
	}
}
