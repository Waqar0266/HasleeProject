namespace Hasslefree.Services.Accounts.Actions
{
	public class LoginWarning
	{
		public LoginWarning(LoginWarningCode code)
		{
			Code = code;
		}

		public LoginWarningCode Code { get; }

		public int Number => (int)Code;

		public string Message
		{
			get
			{
				switch (Code)
				{
					case LoginWarningCode.NotFound: // "An account with this email address does not exist"
					case LoginWarningCode.InvalidCredentials: // "Incorrect password"
						return "Invalid login details.";
					case LoginWarningCode.AccountDisabled:
						return "Login for this account has been disabled.";
					case LoginWarningCode.AccountNotValid:
						return "There is something wrong with your account.";
					case LoginWarningCode.PendingApproval:
						return "Your account is pending approval";
					case LoginWarningCode.AccountBlocked:
						return "Your account has been blocked.";
					case LoginWarningCode.LoginDeactivate:
						return "The entered Login details have been deactivated.";
					default:
						return "How did you end up with this error?";
				}
			}
		}
	}
}
