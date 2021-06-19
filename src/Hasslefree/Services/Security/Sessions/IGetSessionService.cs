using Hasslefree.Web.Models.Security.Sessions.Get;

namespace Hasslefree.Services.Security.Sessions
{
	public interface IGetSessionService
	{
		SessionWarning Warning { get; }

		SessionGet this[int sessionId, bool includeDates = true] { get; }
	}
}
