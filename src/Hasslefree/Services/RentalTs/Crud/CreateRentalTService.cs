using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.RentalTs.Crud
{
	public class CreateRentalTService : ICreateRentalTService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<RentalT> RentalTRepo { get; }

		#endregion

		#region Fields

		private RentalT _rentalT;

		#endregion

		#region Constructor

		public CreateRentalTService
		(
			IDataRepository<RentalT> rentalTRepo
			)
		{
			// Repos
			RentalTRepo = rentalTRepo;
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

		public List<RentalTWarning> Warnings { get; } = new List<RentalTWarning>();

		public int RentalTId { get; private set; }

		public ICreateRentalTService New(LeaseType leaseType, string premises, string standErf, string address, string township)
		{
			_rentalT = new RentalT
			{
				LeaseType = leaseType,
				
			};

			return this;
		}

		public ICreateRentalTService WithLandlord(string idNumber, string name, string surname, string email, string mobile)
		{
			

			return this;
		}

		public ICreateRentalTService WithAgentId(int agentId)
		{
			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				RentalTRepo.Insert(_rentalT);

				scope.Complete();
			}

			// Set property object
			RentalTId = _rentalT.RentalTId;

			return true;
		}

		#endregion

		#region Private Methods

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
