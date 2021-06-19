using Hasslefree.Core.Crypto;
using Hasslefree.Core.Sessions;
using Hasslefree.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Profiles
{
	public class ChangeProfilePasswordService : IChangeProfilePasswordService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }

		// Services
		private IUpdateLoginProfileService UpdateLoginProfileService { get; }

		// Other
		private ISessionManager SessionManager { get; }

		#endregion

		#region Fields

		private string _currentPassword;
		private string _newPassword;

		#endregion

		#region Constructor

		public ChangeProfilePasswordService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IUpdateLoginProfileService updateLoginProfileService,
			ISessionManager sessionManager
		)
		{
			// Repos
			LoginRepo = loginRepo;

			// Services
			UpdateLoginProfileService = updateLoginProfileService;

			// Other
			SessionManager = sessionManager;
		}

		#endregion

		#region IChangeProfilePasswordService

		public bool HasWarnings => Warnings.Any();

		public List<ChangeProfilePasswordWarning> Warnings { get; set; } = new List<ChangeProfilePasswordWarning>();

		/// <summary>
		/// Set the current password
		/// </summary>
		/// <param name="currentPassword"></param>
		/// <returns></returns>
		public IChangeProfilePasswordService WithCurrentPassword(string currentPassword)
		{
			if (String.IsNullOrWhiteSpace(currentPassword)) Warnings.Add(new ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode.NoCurrentPassword));

			_currentPassword = currentPassword;
			return this;
		}

		/// <summary>
		/// Set the new password
		/// </summary>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		public IChangeProfilePasswordService WithNewPassword(string newPassword)
		{
			if (String.IsNullOrWhiteSpace(newPassword))
				Warnings.Add(new ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode.NoNewPassword));

			_newPassword = newPassword;
			return this;
		}

		/* Update */
		public bool Update()
		{
			if (HasWarnings) return false;

			// Get the login id
			// NB. Get this from session and not the model for security
			var loginId = SessionManager.Login.LoginId;

			// Get the login
			// NB. Store id not needed because we use login if from session
			var login = LoginRepo.Table.FirstOrDefault(l => l.LoginId == loginId);

			// Login details not found
			if (login == null)
			{
				Warnings.Add(new ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode.NotFound));
				return false;
			}

			// Check if the current password matches what is saved
			if (!ValidateCurrentPassword(login)) return false;

			var salt = Hash.GetSalt();

			var success = UpdateLoginProfileService
				.WithLogin(login)
				.SetLogin(l => l.PasswordSalt, salt)
				.SetLogin(l => l.Password, Hash.GetHashBase64(_newPassword, salt))
				.SetLogin(l => l.ModifiedOn, DateTime.Now)
				.Update();

			if (success) return true;

			if (UpdateLoginProfileService.HasWarnings) UpdateLoginProfileService.Warnings.ForEach(w =>
			{
				Warnings.Add(new ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode.Custom, w.Message));
			});

			return false;
		}

		#endregion

		#region Private

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <returns></returns>
		private bool ValidateCurrentPassword(Core.Domain.Security.Login login)
		{
			if (login.Password == Hash.GetHashBase64(_currentPassword, login.PasswordSalt)) return true;

			Warnings.Add(new ChangeProfilePasswordWarning(ChangeProfilePasswordWarningCode.PasswordsDontMatch));
			return false;
		}

		#endregion
	}
}
