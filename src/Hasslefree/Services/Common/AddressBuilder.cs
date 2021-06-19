using System;
using System.Globalization;
using GoogleMaps.LocationServices;
using Hasslefree.Core.Domain.Common;
using static System.String;

namespace Hasslefree.Services.Common
{
	public class AddressBuilder
	{
		private Core.Domain.Common.Address _address = new Core.Domain.Common.Address();
		private bool _autoCoordinates = true;
		private string _fullAddress;

		/* CTOR */
		internal AddressBuilder(string address1, string address2, string suburb, string town, string country, string region, string code)
		{
			//Set basic info
			_address.Address1 = address1;
			_address.Address2 = address2;
			_address.Address3 = suburb;
			_address.Town = town;
			_address.Country = country;
			_address.RegionName = region;
			_address.Code = code;
			_address.CreatedOn = DateTime.Now;
			_address.ModifiedOn = DateTime.Now;
		}

		public AddressBuilder WithType(AddressType type)
		{
			_address.Type = type;
			return this;
		}

		public AddressBuilder WithCoordinates(string latitude, string longitude)
		{
			_address.Latitude = latitude;
			_address.Longitude = longitude;
			return this;
		}

		public AddressBuilder AutoCoordinates(bool auto = true)
		{
			_autoCoordinates = auto;
			return this;
		}

		internal Core.Domain.Common.Address Build()
		{
			if ((!IsNullOrEmpty(_address.Latitude) && !IsNullOrEmpty(_address.Longitude)) || !_autoCoordinates) return _address;
			GetFullAddress();
			SetCoordinates();
			return _address;
		}

		#region Private Actions

		/// <summary>
		/// Get the full address
		/// </summary>
		/// <returns></returns>
		private void GetFullAddress()
		{
			if (!IsNullOrEmpty(_address.Address1)) _fullAddress += $"{_address.Address1}, ";
			if (!IsNullOrEmpty(_address.Address2)) _fullAddress += $"{_address.Address2}, ";
			if (!IsNullOrEmpty(_address.Address3)) _fullAddress += $"{_address.Address3}, ";
			if (!IsNullOrEmpty(_address.Town)) _fullAddress += $"{_address.Town}, ";
			if (!IsNullOrEmpty(_address.Code)) _fullAddress += $"{_address.Code}, ";
			if (!IsNullOrEmpty(_address.RegionName)) _fullAddress += $"{_address.RegionName}, ";
			if (!IsNullOrEmpty(_address.Country)) _fullAddress += $"{_address.Country}, ";
		}

		private void SetCoordinates()
		{
			if (IsNullOrEmpty(_fullAddress)) return;

			var gls = new GoogleLocationService();
			try
			{
				var latlong = gls.GetLatLongFromAddress(_fullAddress);
				_address.Latitude = latlong?.Latitude.ToString(CultureInfo.InvariantCulture) ?? "";
				_address.Longitude = latlong?.Longitude.ToString(CultureInfo.InvariantCulture) ?? "";
			}
			catch (System.Net.WebException) { }
		}
		#endregion
	}
}
