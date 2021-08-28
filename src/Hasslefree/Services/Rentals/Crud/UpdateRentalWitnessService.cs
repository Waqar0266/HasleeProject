using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Rentals.Crud
{
	public class UpdateRentalWitnessService : IUpdateRentalWitnessService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalWitnessId", "CreatedOn" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<RentalWitness> RentalWitnessRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private RentalWitness _rentalWitness;
		private int _rentalId;

		#endregion

		#region Constructor

		public UpdateRentalWitnessService
		(
			IDataRepository<RentalWitness> rentalWitnessRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			RentalWitnessRepo = rentalWitnessRepo;

			// Other
			Database = database;
			Cache = cache;
		}

		#endregion

		#region IUpdateRentalWitnessService

		public int RentalWitnessId { get; internal set; }

		public IUpdateRentalWitnessService WithRentalId(int rentalId)
		{
			_rentalId = rentalId;
			return this;
		}

		public IUpdateRentalWitnessService this[int rentalWitnessId]
		{
			get
			{
				if (rentalWitnessId <= 0)
				{
					_rentalWitness = new RentalWitness()
					{
						RentalId = _rentalId,
						WitnessStatus = WitnessStatus.Pending
					};
					RentalWitnessRepo.Insert(_rentalWitness);
					return this;
				}


				_rentalWitness = Query(rentalWitnessId);

				return this;
			}
		}

		public IUpdateRentalWitnessService Set<T>(Expression<Func<RentalWitness, T>> lambda, object value)
		{
			_rentalWitness?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_rentalWitness.RentalId = _rentalId;
				_rentalWitness.ModifiedOn = DateTime.Now;
				RentalWitnessRepo.Edit(_rentalWitness);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();

				RentalWitnessId = _rentalWitness.RentalWitnessId;
			}

			Cache.RemoveByPattern(CacheKeys.Server.Rentals.Path);

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private RentalWitness Query(int rentalWitnessId)
		{
			var cFuture = (from c in RentalWitnessRepo.Table
						   where c.RentalWitnessId == rentalWitnessId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		#endregion
	}
}
