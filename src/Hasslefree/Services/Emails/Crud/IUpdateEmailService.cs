using System;
using Hasslefree.Core.Domain.Emails;

namespace Hasslefree.Services.Emails.Crud
{
	public interface IUpdateEmailService
	{
		IUpdateEmailService this[String type] { get; }
		IUpdateEmailService this[Int32 id] { get; }

		IUpdateEmailService WithSendType(SendType type);
		IUpdateEmailService WithSendType(String type);
		IUpdateEmailService WithSend(bool send);
		IUpdateEmailService From(String from);
		IUpdateEmailService WithSubject(String subject);
		IUpdateEmailService WithRecipient(String recipient);
		IUpdateEmailService WithUrl(String url);
		IUpdateEmailService WithTemplate(String template);

		Boolean Update();
	}
}
