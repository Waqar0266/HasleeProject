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
	public class UpdateRentalResolutionMemberService : IUpdateRentalResolutionMemberService, IInstancePerRequest
	{
		#region Constants

		private readonly string[] _restrictedProperties = { "RentalResolutionMemberId", "CreatedOn" };

		#endregion

		#region Private Properties

		// Repos
		private IDataRepository<RentalResolutionMember> RentalResolutionMemberRepo { get; }

		// Other
		private IDataContext Database { get; }
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private RentalResolutionMember _rentalResolutionMember;
		private int _rentalId;

		#endregion

		#region Constructor

		public UpdateRentalResolutionMemberService
		(
			IDataRepository<RentalResolutionMember> rentalResolutionMemberRepo,
			IDataContext database,
			ICacheManager cache
		)
		{
			// Repos
			RentalResolutionMemberRepo = rentalResolutionMemberRepo;

			// Other
			Database = database;
			Cache = cache;
		}

		#endregion

		#region IUpdateAgentService

		public IUpdateRentalResolutionMemberService WithRentalId(int rentalId)
		{
			_rentalId = rentalId;
			return this;
		}

		public IUpdateRentalResolutionMemberService this[int rentalResolutionMemberId]
		{
			get
			{
				_rentalResolutionMember = Query(rentalResolutionMemberId);

				return this;
			}
		}

		public IUpdateRentalResolutionMemberService Set<T>(Expression<Func<RentalResolutionMember, T>> lambda, object value)
		{
			_rentalResolutionMember?.SetPropertyValue(lambda, value, _restrictedProperties);

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				RentalResolutionMemberRepo.Edit(_rentalResolutionMember);

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

		private RentalResolutionMember Query(int rentalResolutionMemberId)
		{
			var cFuture = (from c in RentalResolutionMemberRepo.Table
						   where c.RentalResolutionMemberId == rentalResolutionMemberId
						   select c).DeferredFirstOrDefault().FutureValue();

			return cFuture.Value;
		}

		#endregion
	}
}
