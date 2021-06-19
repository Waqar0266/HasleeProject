using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Profiles
{
	public interface IUpdateProfileService
	{
		bool HasWarnings { get; }
		List<UpdateProfileWarning> Warnings { get; }

		AddressBuilder AddAddress(string address1, string address2, string suburb, string town, string country, string region, string code);
		IUpdateProfileService SetPerson<T>(Expression<Func<Person, T>> lambda, object value);
		IUpdateProfileService SetAddress<T>(int addressId, Expression<Func<Address, T>> lambda, object value);
		IUpdateProfileService DeleteAddress(int addressId);
		IUpdateProfileService UsePerson();

		bool Update();
	}
}
