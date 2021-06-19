using Hasslefree.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Services.Common
{
	public interface IUpdateFirmService
	{
		IUpdateFirmService WithSettings(string businessName, string tradeName, string phone, string fax, string email, string referenceNumber, string aiNumber);
		IUpdateFirmService WithPostalAddress(string address1, string address2, string address3, string town, string code, string country, string region);
		IUpdateFirmService WithPhysicalAddress(string address1, string address2, string address3, string town, string code, string country, string region);
		void Update();
	}
}
