using System;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Domain.Emails;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;

namespace Hasslefree.Services.Emails.Crud
{
	public class UpdateEmailService : IUpdateEmailService, IInstancePerRequest
	{
		#region Private Properties

		private IDataRepository<Email> EmailRepo { get; }

		#endregion

		#region Fields

		private Email _email;

		#endregion

		#region Constructor

		public UpdateEmailService(
				IDataRepository<Email> emailRepo
			)
		{
			EmailRepo = emailRepo;
		}

		#endregion

		#region IUpdateEmailService

		public IUpdateEmailService this[String type]
		{
			get
			{
				if (type.Contains("-"))
					type = type.Replace("-", ".");

				_email = EmailRepo.Table.FirstOrDefault(a => a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

				return this;
			}
		}

		public IUpdateEmailService this[Int32 id]
		{
			get
			{
				_email = EmailRepo.Table.FirstOrDefault(a => a.EmailId == id);

				return this;
			}
		}

		public IUpdateEmailService WithSendType(SendType type)
		{
			_email.SendType = type;

			return this;
		}

		public IUpdateEmailService WithSendType(String type)
		{
			_email.SendTypeEnum = type;

			return this;
		}

		public IUpdateEmailService WithSend(bool send)
		{
			_email.Send = send;

			return this;
		}

		public IUpdateEmailService From(String from)
		{
			_email.From = from;

			return this;
		}

		public IUpdateEmailService WithSubject(String subject)
		{
			_email.Subject = subject;

			return this;
		}

		public IUpdateEmailService WithRecipient(String recipient)
		{
			_email.Recipient = recipient;

			return this;
		}

		public IUpdateEmailService WithUrl(String url)
		{
			_email.Url = url;

			return this;
		}

		public IUpdateEmailService WithTemplate(String template)
		{
			_email.Template = template;

			return this;
		}

		public Boolean Update()
		{
			if(_email == null)
				return Clean(false);

			try
			{
				_email.ModifiedOn = DateTime.Now;

				EmailRepo.Update(_email);

				return Clean(true);
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}
			return Clean(false);
		}

		#endregion

		#region Private Methods

		private Boolean Clean(Boolean result)
		{
			_email = null;

			return result;
		}

		#endregion
	}
}
