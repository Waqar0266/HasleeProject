namespace Hasslefree.Services.Emails
{
	public enum SendEmailWarningCode
	{
		Unspecified = 0,
		FailedHttpRequest = 1,
		FailedSmtpSend = 2,
		NoRecipient = 3,
		NoSender = 4,
		NoContent = 5
	}
}
