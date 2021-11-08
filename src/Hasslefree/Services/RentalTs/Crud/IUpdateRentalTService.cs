using Hasslefree.Core.Domain.Rentals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.RentalTs.Crud
{
    public interface IUpdateRentalTService
    {
        bool HasWarnings { get; }
        List<RentalTWarning> Warnings { get; }

        IUpdateRentalTService this[int rentalTId] { get; }

        IUpdateRentalTService Set<T>(Expression<Func<RentalT, T>> lambda, object value);

        bool Update(bool saveChanges = true);
    }
}
