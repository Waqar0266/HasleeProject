namespace Hasslefree.Services.Sales.Crud
{
    public class SellerWarning
    {
        public SellerWarning(SellerWarningCode code, string customMessage = null)
        {
            Code = code;
            CustomMessage = customMessage;
        }

        public SellerWarningCode Code { get; }

        public int Number => (int)Code;

        private string CustomMessage { get; }

        public string Message
        {
            get
            {
                switch (Code)
                {
                    // object
                    case SellerWarningCode.SellerNotFound:
                        return "Seller record was not found.";

                    default:
                        return "Warning code does not have a message. Blame the programmer.";
                }
            }
        }
    }
}
