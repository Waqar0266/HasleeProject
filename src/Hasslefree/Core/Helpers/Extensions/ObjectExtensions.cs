using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class ObjectExtensions
	{
		public static Dictionary<String, Object> ToSnakeDictionary(this Object obj)
		{
			Boolean IsPrimitive(Type t)
			{
				return t.IsPrimitive || t == typeof(Decimal) || t == typeof(String) || t == typeof(DateTime);
			}

			var type = obj.GetType();

			if (type?.FullName?.StartsWith("System.Collections.Generic.Dictionary") ?? false)
			{
				if (obj is Dictionary<String, Object> dicObj)
				{
					return dicObj.ToDictionary(b => b.Key.ToSnakeCase(), c =>
					{
						var value = c.Value;

						if (value == null)
							return null;

						if (value is IEnumerable<Object> enumerable)
						{
							return enumerable.Select(item => item.ToSnakeDictionary()).Cast<Object>().ToList();
						}

						return IsPrimitive(value.GetType()) ? value : c.Value.ToSnakeDictionary();
					});
				}
			}

			var props = type.GetProperties();

			return props.ToDictionary(a => a.Name.ToSnakeCase(), b =>
			{
				var isSfProperty = b?.PropertyType?.Namespace?.StartsWith("Hasslefree") ?? false;
				var value = b.GetValue(obj, null);

				if (value is IEnumerable<Object> enumerable)
				{
					return enumerable.Select(item => item.ToSnakeDictionary()).Cast<Object>().ToList();
				}

				return isSfProperty ? value.ToSnakeDictionary() : value;
			});
		}
	}
}
