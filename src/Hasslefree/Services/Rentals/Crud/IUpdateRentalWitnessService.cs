using Hasslefree.Core.Domain.Rentals;
using System;
using System.Linq.Expressions;

namespace Hasslefree.Services.Rentals.Crud
{
	public interface IUpdateRentalWitnessService
	{
		int RentalWitnessId { get; }
		IUpdateRentalWitnessService this[int rentalWitnessId] { get; }
		IUpdateRentalWitnessService WithRentalId(int rentalId);

		IUpdateRentalWitnessService Set<T>(Expression<Func<RentalWitness, T>> lambda, object value);

		bool Update(bool saveChanges = true);
	}
}
