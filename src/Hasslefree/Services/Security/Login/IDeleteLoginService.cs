using System.Collections.Generic;

namespace Hasslefree.Services.Security.Login
{
	public interface IDeleteLoginService
	{
		bool HasWarnings { get; }
		List<LoginWarning> Warnings { get; }

		IDeleteLoginService this[int loginId] { get; }
		IDeleteLoginService this[List<int> loginIds] { get; }

		bool Remove(bool saveChanges = true);
	}
}
