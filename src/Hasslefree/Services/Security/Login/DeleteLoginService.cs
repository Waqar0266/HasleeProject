using Hasslefree.Data;
using System.Collections.Generic;
using System.Linq;
using LoginDb = Hasslefree.Core.Domain.Security.Login;

namespace Hasslefree.Services.Security.Login
{
	public class DeleteLoginService : IDeleteLoginService
	{
		#region Private Properties

		// Repos
		private IDataRepository<LoginDb> LoginRepo { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private List<LoginDb> _usersLogins;

		#endregion

		#region Constructor

		public DeleteLoginService
		(
			IDataRepository<LoginDb> loginRepo,
			IDataContext database
		)
		{
			// Repos
			LoginRepo = loginRepo;

			// Other
			Database = database;
		}

		#endregion

		#region IDeleteLoginDetailsService

		public bool HasWarnings => Warnings.Any();

		public List<LoginWarning> Warnings { get; } = new List<LoginWarning>();

		public IDeleteLoginService this[int loginId]
		{
			get
			{
				if (loginId <= 0) return this;

				var userLogin = LoginRepo.GetById(loginId);

				if (userLogin == null) return this;

				_usersLogins = _usersLogins ?? new List<LoginDb>();

				_usersLogins.Add(userLogin);

				return this;
			}
		}

		public IDeleteLoginService this[List<int> loginIds]
		{
			get
			{
				if (!(loginIds?.Any() ?? false)) return this;

				_usersLogins = LoginRepo.Table
					.Where(l => loginIds.Contains(l.LoginId))
					.ToList();

				return this;
			}
		}

		public bool Remove(bool saveChanges = true)
		{
			if (!IsValid()) return false;

			LoginRepo.Remove(_usersLogins);
			if (saveChanges) Database.SaveChanges();

			return true;
		}

		#endregion

		#region Private Method

		private bool IsValid()
		{
			if (!(_usersLogins?.Any() ?? false))
				Warnings.Add(new LoginWarning(LoginWarningCode.LoginsNotFound));

			return !HasWarnings;
		}

		#endregion
	}
}
