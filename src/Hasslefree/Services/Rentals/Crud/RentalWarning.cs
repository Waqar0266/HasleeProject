namespace Hasslefree.Services.Rentals.Crud
{
    public class RentalWarning
    {
		public RentalWarning(RentalWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public RentalWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case RentalWarningCode.RentalNotFound:
						return "Rental record was not found.";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}
