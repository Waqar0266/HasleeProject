using Hasslefree.Core.Configuration;
using Hasslefree.Services.Configuration;
using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using Hasslefree.Core.Infrastructure.Email;

namespace Hasslefree.Services.Infrastructure.Email
{
	[Obsolete("Deprecated. Please do not use services from the 'Hasslefree' Project.")]
	public class SMTP : IEmailService, IDisposable
	{
		//Service dependencies
		private ISettingsService Settings { get; }

		//The email settings (with hard coded values for fallback to Amazon)
		private string _host = "email-smtp.us-east-1.amazonaws.com";
		private string _username = "AKIAIRXY7YSIMAJKOQOA";
		private string _password = "AqveOQrrkx18Uj/dxz3rThjgk4nJDGK7DDC5RpFAwvdY";
		private int _port = 25;
		private bool _ssl = true;

		//Keep track if the service has been initialized or not
		private bool _initialized;

		//The email client
		private SmtpClient _client;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings"></param>
		public SMTP(ISettingsService settings)
		{
			Settings = settings;
		}

		/// <summary>
		/// Initialize the service
		/// </summary>
		private void Init()
		{
			var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
			if (smtpSection != null)
			{
				_host = smtpSection.Network.Host;
				_port = smtpSection.Network.Port;
				_username = smtpSection.Network.UserName;
				_password = smtpSection.Network.Password;
				_ssl = smtpSection.Network.EnableSsl;
			}

			//Create the Smtp client
			_client = new SmtpClient(_host, _port)
			{
				Credentials = new NetworkCredential(_username, _password),
				EnableSsl = _ssl,
			};

			//Don't initialize again
			_initialized = true;
		}

		#region IEmailService
		/// <summary>
		/// Send an email
		/// </summary>
		/// <param name="message"></param>
		public void SendEmail(EmailMessage message)
		{
			//Initialize the service if not initialized before
			if (!_initialized) Init();

			try
			{
				_client.Send(message.ToMailMessage());
			}
			catch (Exception ex)
			{
				// Get the inner-most exception
				while (ex.InnerException != null) ex = ex.InnerException;

				// Log the exception
				Core.Logging.Logger.LogError(ex, "EmailSendError");
			}
		}

		#endregion

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			if (_client != null) _client.Dispose();
		}
	}
}
