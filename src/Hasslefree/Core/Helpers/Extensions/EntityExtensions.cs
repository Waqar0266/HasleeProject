using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class EntityExtensions
	{
		//public static void SetModifiedOn(this T entity) where T : <BaseEntity>
		//{
		//	var property = typeof(T).GetProperty("ModifiedOnUtc");
		//	property.SetValue(entity,DateTime.UtcNow);
		//}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
			(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var known = new HashSet<TKey>();
			return source.Where(element => known.Add(keySelector(element)));
		}

		public static IQueryable<TSource> DistinctBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
		{
			return source.GroupBy(keySelector).Select(x => x.FirstOrDefault());
		}

		/// <summary>
		/// Convert a list to HashSet
		/// </summary>
		/// <typeparam name="T">Type of list</typeparam>
		/// <param name="source">List</param>
		/// <returns></returns>
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			var hs = new HashSet<T>();

			foreach(var s in source)
				if (!hs.Contains(s))
					hs.Add(s);

			return hs;
		}

	}
}
