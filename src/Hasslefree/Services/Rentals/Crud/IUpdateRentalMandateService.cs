using Hasslefree.Core.Domain.Rentals;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IUpdateRentalMandateService
	{
		IUpdateRentalMandateService this[int rentalMandateId] { get; }

		IUpdateRentalMandateService Set<T>(Expression<Func<RentalMandate, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
