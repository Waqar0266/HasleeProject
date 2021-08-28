namespace Hasslefree.Services.Landlords.Crud
{
	public interface ICreateLandlordDocumentationService
	{
		ICreateLandlordDocumentationService Add(int rentalLandlordId, int downloadId);
		bool Process();
	}
}
