using System.Collections.Generic;

namespace Hasslefree.Services.Security.Login
{
	public interface ICreateLoginService
	{
		bool HasWarnings { get; }
		List<LoginWarning> Warnings { get; }
		int LoginId { get; }

		ICreateLoginService New(int personId, string email, string salutation, bool active);
		ICreateLoginService WithPassword(string password, string passwordSalt);

		ICreateLoginService WithSecurityGroup(int securityGroupId);
		ICreateLoginService WithSecurityGroups(IEnumerable<int> securityGroupIds);

		bool Create(bool saveChanges = true);
	}
}
