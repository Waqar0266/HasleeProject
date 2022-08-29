using Hasslefree.Core.Domain.Sales;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Sales.Crud
{
    public interface IUpdateSellerService
    {
        bool HasWarnings { get; }
        List<SellerWarning> Warnings { get; }

        IUpdateSellerService this[int sellerId] { get; }

        IUpdateSellerService Set<T>(Expression<Func<Seller, T>> lambda, object value);

        bool Update(bool saveChanges = true);
    }
}
