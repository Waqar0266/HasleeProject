using System;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Web.Models.Emails;

namespace Hasslefree.Services.Emails.Crud
{
	public class PreviewEmailService : IPreviewEmailService, IInstancePerRequest
	{
		#region Private Properties

		private IGetEmailService GetEmail { get; }
		private ICompileScribanEmail CompileScriban { get; }

		#endregion

		#region Fields

		private EmailCrud _email;

		#endregion

		#region Constructor

		public PreviewEmailService(
				IGetEmailService getEmail,
				ICompileScribanEmail compileScriban
			)
		{
			GetEmail = getEmail;
			CompileScriban = compileScriban;
		}

		#endregion

		#region IPreviewEmailService

		public IPreviewEmailService this[String type]
		{
			get
			{
				_email = GetEmail[type];

				return this;
			}
		}

		public IPreviewEmailService this[Int32 id]
		{
			get
			{
				_email = GetEmail[id];

				return this;
			}
		}

		public String Get(String template)
		{
			return CompileScriban[template, _email.Model];
		}

		#endregion
	}
}
