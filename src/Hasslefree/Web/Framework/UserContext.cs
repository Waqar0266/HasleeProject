using Hasslefree.Core;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Cache;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Web.Framework
{
	public class UserContext : IUserContext
	{
		#region Private Properties

		private ICacheManager CacheManager { get; }
		private ISessionManager SessionManager { get; }

		#endregion

		#region Fields

		private Login _login;
		private List<SecurityGroup> _securityGroups;
		private List<Permission> _permissions;
		private bool _isSystemAdmin;

		#endregion

		#region Constructor

		public UserContext
		(
			ICacheManager cacheManager,
			ISessionManager sessionManager
		)
		{
			CacheManager = cacheManager;
			SessionManager = sessionManager;
		}

		#endregion

		#region Properties

		public static IUserContext Current => EngineContext.Current.Resolve<IUserContext>();

		public Login Login
		{
			get
			{
				if (_login != null) return _login;
				LoadUserContext();
				return _login;
			}
		}

		public List<SecurityGroup> SecurityGroups
		{
			get
			{
				if (_securityGroups != null) return _securityGroups;
				LoadUserContext();
				return _securityGroups;
			}
		}

		public List<Permission> Permissions
		{
			get
			{
				if (_permissions != null) return _permissions;
				LoadUserContext();
				return _permissions;
			}
		}

		public bool IsSystemAdmin
		{
			get
			{
				if (_login == null) LoadUserContext();
				return _isSystemAdmin;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Loads the data needed by the user context
		/// </summary>
		private void LoadUserContext()
		{
			_login = SessionManager.Login;
			if (_login == null) return;

			_securityGroups = GetSecurityGroups();
			_permissions = GetPermissions();
			_isSystemAdmin = GetIsSystemAdmin();
		}

		/// <summary>
		/// Get the security groups for the logged in user
		/// </summary>
		/// <returns></returns>
		private List<SecurityGroup> GetSecurityGroups()
		{
			return CacheManager.Get(CacheKeys.Store.Account.Login.SecurityGroups(0, _login.LoginId), CacheKeys.Time.LongTime,
				() => _login.SecurityGroupLogins.Select(sgl => sgl.SecurityGroup).ToList());
		}

		/// <summary>
		/// Get the persmissions for the logged in user
		/// </summary>
		/// <returns></returns>
		private List<Permission> GetPermissions()
		{
			return CacheManager.Get(CacheKeys.Store.Account.Login.Permissions(0, _login.LoginId), CacheKeys.Time.LongTime,
				() => _securityGroups.SelectMany(sg => sg.Permissions).Distinct().ToList());
		}

		/// <summary>
		/// Does the logged in user have System Access?
		/// </summary>
		/// <returns></returns>
		private bool GetIsSystemAdmin()
		{
			return CacheManager.Get(CacheKeys.Store.Account.Login.IsSystemAdmin(0, _login.LoginId), CacheKeys.Time.LongTime, () =>
			{
				return _permissions.Any(p => p.PermissionUniqueName.Contains("SysAdmin")) || 
					   _securityGroups.Any(sg => sg.IsSystemSecurityGroup);
			});
		}

		#endregion
	}
}
