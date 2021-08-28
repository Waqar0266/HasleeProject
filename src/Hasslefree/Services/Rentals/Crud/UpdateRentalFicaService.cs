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
	public class UpdateRentalFicaService : IUpdateRentalFicaService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalFicaId", "CreatedOn" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<RentalFica> RentalFicaRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private RentalFica _rentalFica;
		private int _rentalId;

		#endregion

		#region Constructor

		public UpdateRentalFicaService
		(
			IDataRepository<RentalFica> rentalFicaRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			RentalFicaRepo = rentalFicaRepo;

			// Other
			Database = database;
			Cache = cache;
		}

		#endregion

		#region IUpdateRentalFicaService

		public IUpdateRentalFicaService WithRentalId(int rentalId)
		{
			_rentalId = rentalId;
			return this;
		}

		public IUpdateRentalFicaService this[int rentalFicaId]
		{
			get
			{
				if (rentalFicaId <= 0)
				{
					_rentalFica = new RentalFica()
					{
						RentalId = _rentalId
					};
					RentalFicaRepo.Insert(_rentalFica);
					return this;
				}


				_rentalFica = Query(rentalFicaId);

				return this;
			}
		}

		public IUpdateRentalFicaService Set<T>(Expression<Func<RentalFica, T>> lambda, object value)
		{
			_rentalFica?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				_rentalFica.RentalId = _rentalId;
				RentalFicaRepo.Edit(_rentalFica);

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

		private RentalFica Query(int rentalFicaId)
		{
			var cFuture = (from c in RentalFicaRepo.Table
						   where c.RentalFicaId == rentalFicaId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		#endregion
	}
}
