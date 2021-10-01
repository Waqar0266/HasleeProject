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
	public class UpdateExistingRentalService : IUpdateExistingRentalService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "ExistingRentalId", "CreatedOnUtc" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<ExistingRental> ExistingRentalRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private ExistingRental _existingRental;

		#endregion

		#region Constructor

		public UpdateExistingRentalService
		(
			IDataRepository<ExistingRental> existingRentalRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			ExistingRentalRepo = existingRentalRepo;

			// Other
			Database = database;
			Cache = cache;
		}

		#endregion

		#region IUpdateExistingRentalService

		public IUpdateExistingRentalService this[int existingRentalId]
		{
			get
			{
				if (existingRentalId <= 0)
					return this;

				_existingRental = ExistingRentalQuery(existingRentalId);

				return this;
			}
		}

		public IUpdateExistingRentalService Set<T>(Expression<Func<ExistingRental, T>> lambda, object value)
		{
			_existingRental?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_existingRental.ModifiedOn = DateTime.Now;
				ExistingRentalRepo.Edit(_existingRental);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			Cache.RemoveByPattern(CacheKeys.Server.ExistingRentals.Path);

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private ExistingRental ExistingRentalQuery(int existingRentalId)
		{
			var cFuture = (from c in ExistingRentalRepo.Table
						   where c.ExistingRentalId == existingRentalId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		#endregion
	}
}
