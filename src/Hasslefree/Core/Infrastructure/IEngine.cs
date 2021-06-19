using System;

namespace Hasslefree.Core.Infrastructure
{
	public interface IEngine
	{
		/// <summary>
		/// The container manager
		/// </summary>
		ContainerManager ContainerManager { get; }

		/// <summary>
		/// Resolve dependency
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <returns></returns>
		T Resolve<T>() where T : class;
		
		/// <summary>
		///  Resolve dependency
		/// </summary>
		/// <param name="type">Type</param>
		/// <returns></returns>
		object Resolve(Type type);

		/// <summary>
		/// Resolve dependencies
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <returns></returns>
		T[] ResolveAll<T>();
	}
}
