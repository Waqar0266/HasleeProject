using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using Hasslefree.Web.Models.Rentals;
using System;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Rentals.Crud
{
	public class GetExistingRentalService : IGetExistingRentalService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IReadOnlyRepository<ExistingRental> ExistingRentalRepo { get; }

		//Services
		private IGetRentalService GetRentalService { get; }

		//Managers
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private ExistingRental _existingRental;

		#endregion

		#region Constructor

		public GetExistingRentalService
		(
			//Repos
			IReadOnlyRepository<ExistingRental> existingRentalRepo,

			//Services
			IGetRentalService getRentalService,

			//Managers
			ICacheManager cache
		)
		{
			// Repos
			ExistingRentalRepo = existingRentalRepo;

			//Services
			GetRentalService = getRentalService;

			//Managers
			Cache = cache;
		}

		#endregion

		#region IGetRentalService

		public IGetExistingRentalService this[int existingRentalId]
		{
			get
			{
				if (existingRentalId <= 0)
					return this;

				_existingRental = ExistingRentalQuery(existingRentalId);

				return this;
			}
		}

		public IGetExistingRentalService this[string existingRentalGuid]
		{
			get
			{
				if (String.IsNullOrEmpty(existingRentalGuid))
					return this;

				_existingRental = ExistingRentalQuery(existingRentalGuid);

				return this;
			}
		}

		public ExistingRentalGet Get()
		{
			if (_existingRental == null) return null;

			return new ExistingRentalGet
			{
				ExistingRentalId = _existingRental.ExistingRentalId,
				ExistingRentalType = _existingRental.ExistingRentalType,
				RentalId = _existingRental.RentalId,
				ExistingRentalGuid = _existingRental.UniqueId,
				Rental = GetRentalService[_existingRental.RentalId].Get(),
				Status = _existingRental.ExistingRentalStatus,
				AgentWitness1Email = _existingRental.AgentWitness1Email,
				AgentWitness1Name = _existingRental.AgentWitness1Name,
				AgentWitness1SignatureId = _existingRental.AgentWitness1SignatureId,
				AgentWitness1Surname = _existingRental.AgentWitness1Surname,
				AgentWitness2Email = _existingRental.AgentWitness2Email,
				AgentWitness2Name = _existingRental.AgentWitness2Name,
				AgentWitness2SignatureId = _existingRental.AgentWitness2SignatureId,
				AgentWitness2Surname = _existingRental.AgentWitness2Surname,
				AmendedAddendum = _existingRental.AmendedAddendum,
				EndDate = _existingRental.EndDate,
				LandlordWitness1Email = _existingRental.LandlordWitness1Email,
				LandlordWitness1Name = _existingRental.LandlordWitness1Name,
				LandlordWitness1SignatureId = _existingRental.LandlordWitness1SignatureId,
				LandlordWitness1Surname = _existingRental.LandlordWitness1Surname,
				LandlordWitness2Email = _existingRental.LandlordWitness2Email,
				LandlordWitness2Name = _existingRental.LandlordWitness2Name,
				LandlordWitness2SignatureId = _existingRental.LandlordWitness2SignatureId,
				LandlordWitness2Surname = _existingRental.LandlordWitness2Surname,
				MaterialChanges = _existingRental.MaterialChanges,
				ParkingBays = _existingRental.ParkingBays,
				RenewalCommencementDate = _existingRental.RenewalCommencementDate,
				RenewalPeriod = _existingRental.RenewalPeriod,
				RenewalTerminationDate = _existingRental.RenewalTerminationDate,
				RenewLease = _existingRental.RenewLease,
				StartDate = _existingRental.StartDate,
				TerminationDate = _existingRental.TerminationDate
			};
		}

		#endregion

		#region Private Methods

		private ExistingRental ExistingRentalQuery(int existingRentalId)
		{
			return Cache.Get(CacheKeys.Server.ExistingRentals.ExistingRentalById(existingRentalId), CacheKeys.Time.LongTime, () =>
			{
				var cFuture = (from c in ExistingRentalRepo.Table.Include(r => r.Rental)
							   where c.ExistingRentalId == existingRentalId
							   select c).DeferredFirstOrDefault().FutureValue();

				return cFuture.Value;

			});
		}

		private ExistingRental ExistingRentalQuery(string existingRentalGuid)
		{
			return Cache.Get(CacheKeys.Server.ExistingRentals.ExistingRentalByGuid(existingRentalGuid), CacheKeys.Time.LongTime, () =>
			{
				var cFuture = (from c in ExistingRentalRepo.Table
							   where c.UniqueId.ToString().ToLower() == existingRentalGuid.ToLower()
							   select c).DeferredFirstOrDefault().FutureValue();

				return cFuture.Value;

			});
		}

		#endregion
	}
}
