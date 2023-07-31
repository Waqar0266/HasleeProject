using Hasslefree.Core.Domain.Rentals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hasslefree.Services.RentalTs.Crud
{
    public interface IUpdateTenantService
    {
        bool HasWarnings { get; }
        List<TenantWarning> Warnings { get; }

        IUpdateTenantService this[int tenantId] { get; }

        IUpdateTenantService Set<T>(Expression<Func<Tenant, T>> lambda, object value);

        bool Update(bool saveChanges = true);
    }
}
