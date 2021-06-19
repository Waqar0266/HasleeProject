namespace Hasslefree.Services.Profiles
{
	public class UpdateProfileWarning
	{
		/* CTOR */
		public UpdateProfileWarning(UpdateProfileWarningCode code)
		{
			Code = code;
		}

		/* Properties */
		public UpdateProfileWarningCode Code { get; }
		public int Number => (int)Code;

		/// <summary>
		/// Get the message corresponding to the code
		/// </summary>
		public string Message
		{
			get
			{
				switch (Code)
				{
					case UpdateProfileWarningCode.AccountNotFound:
						return "The account does not exist.";
					case UpdateProfileWarningCode.AddressNotFound:
						return "The address can't be found.";
					case UpdateProfileWarningCode.PersonNotFound:
						return "The person does not exist.";
					case UpdateProfileWarningCode.PropertyNotFound:
						return "The property can't be found.";
					case UpdateProfileWarningCode.Restricted:
						return "The property you are trying modify may not be updated.";
					case UpdateProfileWarningCode.DuplicateEmail:
						return "The email is already registered. Please use a different one";
					case UpdateProfileWarningCode.LoginNotFound:
						return "No login credentials were found.";
					default:
						return "This message should never ever be reached. How did you get this?";
				}
			}
		}
	}
}
