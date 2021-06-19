namespace Hasslefree.Services.Agents.Crud
{
	public class AgentWarning
	{
		public AgentWarning(AgentWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public AgentWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case AgentWarningCode.AgentNotFound:
						return "Agent record was not found.";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}