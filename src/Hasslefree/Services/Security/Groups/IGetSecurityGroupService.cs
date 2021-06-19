using Hasslefree.Web.Models.Security.SecurityGroups.Get;

namespace Hasslefree.Services.Security.Groups
{
	public interface IGetSecurityGroupService
	{
		SecurityGroupWarning Warning { get; }

		SecurityGroupGet this[int id, bool includeDates = true] { get; }
	}
}