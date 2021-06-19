using log4net;
using System;
using System.Diagnostics;
using System.Web;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Core.Logging
{
	public static class Logger
	{
		private static object _debugLocker = new object();
		private static void Debug(string message)
		{
			lock (_debugLocker)
			{
				SetStoreName();
				log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(HttpContext.Current.Server.MapPath("~/log4net.config")));
				LogManager.GetLogger("root").Debug(message);
			}
		}

		private static object _infoLocker = new object();
		/// <summary>Log an Informational Message</summary>
		private static void Info(string message)
		{
			lock (_infoLocker)
			{
				SetStoreName();
				log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(HttpContext.Current.Server.MapPath("~/log4net.config")));
				LogManager.GetLogger("root").Info(message);
			}
		}

		private static object _errorLocker = new object();
		/// <summary>Log an Error Message</summary>
		private static void Error(string message, Exception ex)
		{
			lock (_errorLocker)
			{
				SetStoreName();
				log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(HttpContext.Current.Server.MapPath("~/log4net.config")));
				//for errors, use the ErrorLog that will write to a flat file (bufferless) rather than database (buffered)
				LogManager.GetLogger("root").Error(message, ex);
			}
		}

		private static void SetStoreName()
		{
			try
			{
				SetParameter("store", "N/A");
			}
			catch
			{
				SetParameter("store", "N/A");
			}
		}

		/// <summary>Insert a parameter to MDC</summary>
		private static void SetParameter(string name, string value)
		{
			ThreadContext.Properties[name] = value;
		}

		public static void LogDebug(string message)
		{
			var frame = new StackFrame(1);
			var method = frame.GetMethod();
			var type = method.DeclaringType;
			var name = method.Name;

			SetParameter("method_name", name);
			SetParameter("class_name", type?.ToString());

			Debug(message);
		}

		public static void LogInfo(string message)
		{
			var frame = new StackFrame(1);
			var method = frame.GetMethod();
			var type = method.DeclaringType;
			var name = method.Name;

			SetParameter("method_name", name);
			SetParameter("class_name", type?.ToString());

			Info(message);
		}

		public static void LogError(Exception ex, string message = "")
		{
			while (ex.InnerException != null)
				ex = ex.InnerException;

			var frame = new StackFrame(1);
			var method = frame.GetMethod();
			var type = method.DeclaringType;
			var name = method.Name;

			SetParameter("method_name", name);
			SetParameter("class_name", type?.ToString());

			Error("Error Encountered in " + method + (!String.IsNullOrWhiteSpace(message) ? (" with message: " + message) : ""), ex);
		}
	}
}
