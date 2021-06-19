using System.Runtime.CompilerServices;

namespace Hasslefree.Core.Infrastructure
{
	public class EngineContext
	{
		/// <summary>
		/// Initializes a static instance of the factory
		/// </summary>
		/// <param name="forceRecreate">Creates a new factory instance even though the factory has been previously initialized.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static IEngine Initialize(bool forceRecreate)
		{
			if (Singleton<IEngine>.Instance == null || forceRecreate)
				Singleton<IEngine>.Instance = CreateEngineInstance();
			return Singleton<IEngine>.Instance;
		}

		/// <summary>
		/// Creates a factory instance and adds a http application injecting facility.
		/// </summary>
		/// <returns>New engine instance</returns>
		private static IEngine CreateEngineInstance() => new HasslefreeEngine();

		private static object _currentSingletonLocker = new object();

		/// <summary>
		/// Get the currenty instance of the engine
		/// </summary>
		public static IEngine Current
		{
			get
			{
				lock (_currentSingletonLocker)
				{
					if(Singleton<IEngine>.Instance == null)
						Initialize(false);
					return Singleton<IEngine>.Instance;
				}
			}
		}
	}
}
