using System.Reflection;
using Autofac;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Data
{
	[CoreRegistrar]
	public class DependencyRegistrar : IDependencyRegistrar
	{
		public void RegisterDependencies(ContainerBuilder builder)
		{
			var assembly = Assembly.GetExecutingAssembly();

			builder.RegisterAssemblyTypes(assembly)
				.AssignableTo<IInstancePerRequest>()
				.AsImplementedInterfaces()
				.InstancePerRequest();
		}
	}
}
