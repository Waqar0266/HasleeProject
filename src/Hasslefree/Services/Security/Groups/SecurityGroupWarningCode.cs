namespace Hasslefree.Services.Security.Groups
{
	public enum SecurityGroupWarningCode
	{
		// Object
		GroupNotFound,
		GroupsNotFound,
		NullGroup,

		// Database
		DuplicateGroupName,
		PermissionNotFound,
		UserNotFound,
		CannotEditSystemSecurityGroup,
		CannotDeleteSystemSecurityGroup,

		// Properties
		PropertyNotValid,

		// Services

	}
}