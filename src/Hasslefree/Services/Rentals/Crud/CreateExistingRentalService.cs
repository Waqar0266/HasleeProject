using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.Rentals.Crud
{
	public class CreateExistingRentalService : ICreateExistingRentalService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<ExistingRental> ExistingRentalRepo { get; }

		#endregion

		#region Fields

		private ExistingRental _existingRental;

		#endregion

		#region Constructor

		public CreateExistingRentalService
		(
			IDataRepository<ExistingRental> existingRentalRepo
			)
		{
			// Repos
			ExistingRentalRepo = existingRentalRepo;
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

		public int ExistingRentalId { get; private set; }

		public ICreateExistingRentalService New(int rentalId, ExistingRentalType type)
		{
			_existingRental = new ExistingRental
			{
				RentalId = rentalId,
				ExistingRentalType = type
			};

			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				ExistingRentalRepo.Insert(_existingRental);

				scope.Complete();
			}

			// Set property object
			ExistingRentalId = _existingRental.ExistingRentalId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_existingRental == null)
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
