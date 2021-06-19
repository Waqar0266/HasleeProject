using System;
using System.Collections.Generic;
using System.Linq;
using Hasslefree.Data;

namespace Hasslefree.Services.Common.Addresses
{
	public class DeleteAddressService : IDeleteAddressService
	{
		#region Private Properties

		private IDataRepository<Core.Domain.Common.Address> AddressRepo { get; }

		#endregion

		#region Fields

		private HashSet<Int32> _addressIds = new HashSet<Int32>();

		#endregion

		#region Constructor

		public DeleteAddressService(
				IDataRepository<Core.Domain.Common.Address> addressRepo
			)
		{
			AddressRepo = addressRepo;
		}

		#endregion

		#region IDeleteAddressService

		public IDeleteAddressService this[Int32 id]
		{
			get
			{
				_addressIds.Add(id);
				return this;
			}
		}

		public IDeleteAddressService this[IEnumerable<Int32> ids]
		{
			get
			{
				foreach (var id in ids)
					_addressIds.Add(id);

				return this;
			}
		}

		public Boolean Delete()
		{
			try
			{
				if (!_addressIds.Any())
					return Clean(false);

				DeleteAddresses();

				return Clean(true);
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}
			return Clean(false);
		}

		#endregion

		#region Private Methods

		private Boolean Clean(Boolean success)
		{
			_addressIds = new HashSet<Int32>();

			return success;
		}

		private void DeleteAddresses()
		{
			AddressRepo.Table.Where(a => _addressIds.Contains(a.AddressId)).DeleteFromQuery();
		}

		#endregion
	}
}
