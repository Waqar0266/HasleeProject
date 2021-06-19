using System;

namespace Hasslefree.Services.Accounts.Password
{
	public class ResetPasswordWarning
	{
		/* CTOR */
		public ResetPasswordWarning(ResetPasswordWarningCode code)
		{
			Code = code;
		}

		/* Properties */
		public ResetPasswordWarningCode Code { get; }
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
					case ResetPasswordWarningCode.NotFound:
						return "An account with the the specified email does not exist";
					case ResetPasswordWarningCode.NotHuman:
						return "The verification code is not correct. Are you a robot?";
					case ResetPasswordWarningCode.NotValid:
						return "The OTP is not valid";
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
