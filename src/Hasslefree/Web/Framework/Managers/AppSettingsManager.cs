using System;
using System.Configuration;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Core.Managers;

namespace Hasslefree.Web.Framework.Managers
{
	public class AppSettingsManager : IAppSettingsManager, IInstancePerRequest
	{
		#region Constructor

		public AppSettingsManager()
		{
			CdnRoot = ConfigurationManager.AppSettings["CDNRoot"].TrimEnd('/');
		}

		#endregion

		#region Statics

		public static IAppSettingsManager Current => EngineContext.Current.Resolve<IAppSettingsManager>();

		#endregion

		#region IAppSettingsManager

		public String CdnRoot { get; }

		#endregion
	}
}
