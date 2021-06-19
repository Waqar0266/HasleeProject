using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hasslefree.Services.Emails.Crud
{
	public class ResetEmailService : IResetEmailService, IInstancePerRequest
	{
		#region Constants

		private const String TemplateEmailsPrefix = "Hasslefree.Services.Emails.Templates";

		#endregion

		#region Private Properties

		private IDataRepository<Core.Domain.Emails.Email> EmailRepo { get; }

		#endregion

		#region Fields

		private Core.Domain.Emails.Email _email;

		#endregion

		#region Constructor

		public ResetEmailService(
				IDataRepository<Core.Domain.Emails.Email> emailRepo
			)
		{
			EmailRepo = emailRepo;
		}

		#endregion

		#region IGetEmailService

		public Boolean this[String type]
		{
			get
			{
				if (type.Contains("-"))
					type = type.Replace("-", ".");

				_email = EmailRepo.Table.FirstOrDefault(a => a.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

				return Reset();
			}
		}

		public Boolean this[Int32 id]
		{
			get
			{
				_email = EmailRepo.Table.FirstOrDefault(a => a.EmailId == id);
				return Reset();
			}
		}

		#endregion

		#region Private Methods

		private Boolean Reset()
		{
			if (_email == null)
				return false;

			try
			{
				_email.Template = GetTemplate();
				_email.ModifiedOn = DateTime.Now;

				EmailRepo.Update(_email);

				return true;
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}

			return false;
		}

		public String GetTemplate()
		{
			var type = _email.Type;
			var location = $"{TemplateEmailsPrefix}.{type}.html";

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(location))
			{
				if (stream == null)
					return String.Empty;

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		#endregion
	}
}
