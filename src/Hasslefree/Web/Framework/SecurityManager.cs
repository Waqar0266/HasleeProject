using Hasslefree.Core;
using Hasslefree.Core.Infrastructure;
using System.Linq;

namespace Hasslefree.Web.Framework
{
	public class SecurityManager
	{
		#region Static Engine Context

		public static SecurityManager Current => EngineContext.Current.Resolve<SecurityManager>();

		#endregion

		#region Dependencies

		private IUserContext UserContext { get; }

		#endregion

		#region Constructor

		public SecurityManager
		(
			IUserContext userContext
		)
		{
			UserContext = userContext;
		}

		#endregion

		#region Public Properties

		public bool IsSystemAdmin => GetIsSystemAdmin();

		#endregion

		#region Indexers

		public bool this[string permission] => IsSystemAdmin || HasPermission(permission);

		#endregion

		#region Private Methods

		private bool GetIsSystemAdmin() => UserContext.IsSystemAdmin;

		private bool HasPermission(string permission) => UserContext.Permissions.Any(p => p.PermissionUniqueName.Contains(permission));

		#endregion
	}
}
