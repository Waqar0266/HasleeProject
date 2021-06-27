namespace Hasslefree.Services.AgentForms
{
	public class AgentFormWarning
	{
		public AgentFormWarning(AgentFormWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public AgentFormWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case AgentFormWarningCode.AgentNotFound:
						return "Agent record was not found.";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}