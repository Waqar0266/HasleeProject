using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.RentalForms
{
	public class CreateRentalFormService : ICreateRentalFormService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<RentalForm> RentalFormRepo { get; }

		#endregion

		#region Fields

		private RentalForm _rentalForm;

		#endregion

		#region Constructor

		public CreateRentalFormService
		(
			IDataRepository<RentalForm> rentalFormRepo
			)
		{
			// Repos
			RentalFormRepo = rentalFormRepo;
		}

		#endregion

		#region ICreateRentalFormService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<RentalFormWarning> Warnings { get; } = new List<RentalFormWarning>();

		public int RentalFormId { get; private set; }

		public ICreateRentalFormService New(RentalFormName rentalFormName, int rentalId, int downloadId)
		{
			_rentalForm = new RentalForm
			{
				RentalFormName = rentalFormName,
				RentalId = rentalId,
				DownloadId = downloadId
			};

			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				RentalFormRepo.Insert(_rentalForm);

				scope.Complete();
			}

			// Set property object
			RentalFormId = _rentalForm.RentalFormId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_rentalForm == null)
			{
				Warnings.Add(new RentalFormWarning(RentalFormWarningCode.RentalNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		#endregion
	}
}
