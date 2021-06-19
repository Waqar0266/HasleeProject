using System;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.ModelBuilders.Customers
{
	public interface IBuildOtpEmailModel
	{
		IBuildOtpEmailModel this[Int32 pin] { get; }

		OtpEmailModel Get();
	}
}
