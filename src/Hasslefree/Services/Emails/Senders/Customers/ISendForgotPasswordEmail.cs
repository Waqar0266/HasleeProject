using System;

namespace Hasslefree.Services.Emails.Senders.Customers
{
	public interface ISendForgotPasswordEmail
	{
		Boolean this[String email, Int32 otp] { get; }
	}
}
