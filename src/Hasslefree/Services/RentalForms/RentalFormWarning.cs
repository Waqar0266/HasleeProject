namespace Hasslefree.Services.RentalForms
{
	public class RentalFormWarning
	{
		public RentalFormWarning(RentalFormWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public RentalFormWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case RentalFormWarningCode.RentalNotFound:
						return "Rental record was not found.";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}
