using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using Hasslefree.Services.Configuration;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Emails.Senders.Customers;
using Hasslefree.Web.Models.Accounts;

namespace Hasslefree.Services.Accounts.Otp
{
	public class RegisterOtpService : IRegisterOtpService
	{
		#region Constants

		private const string SecretHashValue = "secret:hash-value:for-registration";

		#endregion

		#region Private Properties

		private IDataRepository<Session> SessionRepo { get; }

		private ISendRegisterOtpEmail SendRegisterOtpEmail { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Fields

		private string _email;
		private string _hash;
		private int _otp;

		#endregion

		#region Constructor

		public RegisterOtpService
		(
			IDataRepository<Session> sessionRepo,
			ISendRegisterOtpEmail sendRegisterOtpEmail,
			ISessionManager sessionManager
		)
		{
			SessionRepo = sessionRepo;
			SendRegisterOtpEmail = sendRegisterOtpEmail;
			SessionManager = sessionManager;
		}

		#endregion

		#region IRegisterOtpService

		public bool HasWarnings => Warnings.Any();

		public List<RegisterOtpWarning> Warnings { get; } = new List<RegisterOtpWarning>();

		public IRegisterOtpService WithEmail(string email)
		{
			_email = email;

			if (String.IsNullOrWhiteSpace(_email)) Warnings.Add(new RegisterOtpWarning(RegisterOtpWarningCode.InvalidEmail));

			return this;
		}

		public IRegisterOtpService WithHash(string hash)
		{
			_hash = hash;

			if (String.IsNullOrWhiteSpace(hash)) Warnings.Add(new RegisterOtpWarning(RegisterOtpWarningCode.InvalidHash));

			return this;
		}

		public IRegisterOtpService GenerateHash(RegisterModel model)
		{
			var session = SessionRepo.GetById(SessionManager.Session.SessionId);

			_otp = new Random().Next(100000, 999999);

			var input = $"{_email}:{SecretHashValue}-{session.SessionId}:{session.CreatedOn}:{_otp}";

			byte[] hash;
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			}

			_hash = hash.Aggregate("", (current, byt) => current + byt.ToString("X"));

			return this;
		}

		public IRegisterOtpService WithOtp(string otp)
		{
			var valid = int.TryParse(otp, out _otp);

			if (!valid) Warnings.Add(new RegisterOtpWarning(RegisterOtpWarningCode.InvalidOtp));

			return this;
		}

		public bool ValidateOtp()
		{
			if (HasWarnings) return false;

			var session = SessionRepo.GetById(SessionManager.Session.SessionId);

			var input = $"{_email}:{SecretHashValue}-{session.SessionId}:{session.CreatedOn}:{_otp}";

			byte[] hash;
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
			}

			var match = _hash == hash.Aggregate("", (current, byt) => current + byt.ToString("X"));

			if (!match) Warnings.Add(new RegisterOtpWarning(RegisterOtpWarningCode.InvalidOtp));

			return match;
		}

		public bool SendOtp(out string hash)
		{
			hash = String.Empty;

			if (HasWarnings) return false;

			hash = _hash;

			if (SendEmail(_email, _otp)) return true;

			Warnings.Add(new RegisterOtpWarning(RegisterOtpWarningCode.EmailFailed));

			return false;
		}

		#endregion

		#region Private Methods

		private bool SendEmail(string email, int otp)
		{
			return SendRegisterOtpEmail[email, otp];
		}

		#endregion
	}
}
