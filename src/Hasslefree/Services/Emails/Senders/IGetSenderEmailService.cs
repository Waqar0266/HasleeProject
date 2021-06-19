using System;
using Hasslefree.Core.Domain.Emails;

namespace Hasslefree.Services.Emails.Senders
{
	public interface IGetSenderEmailService
	{
		Email this[String type] { get; }
	}
}
