using Hasslefree.Core;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using static System.String;

namespace Hasslefree.Services.Emails
{
	public class SendMail : ISendMail, IInstancePerRequest
	{
		/* Dependencies */
		private IWebHelper WebHelper { get; }

		/* Services  */
		private ISettingsService SettingsService { get; }

		/* Fields */
		private readonly List<string> _recipients = new List<string>();
		private readonly List<string> _bcc = new List<string>();
		private readonly List<string> _cc = new List<string>();
		private readonly List<Attachment> _attachments = new List<Attachment>();
		private string _from;
		private string _replyTo;
		private bool _overrideSettings;
		private string _host;
		private int _port;
		private string _username;
		private string _password;
		private string _url;
		private readonly List<SendEmailWarning> _warnings = new List<SendEmailWarning>();

		/* Properties */
		public IEnumerable<SendEmailWarning> Warnings => _warnings;

		/* CTOR */
		public SendMail
		(
			IWebHelper webHelper,
			ISettingsService settingsService
		)
		{
			WebHelper = webHelper;
			SettingsService = settingsService;
		}


		public ISendMail WithRecipient(string email)
		{
			_recipients.Add(email);
			return this;
		}

		public ISendMail WithBcc(string email)
		{
			_bcc.Add(email);
			return this;
		}

		public ISendMail WithCc(string email)
		{
			_cc.Add(email);
			return this;
		}

		public ISendMail WithAttachment(Attachment attachment)
		{
			_attachments.Add(attachment);
			return this;
		}

		public ISendMail WithSender(string reply, string from = null)
		{
			_replyTo = reply;
			_from = from;
			return this;
		}

		public ISendMail WithServer(string host, int port, string uid, string pwd)
		{
			_overrideSettings = true;
			_host = host;
			_port = port;
			_username = uid;
			_password = pwd;
			return this;
		}

		public ISendMail WithUrlBody(string url)
		{
			_url = url;
			return this;
		}

		public bool Send(string subject, string recipient = null, string body = null)
		{
			// Clear warnings
			_warnings.Clear();

			// To keep track of any exception
			Exception exception;

			try
			{
				// Get email settings
				if (!_overrideSettings) GetEmailSettings();

				// Add the recipient if any
				if (recipient != null) _recipients.Add(recipient);
				if (_recipients.Count == 0)
				{
					_warnings.Add(new SendEmailWarning(SendEmailWarningCode.NoRecipient));
					return false;
				}

				// Get body from request?
				var html = body;
				if (!IsNullOrWhiteSpace(_url))
				{
					// Relative URL?
					if (!_url.Contains(":"))
					{
						//Strip leading '/'
						if (_url.StartsWith("/")) _url = _url.Substring(1);

						// Get current host
						var host = WebHelper.GetRequestHost();

						// Make absolute 
						_url = $"{WebHelper.GetRequestProtocol()}://{host}/{_url}";
					}

					// Do HTTP request
					html = GetBodyFromUrl(_url);
				}

				// Must have a body
				if (IsNullOrWhiteSpace(html))
				{
					_warnings.Add(new SendEmailWarning(SendEmailWarningCode.NoContent));
					return false;
				}

				//Move CSS to Inline
				var pm = new PreMailer.Net.PreMailer(html);

				// Construct the message
				var message = new MailMessage
				{
					BodyEncoding = Encoding.UTF8, // Allow special characters
					BodyTransferEncoding = TransferEncoding.QuotedPrintable, // Send email as clear text
					IsBodyHtml = true,
					From = new MailAddress(_replyTo, _from),
					SubjectEncoding = Encoding.UTF8, // Allow special characters
					Subject = subject,
					Body = pm.MoveCssInline().Html
				};

				// Add the various recipients
				_recipients.ForEach(r => message.To.Add(r));
				_cc.ForEach(cc => message.CC.Add(cc));
				_bcc.ForEach(bcc => message.Bcc.Add(bcc));
				_attachments.ForEach(att => message.Attachments.Add(att));

				// Create the SMTP client
				using (var client = new SmtpClient(_host, _port))
				{
					// Configure
					client.DeliveryFormat = SmtpDeliveryFormat.SevenBit;

					// Authenticate
					var anonymous = IsNullOrWhiteSpace(_username);
					if (!anonymous)
						client.Credentials = new NetworkCredential(_username, _password);

					// SSL
					client.EnableSsl = true;

					// Send
					client.Send(message);

				}

				_recipients.Clear();
				_cc.Clear();
				_bcc.Clear();
				_overrideSettings = false;

				return true;
			}
			catch (ProtocolViolationException ex1)
			{
				_warnings.Add(new SendEmailWarning(SendEmailWarningCode.FailedHttpRequest));
				exception = ex1;
			}
			catch (WebException ex2)
			{
				_warnings.Add(new SendEmailWarning(SendEmailWarningCode.FailedHttpRequest));
				exception = ex2;
			}
			catch (SmtpException ex3)
			{
				_warnings.Add(new SendEmailWarning(SendEmailWarningCode.FailedSmtpSend));
				exception = ex3;
			}
			catch (Exception ex5)
			{
				_warnings.Add(new SendEmailWarning(SendEmailWarningCode.Unspecified));
				exception = ex5;
			}

			// Log the error
			//LogError(exception);

			_recipients.Clear();
			_cc.Clear();
			_bcc.Clear();
			_overrideSettings = false;

			// Return
			return false;
		}

		#region Private

		/// <summary>
		/// Gets the email settings from either the database or the web.config
		/// </summary>
		private void GetEmailSettings()
		{
			var settings = SettingsService.LoadSetting<EmailSettings>();
			if (!String.IsNullOrEmpty(settings.Host))
			{
				_host = settings.Host;
				_port = settings.Port;
				_username = settings.Username;
				_password = settings.Password;
			}
			else
			{
				var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
				if (smtpSection == null) throw new Exception();

				try
				{
					if (IsNullOrWhiteSpace(_replyTo)) _replyTo = smtpSection.From;
					_host = smtpSection.Network.Host;
					_port = smtpSection.Network.Port;
					_username = smtpSection.Network.UserName;
					_password = smtpSection.Network.Password;
				}

				// Exception if the server was configured correctly
				catch
				{
					throw new Exception();
				}
			}

			if (String.IsNullOrEmpty(_replyTo)) _replyTo = "info@hasslefree.sa.com";
			if (String.IsNullOrEmpty(_from)) _from = "Hasslefree";
		}

		/// <summary>
		/// Get the email message by requesting HTML from a URL
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private static string GetBodyFromUrl(string url)
		{
			// No URL? No request.
			if (IsNullOrWhiteSpace(url)) return null;

			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

			// Create the request
			var request = (HttpWebRequest)WebRequest.Create(url);

			// Get the response
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				if (response.StatusCode != HttpStatusCode.OK) return null;

				using (var stream = response.GetResponseStream())
				{
					if (stream == null) return null;

					// Get the reader
					var reader = response.CharacterSet == null
						? new StreamReader(stream)
						: new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet));

					// Read the text from the stream
					var html = reader.ReadToEnd();

					// Close and dispose the reader
					reader.Close();
					reader.Dispose();

					// Return
					return IsNullOrWhiteSpace(html) ? null : html;
				}
			}
		}
		#endregion
	}
}
