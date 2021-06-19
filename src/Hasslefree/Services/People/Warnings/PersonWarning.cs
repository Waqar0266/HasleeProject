namespace Hasslefree.Services.People.Warnings
{
	public class PersonWarning
	{
		public PersonWarning(PersonWarningCode code, string customMessage = null)
		{
			Code = code;
			CustomMessage = customMessage;
		}

		public PersonWarningCode Code { get; }

		public int Number => (int)Code;

		private string CustomMessage { get; }

		public string Message
		{
			get
			{
				switch (Code)
				{
					// Object
					case PersonWarningCode.PersonNotFound:
						return "Person record was not found.";
					case PersonWarningCode.PersonsNotFound:
						return "Person record(s) were not found.";
					case PersonWarningCode.NullPerson:
						return "Cannot create a new 'Person' as a null object.";

					// Database
					case PersonWarningCode.DuplicateAccountEmail:
						return $"Account record already exists with the Email, {CustomMessage}";
					case PersonWarningCode.DuplicateLoginEmail:
						return $"Login record already exists with the Email, {CustomMessage}";
					case PersonWarningCode.DuplicatePersonEmail:
						return $"Person record already exists with the Email, {CustomMessage}";

					// Property
					case PersonWarningCode.PropertyNotValid:
						return CustomMessage;

					// Services
					case PersonWarningCode.UpdateLoginServiceError:
						return CustomMessage;

					default:
						return "Warning code does not contain a message. Blame the programmer.";
				}
			}
		}
	}
}