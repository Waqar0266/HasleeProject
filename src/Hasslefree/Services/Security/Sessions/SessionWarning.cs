namespace Hasslefree.Services.Security.Sessions
{
	public class SessionWarning
	{
		public enum SessionWarningCode
		{
			// Object
			SessionNotFound
		}

		public SessionWarning(SessionWarningCode code)
		{
			Code = code;
		}

		public SessionWarningCode Code { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					case SessionWarningCode.SessionNotFound:
						return "Session record was not found.";
					default:
						return "Warning code does not contain a message. Blame the programmer.";
				}
			}
		}
	}
}
