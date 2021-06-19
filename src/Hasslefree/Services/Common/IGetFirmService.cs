using Hasslefree.Web.Models.Common;

namespace Hasslefree.Services.Common
{
	public interface IGetFirmService
	{
		/// <summary>
		/// Get the details of a single firm
		/// </summary>
		/// <param name="id">The firm identifier</param>
		/// <returns></returns>
		FirmModel Get();
	}
}
