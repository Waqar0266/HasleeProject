using Hasslefree.Core.Domain.Rentals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
    public interface IUpdateRentalService
    {
		bool HasWarnings { get; }
		List<RentalWarning> Warnings { get; }

		IUpdateRentalService this[int rentalId] { get; }

		IUpdateRentalService Set<T>(Expression<Func<Rental, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
