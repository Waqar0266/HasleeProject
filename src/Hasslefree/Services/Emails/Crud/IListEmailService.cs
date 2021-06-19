using Hasslefree.Web.Models.Emails;
using System.Collections.Generic;

namespace Hasslefree.Services.Emails.Crud
{
	public interface IListEmailService
	{
		IListEmailService StartsWithType(string type);
		List<EmailSettingsModel> List();
	}
}
