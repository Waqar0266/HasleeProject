using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Security;

namespace Hasslefree.Core.Sessions
{
	public interface ISessionManager
	{
		Session Session { get; }
		Login Login { get; }
		LocationModel Location { get; }

		#region Public Methods

		ISessionManager WithReference(string reference);
		void ClearSessionCache();
		void ClearCartCache();
		void ClearLoginCache();
		void ClearAccountCache();
		bool IsLoggedIn();

		#endregion
	}
}
