using System;
using Hasslefree.Web.Models.Emails;

namespace Hasslefree.Services.Emails.Crud
{
	public interface IGetEmailService
	{
		EmailCrud this[String type] { get; }
		EmailCrud this[Int32 id] { get; }
	}
}
