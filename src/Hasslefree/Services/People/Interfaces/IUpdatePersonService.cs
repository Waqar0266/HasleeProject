using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Services.People.Warnings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.People.Interfaces
{
	public interface IUpdatePersonService
	{
		bool HasWarnings { get; }
		List<PersonWarning> Warnings { get; }

		IUpdatePersonService this[int personId] { get; }
		IUpdatePersonService WithPersonId(int personId);

		IUpdatePersonService SetEmail(string email);
		IUpdatePersonService Set<T>(Expression<Func<Person, T>> lambda, object value);

		IUpdatePersonService SetKeyValue(string key, string value);
		IUpdatePersonService RemoveKeyValue(string key);

		IUpdatePersonService SetAttribute(int attributeValueId);
		IUpdatePersonService RemoveAttribute(int attributeValueId);

		bool Update(bool saveChanges = true);
	}
}
