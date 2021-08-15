using Hasslefree.Core.Domain.Rentals;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IUpdateRentalLandlordService
	{
		IUpdateRentalLandlordService this[int rentalLandlordId] { get; }

		IUpdateRentalLandlordService Set<T>(Expression<Func<RentalLandlord, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
