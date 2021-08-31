using Hasslefree.Core.Domain.Rentals;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IUpdateRentalResolutionMemberService
	{
		IUpdateRentalResolutionMemberService this[int rentalResolutionId] { get; }
		IUpdateRentalResolutionMemberService WithRentalId(int rentalId);

		IUpdateRentalResolutionMemberService Set<T>(Expression<Func<RentalResolutionMember, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
