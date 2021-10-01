using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.RentalForms
{
	public class CreateExistingRentalFormService : ICreateExistingRentalFormService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<ExistingRentalForm> ExistingRentalFormRepo { get; }

		#endregion

		#region Fields

		private ExistingRentalForm _existingRentalForm;

		#endregion

		#region Constructor

		public CreateExistingRentalFormService
		(
			IDataRepository<ExistingRentalForm> existingRentalFormRepo
			)
		{
			// Repos
			ExistingRentalFormRepo = existingRentalFormRepo;
		}

		#endregion

		#region ICreateExistingRentalFormService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<RentalFormWarning> Warnings { get; } = new List<RentalFormWarning>();

		public int ExistingRentalFormId { get; private set; }

		public ICreateExistingRentalFormService New(ExistingRentalFormName existingRentalFormName, int existingRentalId, int downloadId)
		{
			_existingRentalForm = new ExistingRentalForm
			{
				ExistingRentalFormName = existingRentalFormName,
				ExistingRentalId = existingRentalId,
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
				ExistingRentalFormRepo.Insert(_existingRentalForm);

				scope.Complete();
			}

			// Set property object
			ExistingRentalFormId = _existingRentalForm.ExistingRentalFormId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_existingRentalForm == null)
			{
				Warnings.Add(new RentalFormWarning(RentalFormWarningCode.RentalNotFound));
				return false;
			}

			return !Warnings.Any();
		}

		#endregion
	}
}
