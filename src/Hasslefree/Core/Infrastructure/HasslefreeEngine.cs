using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Hosting;
using System.Reflection;
using System.IO;
#pragma warning disable 618

namespace Hasslefree.Core.Infrastructure
{
	public class HasslefreeEngine : IEngine
	{
		private ContainerManager _container;

		#region Constructor

		/// <summary>
		/// Creates a new instance of the engine and registers all dependencies
		/// </summary>
		public HasslefreeEngine()
		{
			RegisterDependencies();
		}

		#endregion

		#region IEngine

		public ContainerManager ContainerManager => _container;

		/// <summary>
		/// Resolve the specified type from the container
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Resolve<T>() where T : class =>  _container?.Resolve<T>();

		/// <summary>
		/// Resolve the specified type from the container
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object Resolve(Type type) => _container?.Resolve(type);

		/// <summary>
		/// Resolve dependencies
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <returns></returns>
		public T[] ResolveAll<T>() => _container?.ResolveAll<T>();

		#endregion

		#region Methods

		/// <summary>
		/// Register dependencies
		/// </summary>
		private void RegisterDependencies()
		{
			string logFolder = HostingEnvironment.ApplicationPhysicalPath + "/Logs/";
			try
			{
				//Create an empty container (we will update it several times since you can only build it once)
				var builder = new ContainerBuilder();

				//Register the engine as a dependency
				builder.RegisterInstance(this).As<IEngine>().SingleInstance();

				//Find other dependency registrars
				var registrars = AppDomain.CurrentDomain
											  .GetAssemblies()
											  .SelectMany(a => a.GetTypes())
											  .Where(t => typeof(IDependencyRegistrar).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
											  .OrderBy(t => t.GetCustomAttributes(typeof(CoreRegistrarAttribute)).Any() ? 0 : 1)
											  .Select(c => Activator.CreateInstance(c) as IDependencyRegistrar);

				//Register the other dependencies
				foreach (var registrar in registrars)
				{
					if (registrar == null) continue;
					registrar.RegisterDependencies(builder);
				}

				var container = builder.Build();

				//Set the container in the engine
				_container = new ContainerManager(container);

				//Set the dependency resolver for MVC
				DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

				//Set the dependency resolver for Web API
				GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
			}
			catch (ReflectionTypeLoadException ex)
			{
				string message = ex.Message;
				foreach (var loaderEx in ex.LoaderExceptions)
					message += "\n" + loaderEx.Message;

				if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
				File.AppendAllText(logFolder + "PluginLoaderExceptions.txt", "EXCEPTION:" + Environment.NewLine + message);
			}
		}

		#endregion
	}
}
