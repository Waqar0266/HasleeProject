using System;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using Hasslefree.Web.Models.Emails;

namespace Hasslefree.Services.Emails.Crud
{
	public class GetEmailService : IGetEmailService, IInstancePerRequest
	{
		#region Constants

		private const String EmailsPrefix = "Hasslefree.Services.Emails.Definitions";

		#endregion

		#region Private Properties

		private IReadOnlyRepository<Core.Domain.Emails.Email> EmailRepo { get; }
		private ISettingsService Settings { get; }

		#endregion

		#region Fields

		private Core.Domain.Emails.Email _email;

		#endregion

		#region Constructor

		public GetEmailService(
				IReadOnlyRepository<Core.Domain.Emails.Email> emailRepo,
				ISettingsService settings
			)
		{
			EmailRepo = emailRepo;
			Settings = settings;
		}

		#endregion

		#region IGetEmailService

		public EmailCrud this[String type]
		{
			get
			{
				if (type.Contains("-"))
					type = type.Replace("-", ".");

				_email = EmailRepo.Table.FirstOrDefault(a => a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

				var crud = GetCrud();

				_email = null;

				return crud;
			}
		}

		public EmailCrud this[Int32 id]
		{
			get
			{
				_email = EmailRepo.Table.FirstOrDefault(a => a.EmailId == id);

				var crud = GetCrud();

				_email = null;

				return crud;
			}
		}

		#endregion

		#region Private Methods

		private EmailCrud GetCrud()
		{
			if (_email == null)
				return null;

			try
			{
				var model = GetModel();
				return new EmailCrud()
				{
					Id = _email.EmailId,
					Type = _email.SendTypeEnum,
					Send = _email.Send,
					Url = _email.Url,
					From = _email.From,
					FromAddress = "noreply@example.com",
					Subject = _email.Subject,
					Recipient = _email.Recipient,
					Template = _email.Template,
					Model = model.Model,
					Title = model.Title
				};
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}

			return null;
		}

		private (Object Model, string Title) GetModel()
		{
			var iCastType = typeof(IEmailDefinition);

			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => p != null && 
							(p.FullName?.StartsWith(EmailsPrefix) ?? false) && 
							!p.IsInterface &&
							!p.IsAbstract &&
							iCastType.IsAssignableFrom(p))
				.Select(a => a).ToList();

			var type = types.FirstOrDefault(a => a.FullName == $"{EmailsPrefix}.{_email.Type}");

			if (type == null)
				return (null,null);

			var instance = (IEmailDefinition)Activator.CreateInstance(type);

			return (instance.Example?.ToSnakeDictionary(), instance.Title);
		}

		#endregion
	}
}
