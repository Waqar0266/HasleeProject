using Hasslefree.Core.Domain.Sales;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hasslefree.Services.Sales.Crud
{
    public class CreateSaleService : ICreateSaleService, IInstancePerRequest
    {
		#region Private Properties

		// Repos
		private IDataRepository<Sale> SaleRepo { get; }

		#endregion

		#region Fields

		private Sale _sale;

		#endregion

		#region Constructor

		public CreateSaleService
		(
			IDataRepository<Sale> saleRepo
			)
		{
			// Repos
			SaleRepo = saleRepo;
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

		public List<SaleWarning> Warnings { get; } = new List<SaleWarning>();

		public int SaleId { get; private set; }
		public List<Seller> Sellers { get { return _sale.Sellers.ToList(); } }

		public ICreateSaleService New(SaleType saleType)
		{
			_sale = new Sale
			{
				SaleType = saleType,
				SaleStatus = SaleStatus.PendingNew
			};

			return this;
		}

		public ICreateSaleService WithSeller(string idNumber, string name, string surname, string email, string mobile)
		{
			_sale.Sellers.Add(new Seller()
			{
				IdNumber = idNumber,
				Tempdata = BuildTempData(name, surname, email, mobile)
			});

			return this;
		}

		public ICreateSaleService WithAgentId(int agentId)
		{
			_sale.AgentId = agentId;
			return this;
		}

		public bool Create()
		{
			if (HasWarnings) return false;

			// Use Transaction
			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				SaleRepo.Insert(_sale);

				scope.Complete();
			}

			// Set property object
			SaleId = _sale.SaleId;

			return true;
		}

		#endregion

		#region Private Methods

		private bool IsValid()
		{
			if (_sale == null)
			{
				Warnings.Add(new SaleWarning(SaleWarningCode.SaleNotFound));
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
