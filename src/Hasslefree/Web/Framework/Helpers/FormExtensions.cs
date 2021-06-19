using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hasslefree.Web.Framework.Helpers
{
	public static class FormExtensions
	{

		public static T ToObject<T>(this Dictionary<String, String> source) where T : class, new()
		{
			var someObject = new T();
			var someObjectType = someObject.GetType();

			foreach(var sourceItem in source)
			{
				// Check if its a list
				if(IsListItem(sourceItem.Key))
				{
					someObject.SetListProperty(sourceItem.Key, source);
				}
				else
				{
					var prop = someObjectType.GetProperty(sourceItem.Key);
					if(prop == null)
						continue;

					someObject.SetSingleProperty(sourceItem.Key, sourceItem.Value);
				}
			}

			return someObject;
		}

		private static Boolean IsListItem(String key)
		{
			return key.Contains("].");
		}

		public static void SetSingleProperty<T>(this T obj, String key, String value) where T : class
		{
			var type = obj.GetType();

			var prop = type.GetProperty(key);
			if(prop == null)
				return;

			if(prop.PropertyType == typeof(Boolean))
			{
				Boolean.TryParse(value, out var boolVal);
				prop.SetValue(obj, boolVal, null);
			}

			else if(prop.PropertyType == typeof(Int32))
			{
				Int32.TryParse(value, out var intVal);
				prop.SetValue(obj, intVal, null);
			}
			else if(prop.PropertyType == typeof(Decimal))
			{
				Decimal.TryParse(value, out var decVal);
				prop.SetValue(obj, decVal, null);
			}
			else
				prop.SetValue(obj, value, null);
		}

		public static void SetListProperty<T>(this T obj, String sourceKey, Dictionary<String, String> source) where T : class
		{
			var type = obj.GetType();

			var key = sourceKey.Substring(0, sourceKey.IndexOf('['));
			var index = Int32.Parse(sourceKey.Substring(sourceKey.IndexOf('[') + 1, sourceKey.IndexOf(']') - key.Length - 1));

			var listProp = type.GetProperty(key);

			if(listProp == null)
				return;

			if(listProp.PropertyType.IsGenericType && listProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
			{
				//check if property is null or if get some value, don't want to rewrite values from database
				var value = listProp.GetValue(obj);

				var itemType = listProp.PropertyType.GetGenericArguments()[0];
				var constructedListType = typeof(List<>).MakeGenericType(itemType);
				Object list = null;
				if(value != null)
				{
					list = Convert.ChangeType(value, constructedListType);
					var count = (Int32)list.GetType().GetProperty("Count").GetValue(list);
					if(count == index + 1)
						return;
				}

				var startsWith = $"{key}[{index}].";
				var dict = source.Where(s => s.Key.StartsWith(startsWith))
					.ToDictionary(a => a.Key.Substring(startsWith.Length), b => b.Value);

				var toObj = typeof(FormExtensions).GetMethod("ToObject");

				if(toObj == null)
					return;

				var valObj = toObj.MakeGenericMethod(itemType).Invoke(null, new Object[] { dict });

				var addToList = constructedListType.GetMethod("Add");

				if(addToList == null)
					return;

				if(list == null)
					list = Activator.CreateInstance(constructedListType);

				addToList.Invoke(list, new[] { valObj });
				listProp.SetValue(obj, list);
			}
		}

	}
}
