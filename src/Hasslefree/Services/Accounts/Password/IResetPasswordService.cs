using System.Collections.Generic;

namespace Hasslefree.Services.Accounts.Password
{
	public interface IResetPasswordService
	{
		bool HasWarnings { get; }
		List<ResetPasswordWarning> Warnings { get; }
		bool ResetPassword( string email, string password, string hash, string otp);
	}
}
