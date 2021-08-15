using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Transactions;

namespace Hasslefree.Services.Landlords.Crud
{
	public class CreateLandlordBankAccountService : ICreateLandlordBankAccountService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<LandlordBankAccount> LandlordBankAccountRepo { get; }

		#endregion

		#region Fields

		private LandlordBankAccount _landlordBankAccount;
		private int _rentalId;

		#endregion

		#region Constructor

		public CreateLandlordBankAccountService
		(
			IDataRepository<LandlordBankAccount> landlordBankAccountRepo
			)
		{
			// Repos
			LandlordBankAccountRepo = landlordBankAccountRepo;
		}

		#endregion

		#region ICreateLandlordBankAccountService

		public int LandlordBankAccountId { get; private set; }

		public ICreateLandlordBankAccountService WithRentalId(int rentalId)
		{
			_rentalId = rentalId;
			return this;
		}

		public ICreateLandlordBankAccountService New(string accountHolder, string bank, string branch, string branchCode, string accountNumber, string reference)
		{
			_landlordBankAccount = new LandlordBankAccount
			{
				AccountHolder = accountHolder,
				AccountNumber = accountNumber,
				Bank = bank,
				BankReference = reference,
				Branch = branch,
				BranchCode = branchCode,
				RentalId = _rentalId
			};

			return this;
		}

		public bool Create()
		{
			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				LandlordBankAccountRepo.Insert(_landlordBankAccount);

				scope.Complete();
			}

			// Set property object
			LandlordBankAccountId = _landlordBankAccount.LandlordBankAccountId;

			return true;
		}

		#endregion
	}
}
