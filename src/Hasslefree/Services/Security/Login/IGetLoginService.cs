using Hasslefree.Web.Models.Security.Login.Get;

namespace Hasslefree.Services.Security.Login
{
	public interface IGetLoginService
	{
		LoginWarning Warning { get; }

		LoginGet this[int loginId, bool includeDates = true] { get; }
	}
}
