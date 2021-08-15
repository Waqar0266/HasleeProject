namespace Hasslefree.Services.Landlords.Crud
{
	public interface ICreateLandlordBankAccountService
	{
		int LandlordBankAccountId { get; }

		ICreateLandlordBankAccountService New(string accountHolder, string bank, string branch, string branchCode, string accountNumber, string reference);
		ICreateLandlordBankAccountService WithRentalId(int rentalId);
		bool Create();
	}
}
