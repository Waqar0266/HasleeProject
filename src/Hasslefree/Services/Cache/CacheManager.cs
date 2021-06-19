using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Services.Cache
{
	public class CacheManager
	{
		/// <summary>
		/// Gets an instance of the SessionManager from the engine context
		/// </summary>
		public static ICacheManager Current => EngineContext.Current.Resolve<ICacheManager>();
	}
}
