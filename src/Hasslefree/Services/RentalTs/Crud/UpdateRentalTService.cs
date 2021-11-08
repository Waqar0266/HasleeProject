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

namespace Hasslefree.Services.RentalTs.Crud
{
    public class UpdateRentalTService : IUpdateRentalTService, IInstancePerRequest
    {
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalTId", "CreatedOnUtc" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<RentalT> RentalTRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private RentalT _rentalT;

		#endregion

		#region Constructor

		public UpdateRentalTService
		(
			IDataRepository<RentalT> rentalTRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			RentalTRepo = rentalTRepo;

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

		public List<RentalTWarning> Warnings { get; } = new List<RentalTWarning>();

		public IUpdateRentalTService this[int rentalTId]
		{
			get
			{
				if (rentalTId <= 0)
					return this;

				_rentalT = RentalTQuery(rentalTId);

				return this;
			}
		}

		public IUpdateRentalTService Set<T>(Expression<Func<RentalT, T>> lambda, object value)
		{
			_rentalT?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			if (HasWarnings)
				return false;

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_rentalT.ModifiedOn = DateTime.Now;
				RentalTRepo.Edit(_rentalT);

				// Use Transaction
				if (saveChanges) Database.SaveChanges();

				scope.Complete();
			}

			Cache.RemoveByPattern(CacheKeys.Server.RentalTs.Path);

			// Success
			return true;
		}

		#endregion

		#region Private Methods

		private RentalT RentalTQuery(int rentalTId)
		{
			var cFuture = (from c in RentalTRepo.Table
						   where c.RentalTId == rentalTId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		private bool IsValid()
		{
			if (_rentalT == null)
			{
				Warnings.Add(new RentalTWarning(RentalTWarningCode.RentalNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		#endregion
	}
}
