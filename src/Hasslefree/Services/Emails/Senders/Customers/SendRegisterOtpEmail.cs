using System;
using Hasslefree.Core.Domain.Emails;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Services.Emails.ModelBuilders;
using Hasslefree.Services.Emails.ModelBuilders.Customers;

namespace Hasslefree.Services.Emails.Senders.Customers
{
	public class SendRegisterOtpEmail : ISendRegisterOtpEmail, IInstancePerRequest
	{
		#region Constants

		private const String Type = "Customers.RegisterOtp";

		#endregion

		#region Private Properties

		private IBuildEmailModel BuildEmail { get; }
		private IBuildOtpEmailModel BuildOtp { get; }
		private IGetSenderEmailService GetSenderEmail { get; }
		private ISendMail Send { get; }

		#endregion

		#region Constructor

		public SendRegisterOtpEmail(
				IBuildEmailModel buildEmail,
				IBuildOtpEmailModel buildOtp,
				IGetSenderEmailService getSenderEmail,
				ISendMail send
			)
		{
			BuildEmail = buildEmail;
			BuildOtp = buildOtp;
			GetSenderEmail = getSenderEmail;
			Send = send;
		}

		#endregion

		#region ISendRegisterEmail

		public Boolean this[String email, Int32 otp]
		{
			get
			{
				try
				{
					var senderEmail = GetSenderEmail[Type];

					if (!senderEmail.Send)
						return true;

					switch (senderEmail.SendType)
					{
						case SendType.Default:
							{
								Send.WithRecipient(email).WithUrlBody(senderEmail.Url).From(senderEmail.From).Send(senderEmail.Subject);

								return true;
							}
						case SendType.Custom:
							{
								var model = BuildEmail["Register OTP", senderEmail.Subject, senderEmail.From].WithModel("Otp", BuildOtp[otp]).Get();
								Send.WithRecipient(email).WithTemplateBody(senderEmail.Template, model).From(senderEmail.From).Send(senderEmail.Subject);
								return true;
							}
						default:
							return true;
					}
				}
				catch (Exception ex)
				{
					Core.Logging.Logger.LogError(ex);
				}

				return false;
			}
		}

		#endregion
	}
}
