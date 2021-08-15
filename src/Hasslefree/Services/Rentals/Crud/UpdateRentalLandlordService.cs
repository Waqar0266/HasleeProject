using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Rentals.Crud
{
	public class UpdateRentalLandlordService : IUpdateRentalLandlordService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalLandlordId", "CreatedOn" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<RentalLandlord> RentalLandlordRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private RentalLandlord _rentalLandlord;

		#endregion

		#region Constructor

		public UpdateRentalLandlordService
		(
			IDataRepository<RentalLandlord> rentalLandlordRepo,
			IDataContext database
		)
		{
			// Repos
			RentalLandlordRepo = rentalLandlordRepo;

			// Other
			Database = database;
		}

		#endregion

		#region IUpdateRentalLandlordService

		public IUpdateRentalLandlordService this[int rentalLandlordId]
		{
			get
			{
				if (rentalLandlordId <= 0)
					return this;

				_rentalLandlord = RentalLandlordQuery(rentalLandlordId);

				return this;
			}
		}

		public IUpdateRentalLandlordService Set<T>(Expression<Func<RentalLandlord, T>> lambda, object value)
		{
			_rentalLandlord?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_rentalLandlord.ModifiedOn = DateTime.Now;
				RentalLandlordRepo.Edit(_rentalLandlord);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private RentalLandlord RentalLandlordQuery(int rentalLandlordId)
		{
			var cFuture = (from c in RentalLandlordRepo.Table
						   where c.RentalLandlordId == rentalLandlordId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}
		
		#endregion
	}
}
