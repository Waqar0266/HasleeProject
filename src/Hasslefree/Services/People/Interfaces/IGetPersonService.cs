using Hasslefree.Services.People.Warnings;
using Hasslefree.Web.Models.People.Get;

namespace Hasslefree.Services.People.Interfaces
{
	public interface IGetPersonService
	{
		PersonWarning Warning { get; }
		
		PersonGet this[int personId, bool includeDates = true] { get; }

		PersonGet this[string email,bool includeDates = true] { get; }
	}
}
