namespace Hasslefree.Services.Accounts.Password
{
	public enum ForgotPasswordWarningCode
	{
		NotFound = 0,
		NotHuman = 1, // Used when Captcha doesn't match
		EmailFailed = 2 // When the email failed to send
	}
}
