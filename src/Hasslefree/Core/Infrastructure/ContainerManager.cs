﻿using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hasslefree.Core.Infrastructure
{
	public class ContainerManager
	{
		public ContainerManager(IContainer container)
		{
			Container = container;
		}

		public virtual IContainer Container { get; }

		public virtual T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
		{
			if(scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			if(!IsRegistered(typeof(T), scope))
				return default(T);
			if(string.IsNullOrEmpty(key))
			{
				return scope.Resolve<T>();
			}
			return scope.ResolveKeyed<T>(key);
		}

		public virtual object Resolve(Type type, ILifetimeScope scope = null)
		{
			if(scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			if(!IsRegistered(type, scope))
				return null;
			return scope.Resolve(type);
		}

		public virtual T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
		{
			if(scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			if(!IsRegistered(typeof(T), scope))
				return null;
			if(string.IsNullOrEmpty(key))
			{
				return scope.Resolve<IEnumerable<T>>().ToArray();
			}
			return scope.ResolveKeyed<IEnumerable<T>>(key).ToArray();
		}

		public virtual T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
		{
			return ResolveUnregistered(typeof(T), scope) as T;
		}

		public virtual object ResolveUnregistered(Type type, ILifetimeScope scope = null)
		{
			if(scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			var constructors = type.GetConstructors();
			foreach(var constructor in constructors)
			{
				try
				{
					var parameters = constructor.GetParameters();
					var parameterInstances = new List<object>();
					foreach(var parameter in parameters)
					{
						var service = Resolve(parameter.ParameterType, scope);
						if(service == null) throw new Exception("Unknown dependency");
						parameterInstances.Add(service);
					}
					return Activator.CreateInstance(type, parameterInstances.ToArray());
				}
				catch
				{
					// ignored
				}
			}
			throw new Exception("No constructor was found that had all the dependencies satisfied: " + type.Assembly.FullName);
		}

		public virtual bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
		{
			if (scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			return scope.TryResolve(serviceType, out instance);
		}

		public virtual bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
		{
			if(scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			return scope.IsRegistered(serviceType);
		}

		public virtual object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
		{
			if(scope == null)
			{
				//no scope specified
				scope = Scope();
			}
			return scope.ResolveOptional(serviceType);
		}

		public virtual ILifetimeScope Scope()
		{
			try
			{
				if(HttpContext.Current != null)
					return AutofacDependencyResolver.Current.RequestLifetimeScope;

				//when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
				return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
			}
			catch(Exception)
			{
				//we can get an exception here if RequestLifetimeScope is already disposed
				//for example, requested in or after "Application_EndRequest" handler
				//but note that usually it should never happen

				//when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
				return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
			}
		}
	}
}
