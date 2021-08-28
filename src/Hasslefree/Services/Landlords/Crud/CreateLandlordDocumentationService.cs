using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Transactions;

namespace Hasslefree.Services.Landlords.Crud
{
	public class CreateLandlordDocumentationService : ICreateLandlordDocumentationService, IInstancePerRequest
	{
		#region Private Properties

		// Repos
		private IDataRepository<LandlordDocumentation> Repo { get; }

		#endregion

		#region Fields

		private List<LandlordDocumentation> _entities;

		#endregion

		#region Constructor

		public CreateLandlordDocumentationService
		(
			IDataRepository<LandlordDocumentation> repo
			)
		{
			// Repos
			Repo = repo;
		}

		#endregion

		#region ICreateLandlordDocumentationService

		public ICreateLandlordDocumentationService Add(int rentalLandlordId, int downloadId)
		{
			if (_entities == null) _entities = new List<LandlordDocumentation>();

			_entities.Add(new LandlordDocumentation()
			{
				RentalLandlordId = rentalLandlordId,
				DownloadId = downloadId
			});

			return this;
		}

		public bool Process()
		{
			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				Repo.Insert(_entities);

				scope.Complete();
			}

			return true;
		}

		#endregion
	}
}
