using System;
using System.Linq;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.ModelBuilders.Customers
{
	public class BuildCustomerEmailModel : IBuildCustomerEmailModel, IInstancePerRequest
	{
		#region Private Properties

		private IReadOnlyRepository<Person> PersonRepo { get; }

		#endregion

		#region Fields

		private Int32 _accountId;

		#endregion

		#region Constructor

		public BuildCustomerEmailModel(
				IReadOnlyRepository<Person> personRepo
			)
		{
			PersonRepo = personRepo;
		}

		#endregion

		#region IBuildCustomerEmailModel

		public IBuildCustomerEmailModel this[Int32 id]
		{
			get
			{
				_accountId = id;

				return this;
			}
		}

		public CustomerEmailModel Get()
		{
			return new CustomerEmailModel();
		}

		#endregion
	}
}
