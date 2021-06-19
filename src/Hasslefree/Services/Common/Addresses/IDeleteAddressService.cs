using System;
using System.Collections.Generic;

namespace Hasslefree.Services.Common.Addresses
{
	public interface IDeleteAddressService
	{
		IDeleteAddressService this[Int32 id] { get; }
		IDeleteAddressService this[IEnumerable<Int32> ids] { get; }

		Boolean Delete();
	}
}
