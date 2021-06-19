using System;

namespace Hasslefree.Services.Cache
{
	public static class CacheExtensions
	{
		/// <summary>
		/// Get an object from the cache or acquire it if it is not in the cache
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cacheManager"></param>
		/// <param name="key"></param>
		/// <param name="cacheTime"></param>
		/// <param name="acquire"></param>
		/// <returns></returns>
		public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
		{
			//Return the item from the cache
			if(cacheManager.IsSet(key)) return cacheManager.Get<T>(key);

			//Get the object from the acquirer0
			var result = acquire();

			//Store the object in the cache
			if(result != null) cacheManager.Set(key, result, cacheTime);

			return result;
		}
	}
}