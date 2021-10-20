using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Services.People.Warnings;
using System;
using System.Collections.Generic;

namespace Hasslefree.Services.People.Interfaces
{
	public interface ICreatePersonService
	{
		bool HasWarnings { get; }
		List<PersonWarning> Warnings { get; }
		Guid PersonGuid { get; }
		int PersonId { get; }
		int LoginId { get; }

		ICreatePersonService New(string firstName, string middleNames, string surname, string email, Titles title = Titles.Mr, string alias = null, Gender gender = Gender.Male, string idNumber = null, PersonStatus status = PersonStatus.Enabled, string tag = null);
		ICreatePersonService WithContactDetails(string phone = null, string fax = null, string mobile = null);

		ICreatePersonService WithKeyValue(string key, string value);
		ICreatePersonService WithAttribute(int attributeId, int attributeValueId);

		ICreatePersonService WithPassword(string password, string passwordSalt);
		ICreatePersonService WithSecurityGroup(string securityGroup);

		bool Create();
	}
}