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
	public class UpdateRentalMandateService : IUpdateRentalMandateService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalMandateId", "CreatedOn" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<RentalMandate> RentalMandateRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private RentalMandate _rentalMandate;
		private int _rentalId;

		#endregion

		#region Constructor

		public UpdateRentalMandateService
		(
			IDataRepository<RentalMandate> rentalMandateRepo,
			IDataContext database
		)
		{
			// Repos
			RentalMandateRepo = rentalMandateRepo;

			// Other
			Database = database;
		}

		#endregion

		#region IUpdateAgentService

		public IUpdateRentalMandateService WithRentalId(int rentalId)
		{
			_rentalId = rentalId;
			return this;
		}

		public IUpdateRentalMandateService this[int rentalMandateId]
		{
			get
			{
				if (rentalMandateId <= 0)
				{
					_rentalMandate = new RentalMandate()
					{
						RentalId = _rentalId
					};
					RentalMandateRepo.Insert(_rentalMandate);
					return this;
				}


				_rentalMandate = Query(rentalMandateId);

				return this;
			}
		}

		public IUpdateRentalMandateService Set<T>(Expression<Func<RentalMandate, T>> lambda, object value)
		{
			_rentalMandate?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_rentalMandate.ModifiedOn = DateTime.Now;
				_rentalMandate.RentalId = _rentalId;
				RentalMandateRepo.Edit(_rentalMandate);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private RentalMandate Query(int rentalMandateId)
		{
			var cFuture = (from c in RentalMandateRepo.Table
						   where c.RentalMandateId == rentalMandateId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		#endregion
	}
}
