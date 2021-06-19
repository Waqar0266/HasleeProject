using System;
using System.Collections.Generic;

namespace Hasslefree.Services.Emails
{
	public interface ISendMail
	{
		IEnumerable<SendEmailWarning> Warnings { get; }

		ISendMail WithRecipient(String email);
		ISendMail WithBcc(String email);
		ISendMail WithCc(String email);
        ISendMail WithAttachment(EmailAttachment attachment);
		ISendMail WithSender(String reply, String from = null);
		ISendMail From(String from);
		ISendMail WithServer(String host, Int32 port, String uid, String pwd, Boolean ssl);
		ISendMail WithUrlBody(String url);
		ISendMail WithTemplateBody(String template, Object model);
		ISendMail WithBody(String body);
		Boolean Send(String subject);
	}
}
