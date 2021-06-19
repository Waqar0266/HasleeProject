using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Profiles
{
	public interface IUpdateLoginProfileService
	{
		bool HasWarnings { get; }
		List<UpdateProfileWarning> Warnings { get; }

		IUpdateLoginProfileService WithLoginId(int loginId);
		IUpdateLoginProfileService WithLogin(Core.Domain.Security.Login login);
		IUpdateLoginProfileService SetLogin<T>(Expression<Func<Core.Domain.Security.Login, T>> lambda, object value);

		bool Update();
	}
}
