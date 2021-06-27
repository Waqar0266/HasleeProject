﻿using FluentValidation.Mvc;
using Hasslefree.Core;
using Hasslefree.Core.Infrastructure;
using System;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hasslefree.Business
{
	public class MvcApplication : HttpApplication
	{
		/// <summary>
		/// Setup the application
		/// </summary>
		protected void Application_Start()
		{
			// Load everything?
			BuildManager.GetReferencedAssemblies();

			// Register all third party licenses
			LicenseConfig.Register();

			//Start the Hasslefree application engine
			EngineContext.Initialize(false);

			GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;

			//Map the attribute routes
			RouteTable.Routes.MapMvcAttributeRoutes();

			//Register the routes in all areas
			AreaRegistration.RegisterAllAreas();

			//Register the default roots
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			//Register FluentValidation.Mvc5
			FluentValidationModelValidatorProvider.Configure();

			MvcHandler.DisableMvcResponseHeader = true;
		}

		protected void Application_PreSendRequestHeaders()
		{
			if (HttpContext.Current != null)
			{
				HttpContext.Current.Response.Headers.Remove("Server");
			}
		}

		public override string GetVaryByCustomString(HttpContext context, string arg)
		{
			switch (arg)
			{
				case "HostName":
					var webHelper = EngineContext.Current.Resolve<IWebHelper>();
					return webHelper.ServerVariables("HTTP_HOST");
			}

			return base.GetVaryByCustomString(context, arg);
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			// Get the exception object.
			var ex = Server.GetLastError();

			// TODO : Custom Error handling
		}
	}
}