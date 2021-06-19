using System;
using System.Collections.Generic;

namespace Hasslefree.Services.Accounts.Actions
{
	public interface ILoginService
	{
		bool HasWarnings { get; }
		List<LoginWarning> Warnings { get; }

		ILoginService WithEmail(string email);
		ILoginService WithPassword(string password);
		ILoginService WithGuid(Guid guid);
		ILoginService Remember(bool remember);

		bool Login();
		bool ValidateLogin();
	}
}
