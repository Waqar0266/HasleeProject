namespace Hasslefree.Services.Security.AuthorizedUsers.Crud
{
	public class AuthorizedUserWarning
	{
		public AuthorizedUserWarning(AuthorizedUserWarningCode code)
		{
			Code = code;
		}

		public AuthorizedUserWarningCode Code { get; }

		public int Number => (int)Code;

		public string Message
		{
			get
			{
				switch (Code)
				{
					case AuthorizedUserWarningCode.UserNotFound:
						return "User was not found / does not exist.";
					case AuthorizedUserWarningCode.UsersNotFound:
						return "User(s) were not found found / do not exist.";
					case AuthorizedUserWarningCode.SecurityGroupsNotFound:
						return "No Security Groups were selected/found. Please ensure that the user is added to at least one Security Group.";
					default:
						return "No message found for this Warning code. Blame the programmer.";
				}
			}
		}
	}
}
