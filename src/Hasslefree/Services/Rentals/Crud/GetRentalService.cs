using Hasslefree.Core;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Agents;
using Hasslefree.Core.Domain.Common;
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
	public class GetRentalService : IGetRentalService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IReadOnlyRepository<Rental> RentalRepo { get; }
		private IReadOnlyRepository<RentalLandlord> RentalLandlordRepo { get; }
		private IReadOnlyRepository<RentalMandate> RentalMandateRepo { get; }
		private IReadOnlyRepository<RentalWitness> RentalWitnessRepo { get; }
		private IReadOnlyRepository<Agent> AgentRepo { get; }
		private IReadOnlyRepository<Person> PersonRepo { get; }
		private IReadOnlyRepository<LandlordBankAccount> LandlordBankAccountRepo { get; }
		private IReadOnlyRepository<Address> AddressRepo { get; }
		private IReadOnlyRepository<AgentAddress> AgentAddressRepo { get; }
		private IReadOnlyRepository<LandlordAddress> LandlordAddressRepo { get; }
		private IReadOnlyRepository<RentalFica> RentalFicaRepo { get; }
		private IReadOnlyRepository<RentalResolution> RentalResolutionRepo { get; }
		private IReadOnlyRepository<LandlordDocumentation> LandlordDocumentationRepo { get; }
		private IReadOnlyRepository<RentalForm> RentalFormRepo { get; }

		//Managers
		private ICacheManager Cache { get; }

		#endregion

		#region Fields

		private Rental _rental;

		#endregion

		#region Constructor

		public GetRentalService
		(
			//Repos
			IReadOnlyRepository<Rental> rentalRepo,
			IReadOnlyRepository<RentalLandlord> rentalLandlordRepo,
			IReadOnlyRepository<RentalMandate> rentalMandateRepo,
			IReadOnlyRepository<Agent> agentRepo,
			IReadOnlyRepository<RentalWitness> rentalWitnessRepo,
			IReadOnlyRepository<Person> personRepo,
			IReadOnlyRepository<LandlordBankAccount> landlordBankAccountRepo,
			IReadOnlyRepository<Address> addressRepo,
			IReadOnlyRepository<AgentAddress> agentAddressRepo,
			IReadOnlyRepository<LandlordAddress> landlordAddressRepo,
			IReadOnlyRepository<RentalFica> rentalFicaRepo,
			IReadOnlyRepository<RentalResolution> rentalResolutionRepo,
			IReadOnlyRepository<LandlordDocumentation> landlordDocumentationRepo,
			IReadOnlyRepository<RentalForm> rentalFormRepo,

			//Managers
			ICacheManager cache
		)
		{
			// Repos
			RentalRepo = rentalRepo;
			RentalLandlordRepo = rentalLandlordRepo;
			RentalMandateRepo = rentalMandateRepo;
			AgentRepo = agentRepo;
			RentalWitnessRepo = rentalWitnessRepo;
			PersonRepo = personRepo;
			LandlordBankAccountRepo = landlordBankAccountRepo;
			AddressRepo = addressRepo;
			AgentAddressRepo = agentAddressRepo;
			LandlordAddressRepo = landlordAddressRepo;
			RentalFicaRepo = rentalFicaRepo;
			RentalResolutionRepo = rentalResolutionRepo;
			LandlordDocumentationRepo = landlordDocumentationRepo;
			RentalFormRepo = rentalFormRepo;

			//Managers
			Cache = cache;
		}

		#endregion

		#region IGetRentalService

		public IGetRentalService this[int rentalId]
		{
			get
			{
				if (rentalId <= 0)
					return this;

				_rental = RentalQuery(rentalId);

				return this;
			}
		}

		public IGetRentalService this[string rentalGuid]
		{
			get
			{
				if (String.IsNullOrEmpty(rentalGuid))
					return this;

				_rental = RentalQuery(rentalGuid);

				return this;
			}
		}

		public RentalGet Get()
		{
			if (_rental == null) return null;

			var rentalLandlords = Cache.Get(CacheKeys.Server.Rentals.GetLandlords(_rental.RentalId), CacheKeys.Time.LongTime, () => RentalLandlordRepo.Table.Include(a => a.Person).Include(a => a.Initials).Include(a => a.Signature).Where(a => a.RentalId == _rental.RentalId).ToList());
			var rentalMandate = Cache.Get(CacheKeys.Server.Rentals.GetMandate(_rental.RentalId), CacheKeys.Time.LongTime, () => RentalMandateRepo.Table.FirstOrDefault(a => a.RentalId == _rental.RentalId));
			var rentalResolution = Cache.Get(CacheKeys.Server.Rentals.GetResolution(_rental.RentalId), CacheKeys.Time.LongTime, () => RentalResolutionRepo.Table.Include(x => x.Members).Include("Members.Signature").FirstOrDefault(a => a.RentalId == _rental.RentalId));
			var rentalFica = Cache.Get(CacheKeys.Server.Rentals.GetFica(_rental.RentalId), CacheKeys.Time.LongTime, () =>
			RentalFicaRepo
			.Table
			.Include(x => x.BranchAddress)
			.Include(x => x.HeadOfficeAddress)
			.Include(x => x.Partner1Address)
			.Include(x => x.Partner2Address)
			.Include(x => x.Partner3Address)
			.Include(x => x.RegisteredAddress)
			.FirstOrDefault(a => a.RentalId == _rental.RentalId));
			var agent = Cache.Get(CacheKeys.Server.Rentals.GetAgent(_rental.RentalId), CacheKeys.Time.LongTime, () => AgentRepo.Table.Include(a => a.Signature).Include(a => a.Initials).Include(x => x.Person).FirstOrDefault(a => a.AgentId == _rental.AgentId));
			var rentalWitness = Cache.Get(CacheKeys.Server.Rentals.GetWitness(_rental.RentalId), CacheKeys.Time.LongTime, () =>
			RentalWitnessRepo
			.Table
			.Include(a => a.AgentWitness1Signature)
			.Include(a => a.AgentWitness1Initials)
			.Include(a => a.AgentWitness2Signature)
			.Include(a => a.AgentWitness2Initials)
			.Include(a => a.LandlordWitness1Signature)
			.Include(a => a.LandlordWitness1Initials)
			.Include(a => a.LandlordWitness2Signature)
			.Include(a => a.LandlordWitness2Initials)
			.FirstOrDefault(a => a.RentalId == _rental.RentalId));

			var agentPerson = Cache.Get(CacheKeys.Server.Rentals.GetAgentPerson(agent.PersonId ?? 0), CacheKeys.Time.LongTime, () => PersonRepo.Table.FirstOrDefault(a => a.PersonId == (agent.PersonId ?? 0)));
			var landlordBankAccounts = Cache.Get(CacheKeys.Server.Rentals.GetLandlordBankAccounts(_rental.RentalId), CacheKeys.Time.LongTime, () => LandlordBankAccountRepo.Table.Where(a => a.RentalId == _rental.RentalId).ToList());
			var landlordAddresses = Cache.Get(CacheKeys.Server.Rentals.GetLandlordCommonAddresses(rentalLandlords?.FirstOrDefault().RentalLandlordId ?? 0), CacheKeys.Time.LongTime, () =>
			{
				var id = rentalLandlords.FirstOrDefault().RentalLandlordId;
				return LandlordAddressRepo.Table.Include(a => a.Address).Where(a => a.RentalLandlordId == id).ToList();
			});
			var agentAddresses = Cache.Get(CacheKeys.Server.Rentals.GetAgentAddresses(agent.AgentId), CacheKeys.Time.LongTime, () => AgentAddressRepo.Table.Include(a => a.Address).Where(a => a.AgentId == agent.AgentId).ToList());
			var physical = AddressType.Residential.ToString();
			var postal = AddressType.Residential.ToString();
			var agentPhysicalAddressId = agentAddresses?.FirstOrDefault(a => a.Address.TypeEnum == physical) ?? null;
			var landlordPhysicalAddressId = landlordAddresses?.FirstOrDefault(a => a.Address.TypeEnum == physical) ?? null;
			var agentPostalAddressId = agentAddresses?.FirstOrDefault(a => a.Address.TypeEnum == postal) ?? null;
			var landlordPostalAddressId = landlordAddresses?.FirstOrDefault(a => a.Address.TypeEnum == postal) ?? null;
			var agentPhysicalAddress = Cache.Get(CacheKeys.Server.Rentals.GetAgentAddress(agentPhysicalAddressId?.AddressId ?? 0), CacheKeys.Time.LongTime, () =>
			{
				if (agentPhysicalAddressId == null) return null;
				return AddressRepo.Table.FirstOrDefault(a => a.AddressId == agentPhysicalAddressId.AddressId);
			});
			var agentPostalAddress = Cache.Get(CacheKeys.Server.Rentals.GetAgentAddress(agentPostalAddressId?.AddressId ?? 0), CacheKeys.Time.LongTime, () =>
			{
				if (agentPostalAddressId == null) return null;
				return AddressRepo.Table.FirstOrDefault(a => a.AddressId == agentPostalAddressId.AddressId);
			});

			var landlordPhysicalAddress = Cache.Get(CacheKeys.Server.Rentals.GetLandlordAddresses(landlordPhysicalAddressId?.AddressId ?? 0), CacheKeys.Time.LongTime, () =>
			{
				if (landlordPhysicalAddressId == null) return null;
				return AddressRepo.Table.FirstOrDefault(a => a.AddressId == landlordPhysicalAddressId.AddressId);
			});
			var landlordPostalAddress = Cache.Get(CacheKeys.Server.Rentals.GetLandlordAddresses(landlordPostalAddressId?.AddressId ?? 0), CacheKeys.Time.LongTime, () =>
			{
				if (landlordPostalAddressId == null) return null;
				return AddressRepo.Table.FirstOrDefault(a => a.AddressId == landlordPostalAddressId.AddressId);
			});

			var landlordDocumentation = Cache.Get(CacheKeys.Server.Rentals.GetLandlordDocumentation(rentalLandlords?.FirstOrDefault().RentalLandlordId ?? 0), CacheKeys.Time.LongTime, () =>
			{
				var id = rentalLandlords.FirstOrDefault().RentalLandlordId;
				return LandlordDocumentationRepo.Table.Include(a => a.Download).Where(a => a.RentalLandlordId == id).Select(a => a.Download).ToList();
			});

			var rentalForms = Cache.Get(CacheKeys.Server.Rentals.GetForms(_rental.RentalId), CacheKeys.Time.LongTime, () =>
			{
				return RentalFormRepo.Table.Include(a => a.Download).Where(a => a.RentalId == _rental.RentalId).ToList();
			});

			return new RentalGet
			{
				RentalId = _rental.RentalId,
				RentalGuid = _rental.UniqueId,
				RentalLandlords = rentalLandlords,
				RentalMandate = rentalMandate,
				RentalStatus = _rental.RentalStatus,
				Address = _rental.Address,
				Premises = _rental.Premises,
				StandErf = _rental.StandErf,
				Township = _rental.Township,
				LeaseType = _rental.LeaseType,
				Agent = agent,
				AskLandlordConsent = _rental.AskLandlordConsent,
				ContactLandlord = _rental.ContactLandlord,
				Explaining = _rental.Explaining,
				IncomingSnaglist = _rental.IncomingSnaglist,
				Informing = _rental.Informing,
				Management = _rental.Management,
				Negotiating = _rental.Negotiating,
				OutgoingSnaglist = _rental.OutgoingSnaglist,
				PayingLandlord = _rental.PayingLandlord,
				ProcureDepositLandlord = _rental.ProcureDepositLandlord,
				ProcureDepositOther = _rental.ProcureDepositOther,
				ProcureDepositPreviousRentalAgent = _rental.ProcureDepositPreviousRentalAgent,
				Procurement = _rental.Procurement,
				ProvideLandlord = _rental.ProvideLandlord,
				SpecialConditions = _rental.SpecialConditions,
				SpecificRequirements = _rental.SpecificRequirements,
				TransferDeposit = _rental.TransferDeposit,
				Deposit = _rental.Deposit ?? 0m,
				DepositPaymentDate = _rental.DepositPaymentDate ?? DateTime.MinValue,
				MonthlyPaymentDate = _rental.MonthlyPaymentDate ?? DateTime.MinValue,
				MonthlyRental = _rental.MonthlyRental ?? 0m,
				RentalWitness = rentalWitness,
				AgentPerson = agentPerson,
				AgentId = _rental.AgentId,
				LandlordBankAccounts = landlordBankAccounts,
				LandlordPhysicalAddress = landlordPhysicalAddress,
				LandlordPostalAddress = landlordPostalAddress,
				AgentPhysicalAddress = agentPhysicalAddress,
				AgentPostalAddress = agentPostalAddress,
				Marketing = _rental.Marketing,
				PowerOfAttorney = _rental.PowerOfAttorney,
				RentalFica = rentalFica,
				RentalResolution = rentalResolution,
				LandlordDocumentation = landlordDocumentation.Select(a => new RentalDocumentModel()
				{
					CreatedOn = a.CreatedOn,
					DownloadId = a.DownloadId,
					Name = a.FileName,
					Path = a.RelativeFolderPath,
					Size = a.Size / 1024 / 1024
				}).ToList(),
				Forms = rentalForms.Select(a => new RentalFormModel()
				{
					CreatedOn = a.CreatedOn,
					DownloadId = a.DownloadId,
					Name = a.Download.FileName,
					Path = a.Download.RelativeFolderPath,
					Size = a.Download.Size / 1024 / 1024,
					Type = a.RentalFormNameEnum,
					MimeType = a.Download.ContentType
				}).ToList()
			};
		}

		#endregion

		#region Private Methods

		private Rental RentalQuery(int rentalId)
		{
			return Cache.Get(CacheKeys.Server.Rentals.RentalById(rentalId), CacheKeys.Time.LongTime, () =>
			{
				var cFuture = (from c in RentalRepo.Table
							   where c.RentalId == rentalId
							   select c).DeferredFirstOrDefault().FutureValue();

				return cFuture.Value;

			});
		}

		private Rental RentalQuery(string rentalGuid)
		{
			return Cache.Get(CacheKeys.Server.Rentals.RentalByGuid(rentalGuid), CacheKeys.Time.LongTime, () =>
			{
				var cFuture = (from c in RentalRepo.Table
							   where c.UniqueId.ToString().ToLower() == rentalGuid.ToLower()
							   select c).DeferredFirstOrDefault().FutureValue();

				return cFuture.Value;

			});
		}

		#endregion
	}
}
