using System;
using Hasslefree.Services.Emails.Definitions;

namespace Hasslefree.Services.Emails
{
	public interface IEmailDefinition
	{
		String Title { get; }
		Object Example { get; }

		InstallDefinition Install { get; }
	}
}
