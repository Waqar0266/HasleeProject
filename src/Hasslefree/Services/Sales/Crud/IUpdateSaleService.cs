using Hasslefree.Core.Domain.Sales;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.Sales.Crud
{
    public interface IUpdateSaleService
    {
        bool HasWarnings { get; }
        List<SaleWarning> Warnings { get; }

        IUpdateSaleService this[int saleId] { get; }

        IUpdateSaleService Set<T>(Expression<Func<Sale, T>> lambda, object value);

        bool Update(bool saveChanges = true);
    }
}
