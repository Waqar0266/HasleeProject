using System;

namespace Hasslefree.Services.Emails
{
	public class SendEmailWarning
	{
		/* CTOR */
		public SendEmailWarning(SendEmailWarningCode code)
		{
			Code = code;
		}

		/* Properties */
		public SendEmailWarningCode Code { get; }
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
					case SendEmailWarningCode.FailedHttpRequest:
						return "Failed to retrieve the email body";
					case SendEmailWarningCode.FailedSmtpSend:
						return "Failed to send the email";
					case SendEmailWarningCode.Unspecified:
						return "An unspecified error occured while trying to send an email";
					case SendEmailWarningCode.NoRecipient:
						return "No email recipient";
					case SendEmailWarningCode.NoSender:
						return "No email sender";
					case SendEmailWarningCode.NoContent:
						return "No email content";
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
