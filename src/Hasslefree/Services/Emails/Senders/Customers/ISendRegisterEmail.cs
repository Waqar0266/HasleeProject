using System;
using Hasslefree.Core.Domain.Accounts;

namespace Hasslefree.Services.Emails.Senders.Customers
{
	public interface ISendRegisterEmail
	{
		Boolean this[Person person] { get; }
	}
}
