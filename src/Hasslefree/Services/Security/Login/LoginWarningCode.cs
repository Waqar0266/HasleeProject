namespace Hasslefree.Services.Security.Login
{
	public enum LoginWarningCode
	{
		// Object
		LoginNotFound,
		LoginsNotFound,
		NullLogin,

		PersonNotFound,
		SecurityGroupsNotFound,

		// Database
		DuplicateLoginEmail,
		DuplicatePersonEmail,
		ExistingLoginRecord,

		// Properties
		PropertyNotValid,

		// Services
		UpdatePersonServiceError,
	}
}