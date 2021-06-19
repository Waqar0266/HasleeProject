namespace Hasslefree.Services.People.Warnings
{
	public enum PersonWarningCode
	{
		// Object
		PersonNotFound,
		PersonsNotFound,
		NullPerson,
		
		// Database
		DuplicateAccountEmail,
		DuplicateLoginEmail,
		DuplicatePersonEmail,

		// Properties
		PropertyNotValid,

		// Services
		UpdateLoginServiceError
	}
}