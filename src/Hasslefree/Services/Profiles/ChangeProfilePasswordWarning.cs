namespace Hasslefree.Services.Profiles
{
	public class ChangeProfilePasswordWarning
	{
		/// <summary>
		/// Create a product create warning using code
		/// </summary>
		/// <param name="code"></param>
		public ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode code)
		{
			Code = code;
		}

		public ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode code, string message)
		{
			Code = code;
			CustomMessage = message;
		}

		/// <summary>
		/// The Product Create Warning Code
		/// </summary>
		public ChangeProfilePasswordWarningCode Code { get; }

		/// <summary>
		/// The numeric value for the warning code
		/// </summary>
		public int Number => (int)Code;

		public string CustomMessage { get; }

		/// <summary>
		/// Message of the warning
		/// </summary>
		public string Message
		{
			get
			{
				switch (Code)
				{
					case ChangeProfilePasswordWarningCode.NotFound:
						return "An account with this email can't be found.";
					case ChangeProfilePasswordWarningCode.NoCurrentPassword:
						return "Please enter a password for 'Current Password'.";
					case ChangeProfilePasswordWarningCode.NoNewPassword:
						return "Please enter a value for 'New Password'.";
					case ChangeProfilePasswordWarningCode.PasswordsDontMatch:
						return "Your 'Current Password' is incorrect.";
					case ChangeProfilePasswordWarningCode.Custom:
						return CustomMessage;
					default:
						return "Unexpected error has occured. How did you end up here?";
				}
			}
		}
	}
}
