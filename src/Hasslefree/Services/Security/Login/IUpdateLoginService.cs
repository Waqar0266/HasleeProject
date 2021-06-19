using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Security.Login
{
	public interface IUpdateLoginService
	{
		bool HasWarnings { get; }
		List<LoginWarning> Warnings { get; }

		IUpdateLoginService this[int loginId] { get; }

		IUpdateLoginService WithLoginId(int loginId);
		IUpdateLoginService WithPersonId(int personId);
		IUpdateLoginService WithLogin(Core.Domain.Security.Login login);
		IUpdateLoginService WithEmail(string email);
		
		IUpdateLoginService SetEmail(string email, bool updatePerson = false);
		IUpdateLoginService Set<T>(Expression<Func<Core.Domain.Security.Login, T>> lambda, object value);
		IUpdateLoginService SetPassword(string name, string salt = null);

		IUpdateLoginService WithSecurityGroup(int securityGroupId);
		IUpdateLoginService RemoveSecurityGroup(int securityGroupId);

		bool Update(bool saveChanges = true);
	}
}