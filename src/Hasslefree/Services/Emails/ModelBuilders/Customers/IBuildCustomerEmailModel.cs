using System;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.ModelBuilders.Customers
{
	public interface IBuildCustomerEmailModel
	{
		IBuildCustomerEmailModel this[Int32 id] { get; }

		CustomerEmailModel Get();
	}
}
