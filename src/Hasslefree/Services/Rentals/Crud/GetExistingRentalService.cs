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
		private IReadOnlyRepository<ExistingRentalForm> ExistingRentalFormRepo { get; }

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
			IReadOnlyRepository<ExistingRentalForm> existingRentalFormRepo,

			//Services
			IGetRentalService getRentalService,

			//Managers
			ICacheManager cache
		)
		{
			// Repos
			ExistingRentalRepo = existingRentalRepo;
			ExistingRentalFormRepo = existingRentalFormRepo;

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

			var existingRentalForms = Cache.Get(CacheKeys.Server.ExistingRentals.GetForms(_existingRental.ExistingRentalId), CacheKeys.Time.LongTime, () =>
			{
				return ExistingRentalFormRepo.Table.Include(a => a.Download).Where(a => a.ExistingRentalId == _existingRental.ExistingRentalId).ToList();
			});

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
				AgentWitness1Signature = _existingRental.AgentWitness1Signature,
				AgentWitness1InitialsId = _existingRental.AgentWitness1InitialsId,
				AgentWitness1Initials = _existingRental.AgentWitness1Initials,
				AgentWitness1Surname = _existingRental.AgentWitness1Surname,
				AgentWitness2Email = _existingRental.AgentWitness2Email,
				AgentWitness2Name = _existingRental.AgentWitness2Name,
				AgentWitness2SignatureId = _existingRental.AgentWitness2SignatureId,
				AgentWitness2Signature = _existingRental.AgentWitness2Signature,
				AgentWitness2InitialsId = _existingRental.AgentWitness2InitialsId,
				AgentWitness2Initials = _existingRental.AgentWitness2Initials,
				AgentWitness2Surname = _existingRental.AgentWitness2Surname,
				AmendedAddendum = _existingRental.AmendedAddendum,
				EndDate = _existingRental.EndDate,
				LandlordWitness1Email = _existingRental.LandlordWitness1Email,
				LandlordWitness1Name = _existingRental.LandlordWitness1Name,
				LandlordWitness1InitialsId = _existingRental.LandlordWitness1InitialsId,
				LandlordWitness1Initials = _existingRental.LandlordWitness1Initials,
				LandlordWitness1SignatureId = _existingRental.LandlordWitness1SignatureId,
				LandlordWitness1Signature = _existingRental.LandlordWitness1Signature,
				LandlordWitness1Surname = _existingRental.LandlordWitness1Surname,
				LandlordWitness2Email = _existingRental.LandlordWitness2Email,
				LandlordWitness2Name = _existingRental.LandlordWitness2Name,
				LandlordWitness2SignatureId = _existingRental.LandlordWitness2SignatureId,
				LandlordWitness2Signature = _existingRental.LandlordWitness2Signature,
				LandlordWitness2InitialsId = _existingRental.LandlordWitness2InitialsId,
				LandlordWitness2Initials = _existingRental.LandlordWitness2Initials,
				LandlordWitness2Surname = _existingRental.LandlordWitness2Surname,
				MaterialChanges = _existingRental.MaterialChanges,
				ParkingBays = _existingRental.ParkingBays,
				RenewalCommencementDate = _existingRental.RenewalCommencementDate,
				RenewalPeriod = _existingRental.RenewalPeriod,
				RenewalTerminationDate = _existingRental.RenewalTerminationDate,
				RenewLease = _existingRental.RenewLease,
				StartDate = _existingRental.StartDate,
				TerminationDate = _existingRental.TerminationDate,
				Tenant = _existingRental.Tenant,
				Forms = existingRentalForms.Select(a => new RentalFormModel()
				{
					CreatedOn = a.CreatedOn,
					DownloadId = a.DownloadId,
					Name = a.Download.FileName,
					Path = a.Download.RelativeFolderPath,
					Size = a.Download.Size / 1024 / 1024,
					Type = a.ExistingRentalFormNameEnum,
					MimeType = a.Download.ContentType
				}).ToList()
			};
		}

		#endregion

		#region Private Methods

		private ExistingRental ExistingRentalQuery(int existingRentalId)
		{
			return Cache.Get(CacheKeys.Server.ExistingRentals.ExistingRentalById(existingRentalId), CacheKeys.Time.LongTime, () =>
			{
				var cFuture = (from c in ExistingRentalRepo
											.Table
											.Include(r => r.Rental)
											.Include(r => r.AgentWitness1Initials)
											.Include(r => r.AgentWitness1Signature)
											.Include(r => r.AgentWitness2Initials)
											.Include(r => r.AgentWitness2Signature)
											.Include(r => r.LandlordWitness1Initials)
											.Include(r => r.LandlordWitness1Signature)
											.Include(r => r.LandlordWitness2Initials)
											.Include(r => r.LandlordWitness2Signature)
							   where c.ExistingRentalId == existingRentalId
							   select c).DeferredFirstOrDefault().FutureValue();

				return cFuture.Value;

			});
		}

		private ExistingRental ExistingRentalQuery(string existingRentalGuid)
		{
			return Cache.Get(CacheKeys.Server.ExistingRentals.ExistingRentalByGuid(existingRentalGuid), CacheKeys.Time.LongTime, () =>
			{
				var cFuture = (from c in ExistingRentalRepo
										.Table
										.Include(r => r.Rental)
										.Include(r => r.AgentWitness1Initials)
										.Include(r => r.AgentWitness1Signature)
										.Include(r => r.AgentWitness2Initials)
										.Include(r => r.AgentWitness2Signature)
										.Include(r => r.LandlordWitness1Initials)
										.Include(r => r.LandlordWitness1Signature)
										.Include(r => r.LandlordWitness2Initials)
										.Include(r => r.LandlordWitness2Signature)
							   where c.UniqueId.ToString().ToLower() == existingRentalGuid.ToLower()
							   select c).DeferredFirstOrDefault().FutureValue();

				return cFuture.Value;

			});
		}

		#endregion
	}
}
