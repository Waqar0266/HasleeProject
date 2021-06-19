using System.Collections.Generic;
using Hasslefree.Web.Models.Accounts;

namespace Hasslefree.Services.Accounts.Otp
{
	public interface IRegisterOtpService
	{
		bool HasWarnings { get; }
		List<RegisterOtpWarning> Warnings { get; }

		IRegisterOtpService WithEmail(string email);
		IRegisterOtpService WithHash(string hash);
		IRegisterOtpService WithOtp(string otp);
		IRegisterOtpService GenerateHash(RegisterModel model);

		bool ValidateOtp();
		bool SendOtp(out string hash);
	}
}