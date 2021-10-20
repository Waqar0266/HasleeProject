namespace Hasslefree.Services.RentalTs.Crud
{
	public class RentalTWarning
	{
		public RentalTWarning(RentalTWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public RentalTWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case RentalTWarningCode.RentalNotFound:
						return "Rental record was not found.";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}
