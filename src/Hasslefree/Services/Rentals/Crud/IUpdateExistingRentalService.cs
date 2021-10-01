using Hasslefree.Core.Domain.Rentals;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IUpdateExistingRentalService
	{
		IUpdateExistingRentalService this[int existingRentalId] { get; }

		IUpdateExistingRentalService Set<T>(Expression<Func<ExistingRental, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
