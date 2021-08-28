using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.Rentals.Crud
{
	public class CreateRentalService : ICreateRentalService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<Rental> RentalRepo { get; }

		#endregion

		#region Fields

		private Rental _rental;

		#endregion

		#region Constructor

		public CreateRentalService
		(
			IDataRepository<Rental> rentalRepo
			)
		{
			// Repos
			RentalRepo = rentalRepo;
		}

		#endregion

		#region ICreateRentalService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<RentalWarning> Warnings { get; } = new List<RentalWarning>();

		public int RentalId { get; private set; }
		public List<RentalLandlord> Landlords { get { return _rental.Landlords.ToList(); } }

		public ICreateRentalService New(RentalType type, LeaseType leaseType, string premises, string standErf, string address, string township)
		{
			_rental = new Rental
			{
				RentalType = type,
				LeaseType = leaseType,
				Premises = premises,
				StandErf = standErf,
				Address = address,
				RentalStatus = RentalStatus.PendingNew,
				Township = township
			};

			return this;
		}

		public ICreateRentalService WithLandlord(string idNumber, string name, string surname, string email, string mobile)
		{
			_rental.Landlords.Add(new RentalLandlord()
			{
				IdNumber = idNumber,
				Tempdata = BuildTempData(name, surname, email, mobile)
			});

			return this;
		}

		public ICreateRentalService WithAgentId(int agentId)
		{
			_rental.AgentId = agentId;
			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				RentalRepo.Insert(_rental);

				scope.Complete();
			}

			// Set property object
			RentalId = _rental.RentalId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_rental == null)
			{
				Warnings.Add(new RentalWarning(RentalWarningCode.RentalNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		private string BuildTempData(string name, string surname, string email, string mobile)
		{
			return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{name};{surname};{email};{mobile}"));
		}

		#endregion
	}
}
