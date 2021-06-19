using System.Collections.Generic;

namespace Hasslefree.Services.Accounts.Password
{
	public interface IForgotPasswordService
	{
		bool HasWarnings { get; }
		List<ForgotPasswordWarning> Warnings { get; }
		bool ForgotPassword( string email, out int otp, out string hash);
	}
}
