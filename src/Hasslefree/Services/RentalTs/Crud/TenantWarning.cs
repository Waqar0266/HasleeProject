namespace Hasslefree.Services.RentalTs.Crud
{
    public class TenantWarning
    {
        public TenantWarning(TenantWarningCode code, string customMessage = null)
        {
            Code = code;
            CustomMessage = customMessage;
        }

        public TenantWarningCode Code { get; }

        public int Number => (int)Code;

        private string CustomMessage { get; }

        public string Message
        {
            get
            {
                switch (Code)
                {
                    // object
                    case TenantWarningCode.TenantNotFound:
                        return "Tenant record was not found.";

                    default:
                        return "Warning code does not have a message. Blame the programmer.";
                }
            }
        }
    }
}
