using System;

namespace Hasslefree.Services.Accounts.Password
{
	public class ForgotPasswordWarning
	{
		/* CTOR */
		public ForgotPasswordWarning(ForgotPasswordWarningCode code)
		{
			Code = code;
		}

		/* Properties */
		public ForgotPasswordWarningCode Code { get; }
		public int Number => (int)Code;

		/// <summary>
		/// Message of the warning
		/// </summary>
		public string Message
		{
			get
			{
				switch (Code)
				{
					case ForgotPasswordWarningCode.NotFound:
						return "An account with the the specified email does not exist";
					case ForgotPasswordWarningCode.NotHuman:
						return "The verification code is not correct. Are you a robot?";
					case ForgotPasswordWarningCode.EmailFailed:
						return "There was a problem when we tried to send the OTP email";
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
