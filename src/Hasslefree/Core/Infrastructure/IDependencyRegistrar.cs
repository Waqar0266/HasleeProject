using System;
using Autofac;

namespace Hasslefree.Core.Infrastructure
{
	public interface IDependencyRegistrar
	{
		void RegisterDependencies(ContainerBuilder builder);
	}

	public class CoreRegistrarAttribute : Attribute
	{
		
	}
}
