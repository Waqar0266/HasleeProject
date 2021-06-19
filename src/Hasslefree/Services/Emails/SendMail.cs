using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using Hasslefree.Core;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using static Hasslefree.Core.Logging.Logger;
using static System.String;

namespace Hasslefree.Services.Emails
{
	public class SendMail : ISendMail, IInstancePerRequest
	{
		/* Dependencies */
		private IWebHelper WebHelper { get; }
		private ISettingsService Settings { get; }
		private ICompileScribanEmail CompileScriban { get; }

		/* Fields */
		private Boolean _isTemplate;
		private String _template;
		private Object _model;

		private String _body;

		private readonly List<String> _recipients = new List<String>();
		private readonly List<String> _bcc = new List<String>();
		private readonly List<String> _cc = new List<String>();
		private readonly List<EmailAttachment> _attachments = new List<EmailAttachment>();
		private String _from;
		private String _replyTo;
		private Boolean _overrideSender;
		private Boolean _overrideSettings;
		private String _host;
		private Int32 _port;
		private String _username;
		private String _password;
		private Boolean _ssl;
		private String _url;
		private readonly List<SendEmailWarning> _warnings = new List<SendEmailWarning>();

		/* Properties */
		public IEnumerable<SendEmailWarning> Warnings => _warnings;

		/* CTOR */
		public SendMail
		(
			IWebHelper webHelper,
			ISettingsService settingsService,
			ICompileScribanEmail compileScriban

		)
		{
			WebHelper = webHelper;
			Settings = settingsService;
			CompileScriban = compileScriban;
		}


		public ISendMail WithRecipient(String email)
		{
			if (email.Contains(","))
			{
				var emails = email.Split(',').Where(a => !IsNullOrWhiteSpace(a)).ToList();
				if (emails.Any()) _recipients.AddRange(emails);
			}
			else
				_recipients.Add(email);

			return this;
		}

		public ISendMail WithBcc(String email)
		{
			if (email.Contains(","))
			{
				var emails = email.Split(',').Where(a => !IsNullOrWhiteSpace(a)).ToList();
				if (emails.Any()) _bcc.AddRange(emails);
			}
			else
				_bcc.Add(email);

			return this;
		}

		public ISendMail WithCc(String email)
		{
			if (email.Contains(","))
			{
				var emails = email.Split(',').Where(a => !IsNullOrWhiteSpace(a)).ToList();
				if (emails.Any()) _cc.AddRange(emails);
			}
			else
				_cc.Add(email);

			return this;
		}

		public ISendMail WithSender(String reply, String from = null)
		{
			_overrideSender = true;
			_replyTo = reply;
			_from = from;
			return this;
		}

		public ISendMail From(String from)
		{
			_overrideSender = true;
			_from = from;
			return this;
		}

		public ISendMail WithAttachment(EmailAttachment attachment)
		{
			_attachments.Add(attachment);
			return this;
		}


		public ISendMail WithServer(String host, Int32 port, String uid, String pwd, Boolean ssl)
		{
			_overrideSettings = true;
			_host = host;
			_port = port;
			_username = uid;
			_password = pwd;
			_ssl = ssl;
			return this;
		}

		public ISendMail WithUrlBody(String url)
		{
			_url = url;
			return this;
		}

		public ISendMail WithTemplateBody(String template, Object model)
		{
			_isTemplate = true;
			_template = template;
			_model = model;

			return this;
		}

		public ISendMail WithBody(String body)
		{
			_body = body;

			return this;
		}

		public Boolean Send(String subject)
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
				if (_recipients.Count == 0)
				{
					_warnings.Add(new SendEmailWarning(SendEmailWarningCode.NoRecipient));
					return false;
				}

				// Get body from request?
				var body = _body;

				if (String.IsNullOrWhiteSpace(body))
					body = GetBody();

				// Must have a body
				if (IsNullOrWhiteSpace(body))
				{
					_warnings.Add(new SendEmailWarning(SendEmailWarningCode.NoContent));
					return false;
				}

				// Construct the message
				var message = new MailMessage
				{
					BodyEncoding = Encoding.UTF8, // Allow special characters
					BodyTransferEncoding = TransferEncoding.QuotedPrintable, // Send email as clear text
					IsBodyHtml = true,
					From = new MailAddress(_replyTo, _from),
					SubjectEncoding = Encoding.UTF8, // Allow special characters
					Subject = subject,
					Body = body
				};

				// Add the various recipients
				_recipients.ForEach(r => message.To.Add(r));
				_cc.ForEach(cc => message.CC.Add(cc));
				_bcc.ForEach(bcc => message.Bcc.Add(bcc));

				// Add the attachments
				_attachments.ForEach(att => message.Attachments.Add(new Attachment(att.Data, att.Filename)));

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
					client.EnableSsl = _ssl;

					// Send
					client.Send(message);

				}

				_recipients.Clear();
				_cc.Clear();
				_bcc.Clear();
				_overrideSettings = false;
				_attachments.Clear();
				_isTemplate = false;
				_template = null;
				_model = null;
				_body = null;


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
			LogError(exception);

			_recipients.Clear();
			_cc.Clear();
			_bcc.Clear();
			_overrideSettings = false;
			_attachments.Clear();
			_isTemplate = false;
			_template = null;
			_model = null;
			_body = null;

			// Return
			return false;
		}

		#region Private

		/// <summary>
		/// Gets the email settings from either the database or the web.config
		/// </summary>
		private void GetEmailSettings()
		{
			var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

			try
			{
				if (IsNullOrWhiteSpace(_replyTo)) _replyTo = smtpSection.From;
				_host = smtpSection.Network.Host;
				_port = smtpSection.Network.Port;
				_username = smtpSection.Network.UserName;
				_password = smtpSection.Network.Password;
				_ssl = smtpSection.Network.EnableSsl;
			}

			// Exception if the server was configured correctly
			catch
			{
				
			}
		}

		private String GetHost()
		{
			return $"{WebHelper.GetRequestProtocol()}://{WebHelper.GetRequestHost()}";
		}

		private String GetBody()
		{
			if (_isTemplate)
				return CompileScriban[_template, _model];
			else if (!IsNullOrWhiteSpace(_url))
				return RequestBodyFromUrl();

			return null;
		}

		private String RequestBodyFromUrl()
		{
			// Relative URL?
			if (!_url.Contains(":"))
			{
				//Strip leading '/'
				if (_url.StartsWith("/")) _url = _url.Substring(1);

				// Get current host
				var host = GetHost();

				// Make absolute 
				_url = $"{host}/{_url}";
			}

			// Do HTTP request
			var body = RequestBody();

			//Move CSS to Inline
			var pm = new PreMailer.Net.PreMailer(body, HttpContext.Current.Request.Url);
			return pm.MoveCssInline().Html;
		}

		/// <summary>
		/// Get the email message by requesting HTML from a URL
		/// </summary>
		/// <returns></returns>
		private String RequestBody()
		{
			// No URL? No request.
			if (IsNullOrWhiteSpace(_url)) return null;

			// Create the request
			var request = (HttpWebRequest)WebRequest.Create(_url);
			request.UserAgent = "Hasslefree-Bot";

			//Ignore any SSL issues
			request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

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
