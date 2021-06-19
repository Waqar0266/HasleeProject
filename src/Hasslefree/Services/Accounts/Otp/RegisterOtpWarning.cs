namespace Hasslefree.Services.Accounts.Otp
{
	public class RegisterOtpWarning
	{
		#region Constructor

		public RegisterOtpWarning(RegisterOtpWarningCode code)
		{
			Code = code;
		}

		#endregion

		#region Properties

		public RegisterOtpWarningCode Code { get; set; }

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
					case RegisterOtpWarningCode.InvalidEmail:
						return "The email is not valid";
					case RegisterOtpWarningCode.InvalidHash:
						return "Something went wrong while generating your OTP. Please refresh the page and try again.";
					case RegisterOtpWarningCode.InvalidOtp:
						return "The OTP is incorrect";
					case RegisterOtpWarningCode.EmailFailed:
						return "Something went wrong while sending the OTP email. Please refresh the page and try again.";
					default:
						return "No error code. Blame the programmer.";
				}
			}
		}

		#endregion

	}
}
