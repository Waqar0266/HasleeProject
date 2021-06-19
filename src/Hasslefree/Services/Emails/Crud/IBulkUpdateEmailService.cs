using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Services.Emails.Crud
{
	public interface IBulkUpdateEmailService
	{
		IBulkUpdateEmailService WithEmailSetting(int id, string from, string subject, string recipient, bool send);
		bool Update(bool saveChanges = true);
	}
}
