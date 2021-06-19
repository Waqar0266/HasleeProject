using System.Collections.Generic;

namespace Hasslefree.Services.Cache
{
	public class NoCache : ICacheManager
	{
		public T Get<T>(string key) => default(T);

		public virtual IDictionary<string, object> GetMany(string[] keys)
		{
			var tresult = new Dictionary<string, object>();
			foreach (var k in keys)
				tresult.Add(k, null);
			return tresult;
		}

		public void Set(string key, object data, int cacheTime) { }

		public bool IsSet(string key) => false;

		public void Remove(string key) { }

		public void RemoveByPattern(string pattern) { }

		public void Clear() { }
	}
}
