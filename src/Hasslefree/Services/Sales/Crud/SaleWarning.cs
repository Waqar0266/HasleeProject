namespace Hasslefree.Services.Sales.Crud
{
    public class SaleWarning
    {
		public SaleWarning(SaleWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public SaleWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// object
					case SaleWarningCode.SaleNotFound:
						return "Sale record was not found.";

					default:
						return "Warning code does not have a message. Blame the programmer.";
				}
			}
		}
	}
}
