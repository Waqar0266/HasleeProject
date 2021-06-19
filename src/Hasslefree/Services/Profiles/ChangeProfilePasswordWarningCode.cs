namespace Hasslefree.Services.Profiles
{
	public enum ChangeProfilePasswordWarningCode
	{
		NotFound = 0,
		NoCurrentPassword = 1,
		NoNewPassword = 2,
		PasswordsDontMatch = 3,
		Custom = 4
	}
}
