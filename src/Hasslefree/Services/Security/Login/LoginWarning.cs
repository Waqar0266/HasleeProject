namespace Hasslefree.Services.Security.Login
{
	public class LoginWarning
	{
		public LoginWarning(LoginWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public LoginWarningCode Code { get; }

		public int Number => (int)Code;

		public string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// Object
					case LoginWarningCode.LoginNotFound:
						return "Login record was not found.";
					case LoginWarningCode.LoginsNotFound:
						return "Login records were not found.";
					case LoginWarningCode.NullLogin:
						return "Cannot create a new 'Login' as a null object.";
					case LoginWarningCode.PersonNotFound:
						return "Person record was not found.";
					case LoginWarningCode.SecurityGroupsNotFound:
						return "SecurityGroup record(s) were not found.";

					// Database
					case LoginWarningCode.DuplicateLoginEmail:
						return $"Email is already in use by another login record. [Email: {CustomMessage}]";
					case LoginWarningCode.DuplicatePersonEmail:
						return $"Email is already in use by another person. [Email: {CustomMessage}]";
					case LoginWarningCode.ExistingLoginRecord:
						return "Login record already exists for this person.";

					// Properties
					case LoginWarningCode.PropertyNotValid:
						return CustomMessage;

					// Services
					case LoginWarningCode.UpdatePersonServiceError:
						return CustomMessage;

					default:
						return "Warning code does not contain a message. Blame the programmer.";
				}
			}
		}
	}
}
