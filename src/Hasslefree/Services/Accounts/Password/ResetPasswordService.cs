using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using Hasslefree.Core.Crypto;
using Hasslefree.Data;
using Hasslefree.Services.Accounts.Actions;

namespace Hasslefree.Services.Accounts.Password
{
	public class ResetPasswordService : IResetPasswordService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }

		// Services
		private ILoginService LoginService { get; }

		#endregion

		#region Constructor

		public ResetPasswordService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			ILoginService loginService
		)
		{
			// Repos
			LoginRepo = loginRepo;

			// Services
			LoginService = loginService;
		}

		#endregion

		#region IResetPasswordService

		public bool HasWarnings => Warnings.Any();

		public List<ResetPasswordWarning> Warnings { get; } = new List<ResetPasswordWarning>();

		public bool ResetPassword(string email, string password, string hash, string otp)
		{
			// Initialize
			Warnings.Clear();

			// Trim the input
			email = email?.Trim();
			password = password?.Trim();
			otp = otp?.Trim();

			// Check for null
			if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException();
			if (String.IsNullOrWhiteSpace(password)) throw new ArgumentNullException();
			if (String.IsNullOrWhiteSpace(otp)) throw new ArgumentNullException();

			// Fetch the person
			var login = LoginRepo.Table
									.Include(l => l.Person)
									.FirstOrDefault(p => p.Email == email);
			if (login == null)
			{
				Warnings.Add(new ResetPasswordWarning(ResetPasswordWarningCode.NotFound));
				return false;
			}

			// Compute the hash
			var hash2 = Hash.GetHashBase64(login.Person.PersonGuid.ToString(), otp);
			if (hash != hash2)
			{
				Warnings.Add(new ResetPasswordWarning(ResetPasswordWarningCode.NotValid));
				return false;
			}

			// Reset the password
			var salt = Hash.GetSalt();
			login.PasswordSalt = salt;
			login.Password = Hash.GetHashBase64(password, salt);

			// Update and login
			using (var scope = new TransactionScope())
			{
				LoginRepo.Update(login);
				
				LoginService
					.WithEmail(login.Email)
					.WithPassword(password)
					.Login();

				scope.Complete();
			}
			
			return true;
		}

		#endregion
	}
}
