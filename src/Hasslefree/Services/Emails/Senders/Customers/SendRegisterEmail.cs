using System;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Emails;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Services.Emails.ModelBuilders;
using Hasslefree.Services.Emails.ModelBuilders.Customers;

namespace Hasslefree.Services.Emails.Senders.Customers
{
	public class SendRegisterEmail : ISendRegisterEmail, IInstancePerRequest
	{
		#region Constants

		private const String Type = "Customers.Register";

		#endregion

		#region Private Properties

		private IBuildEmailModel BuildEmail { get; }
		private IBuildCustomerEmailModel BuildCustomer { get; }
		private IGetSenderEmailService GetSenderEmail { get; }
		private ISendMail Send { get; }

		#endregion

		#region Constructor

		public SendRegisterEmail(
				IBuildEmailModel buildEmail,
				IBuildCustomerEmailModel buildCustomer,
				IGetSenderEmailService getSenderEmail,
				ISendMail send
			)
		{
			BuildEmail = buildEmail;
			BuildCustomer = buildCustomer;
			GetSenderEmail = getSenderEmail;
			Send = send;
		}

		#endregion

		#region ISendRegisterEmail

		public Boolean this[Person person]
		{
			get
			{
				try
				{
					var senderEmail = GetSenderEmail[Type];

					if (!senderEmail.Send)
						return true;

					var recipients = new List<string>() { person.Email };

					switch (senderEmail.SendType)
					{
						case SendType.Default:
							{
								foreach (var recipient in recipients)
									Send.WithRecipient(recipient).WithUrlBody(String.Format(senderEmail.Url, recipient)).From(senderEmail.From).Send(senderEmail.Subject);

								return true;
							}
						case SendType.Custom:
							{
								var model = BuildEmail["Registration", senderEmail.Subject, senderEmail.From].WithModel("Customer", BuildCustomer[person.PersonId]).Get();
								foreach (var recipient in recipients)
									Send.WithRecipient(recipient).WithTemplateBody(senderEmail.Template, model).From(senderEmail.From).Send(senderEmail.Subject);
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
