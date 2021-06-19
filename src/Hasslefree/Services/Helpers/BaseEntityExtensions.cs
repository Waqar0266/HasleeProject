using Hasslefree.Core.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Hasslefree.Services.Helpers
{
	internal static class BaseEntityExtensions
	{
		internal static TEntity SetPropertyValue<TEntity, TProperty>(this TEntity entity, Expression<Func<TEntity, TProperty>> lambda, object value, params string[] restrictedProperties) where TEntity : BaseEntity
		{
			// Return if no product or standard prices
			if (entity == null) return null;

			// Get the selected property
			if (!(lambda.Body is MemberExpression selector)) throw new ArgumentException(nameof(lambda));

			// Get property info
			var property = selector.Member as PropertyInfo;
			if (property == null) throw new ArgumentException(nameof(lambda));

			// Return if value is unchanged
			if (property.GetValue(entity)?.ToString() == value?.ToString()) return entity;

			// Set value if it's not a restricted property
			if (!restrictedProperties?.Contains(property.Name) ?? true) property.SetValue(entity, value, null);

			return entity;
		}
	}
}
