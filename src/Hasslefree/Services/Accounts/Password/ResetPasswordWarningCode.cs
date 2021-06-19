namespace Hasslefree.Services.Accounts.Password
{
	public enum ResetPasswordWarningCode
	{
		NotFound = 0, // The person could noty be found
		NotHuman = 1, // Used when Captcha doesn't match
		NotValid = 2, // When the otp does not match the hash
	}
}
