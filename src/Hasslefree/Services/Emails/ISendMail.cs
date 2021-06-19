using System.Collections.Generic;
using System.Net.Mail;

namespace Hasslefree.Services.Emails
{
	public interface ISendMail
	{
		IEnumerable<SendEmailWarning> Warnings { get; }

        ISendMail WithRecipient(string email);
        ISendMail WithBcc(string email);
        ISendMail WithCc(string email);
        ISendMail WithSender(string reply, string from = null);
        ISendMail WithServer(string host, int port, string uid, string pwd, bool ssl);
        ISendMail WithUrlBody(string url);
        ISendMail WithAttachment(Attachment attachment);
        bool Send(string subject, string recipient = null, string body = null);
	}
}
