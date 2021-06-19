using Hasslefree.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Hasslefree.Services.Cache
{
	/// <summary>
	/// Represents a MemoryCacheCache
	/// </summary>
	public class MemoryCacheManager : ICacheManager
	{
		private IWebHelper WebHelper { get; }
		private string KeyPrefix { get; }

		public MemoryCacheManager(IWebHelper webHelper)
		{
			WebHelper = webHelper;
			KeyPrefix = $"{WebHelper.ServerVariables("HTTP_HOST")}:";
		}

		/// <summary>
		/// Generate the cache key with the correct prefix
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		internal string GetKey(string key) => $"{KeyPrefix}{key}";

		/// <summary>
		/// Get the cache property
		/// </summary>
		private ObjectCache Cache => MemoryCache.Default;

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="key">The key of the value to get.</param>
		/// <returns>The value associated with the specified key.</returns>
		public virtual T Get<T>(string key) => (T)Cache[GetKey(key)];

		public virtual IDictionary<string, object> GetMany(string[] keys)
		{
			var tresult = new Dictionary<string, object>();
			foreach (var k in keys)
			{
				var val = Get<object>(GetKey(k));
				tresult.Add(k, val);
			}
			return tresult;
		}

		/// <summary>
		/// Adds the specified key and object to the cache.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="data">Data</param>
		/// <param name="cacheTime">Cache time</param>
		public virtual void Set(string key, object data, int cacheTime)
		{
			if (data == null) return;

			var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes((cacheTime)) };
			Cache.Add(new CacheItem(GetKey(key), data), policy);
		}

		/// <summary>
		/// Gets a value indicating whether the value associated with the specified key is cached
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>Result</returns>
		public virtual bool IsSet(string key) => Cache.Contains(GetKey(key));

		/// <summary>
		/// Removes the value with the specified key from the cache
		/// </summary>
		/// <param name="key">/key</param>
		public virtual void Remove(string key)
		{
			//Check if Key Prefix is already present, if not, add it
			if (!key.StartsWith(KeyPrefix)) key = KeyPrefix + key;
			Cache.Remove(key);
		}

		/// <summary>
		/// Removes items by pattern
		/// </summary>
		/// <param name="pattern">pattern</param>
		public virtual void RemoveByPattern(string pattern)
		{
			var regex = new Regex(GetKey(pattern), RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
			var keysToRemove = new List<string>();

			foreach (var item in Cache)
				if (regex.IsMatch(item.Key))
					keysToRemove.Add(item.Key);

			foreach (var key in keysToRemove)
				Remove(key);
		}

		/// <summary>
		/// Clear all cache data (only within key prefix)
		/// </summary>
		public virtual void Clear()
		{
			foreach (var item in Cache.Where(c => c.Key.StartsWith(KeyPrefix)))
				Remove(item.Key);
		}
	}
}