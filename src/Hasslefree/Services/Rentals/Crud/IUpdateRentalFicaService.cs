using Hasslefree.Core.Domain.Rentals;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IUpdateRentalFicaService
	{
		IUpdateRentalFicaService this[int rentalFicaId] { get; }
		IUpdateRentalFicaService WithRentalId(int rentalId);

		IUpdateRentalFicaService Set<T>(Expression<Func<RentalFica, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
