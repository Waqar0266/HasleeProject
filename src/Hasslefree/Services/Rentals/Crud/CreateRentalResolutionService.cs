using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Hasslefree.Services.Rentals.Crud
{
	public class CreateRentalResolutionService : ICreateRentalResolutionService, IInstancePerRequest
	{
		//Repos
		private IDataRepository<RentalResolution> RentalResolutionRepo { get; }

		//Manager
		private ICacheManager Cache { get; }

		public CreateRentalResolutionService(
		//Repos						
		IDataRepository<RentalResolution> rentalResolutionRepo,

		//Managers
		ICacheManager cache
		)
		{
			//Repos
			RentalResolutionRepo = rentalResolutionRepo;

			//Managers
			Cache = cache;
		}

		private int _rentalId;
		private string _heldAt;
		private DateTime _heldOn;
		private string _leaseName;
		private string _authorizedName;
		private string _authorizedSurname;

		private List<RentalResolutionMember> _members;

		public int RentalResolutionId { get; internal set; }

		public ICreateRentalResolutionService New(string heldAt, DateTime heldOn, string leaseName, string authorizedName, string authorizedSurname)
		{
			_heldAt = heldAt;
			_heldOn = heldOn;
			_leaseName = leaseName;
			_authorizedName = authorizedName;
			_authorizedSurname = authorizedSurname;

			return this;
		}

		public ICreateRentalResolutionService WithMember(string name, string surname, string email, string idNumber)
		{
			if (_members == null) _members = new List<RentalResolutionMember>();

			_members.Add(new RentalResolutionMember()
			{
				IdNumber = idNumber,
				Name = name,
				Surname = surname,
				Email = email
			});

			return this;
		}

		public ICreateRentalResolutionService WithRentalId(int rentalId)
		{
			_rentalId = rentalId;
			return this;
		}

		public bool Create(bool saveChanges = true)
		{
			var rentalResolution = new RentalResolution()
			{
				AuthorizedName = _authorizedName,
				AuthorizedSurname = _authorizedSurname,
				LeaseName = _leaseName,
				HeldAt = _heldAt,
				HeldOn = _heldOn,
				RentalId = _rentalId,
				Members = _members
			};

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				RentalResolutionRepo.Insert(rentalResolution);

				scope.Complete();
			}

			// Set property object
			RentalResolutionId = rentalResolution.RentalResolutionId;

			Cache.RemoveByPattern(CacheKeys.Server.Rentals.Path);

			return true;
		}
	}
}
