namespace Hasslefree.Services.Security.Groups
{
	public class SecurityGroupWarning
	{
		public SecurityGroupWarning(SecurityGroupWarningCode code, string customMessage = null)
		{
			Code = code;
			_customMessage = customMessage;
		}

		public SecurityGroupWarningCode Code { get; set; }

		public int Number => (int)Code;

		private readonly string _customMessage;

		public string Message
		{
			get
			{
				switch (Code)
				{
					// Object
					case SecurityGroupWarningCode.GroupNotFound:
						return $"Security Group record was not found. [ID: {_customMessage}]";
					case SecurityGroupWarningCode.GroupsNotFound:
						return "Security Group record(s) were not found.";
					case SecurityGroupWarningCode.NullGroup:
						return "Cannot create a new 'Security Group' as a null object.";

					// Database
					case SecurityGroupWarningCode.DuplicateGroupName:
						return $"A Security Group record already exists with the name. [Name: {_customMessage}]";
					case SecurityGroupWarningCode.PermissionNotFound:
						return $"Permission record was not found. [ID: {_customMessage}]";
					case SecurityGroupWarningCode.UserNotFound:
						return $"Login record was not found. [ID: {_customMessage}]";
					case SecurityGroupWarningCode.CannotEditSystemSecurityGroup:
						return "Cannot edit a system Security Group.";
					case SecurityGroupWarningCode.CannotDeleteSystemSecurityGroup:
						return "Cannot delete a system Security Group.";

					// Properties
					case SecurityGroupWarningCode.PropertyNotValid:
						return _customMessage;

					default:
						return "Warning code does not contain a message. Blame the programmer.";
				}
			}
		}
	}
}
