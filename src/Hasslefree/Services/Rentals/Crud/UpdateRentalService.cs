using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Rentals.Crud
{
    public class UpdateRentalService : IUpdateRentalService, IInstancePerRequest
    {
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalId", "CreatedOnUtc" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<Rental> RentalRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private Rental _rental;

		#endregion

		#region Constructor

		public UpdateRentalService
		(
			IDataRepository<Rental> rentalRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			RentalRepo = rentalRepo;

			// Other
			Database = database;
			Cache = cache;
		}

		#endregion

		#region IUpdateAgentService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<RentalWarning> Warnings { get; } = new List<RentalWarning>();

		public IUpdateRentalService this[int rentalId]
		{
			get
			{
				if (rentalId <= 0)
					return this;

				_rental = RentalQuery(rentalId);

				return this;
			}
		}

		public IUpdateRentalService Set<T>(Expression<Func<Rental, T>> lambda, object value)
		{
			_rental?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings)
				return false;

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_rental.ModifiedOn = DateTime.Now;
				RentalRepo.Edit(_rental);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			Cache.RemoveByPattern(CacheKeys.Server.Rentals.Path);

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private Rental RentalQuery(int rentalId)
		{
			var cFuture = (from c in RentalRepo.Table
						   where c.RentalId == rentalId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		private bool IsValid()
		{
			if (_rental == null)
			{
				Warnings.Add(new RentalWarning(RentalWarningCode.RentalNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		#endregion
	}
}
