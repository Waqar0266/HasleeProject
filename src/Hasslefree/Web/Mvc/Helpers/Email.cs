using Hasslefree.Core;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Services.Configuration;
using System;
using System.Configuration;
using System.Web.Mvc;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Core.Managers;

namespace Hasslefree.Web.Mvc.Helpers
{
	public static class Email
	{
		/// <summary>
		/// Complete an image URL if it is not complete
		/// </summary>
		/// <param name="urlScheme"></param>
		/// <param name="urlHost"></param>
		/// <param name="imagePath"></param>
		/// <returns></returns>
		public static string CompleteImageUrl(string urlScheme, string urlHost, string imagePath)
		{
			return CompletePictureUrl(urlScheme, urlHost, imagePath);
		}

		#region Private Methods

		private static object _completePictureUrlLocker = new object();
		/// <summary>
		/// Complete the URL
		/// </summary>
		/// <param name="urlScheme"></param>
		/// <param name="urlHost"></param>
		/// <param name="imagePath"></param>
		/// <returns></returns>
		private static string CompletePictureUrl(string urlScheme, string urlHost, string imagePath)
		{
			lock (_completePictureUrlLocker)
			{
				// Resolve the settings service which is required to complete the URL
				var settingsService = EngineContext.Current.Resolve<ISettingsService>();
				var appSettingsManager = EngineContext.Current.Resolve<IAppSettingsManager>();

				// If none could be found then return blank
				if(String.IsNullOrEmpty(imagePath)) return "";

				// Get the CDNRoot & the media settings
				string cdnRoot = appSettingsManager.CdnRoot;

				// Make sure the CDNRoot starts correctly
				if(cdnRoot.StartsWith("//")) cdnRoot = $"{urlScheme}:{cdnRoot}";

				var mediaSettings = settingsService.LoadSetting<MediaSettings>();

				// If the image starts with a URL then just return it as it is complete
				if(imagePath.StartsWith("https"))
					return imagePath;

				// If it is a relative path then add the URL Scheme
				else if(imagePath.StartsWith("//"))
					imagePath = $"{urlScheme}:{imagePath}";

				// If just an image path & the media settings are set to upload to the cloud then add the CDNRoot
				else if(mediaSettings.MediaStorage == MediaStorage.Cloud)
					imagePath = $"{cdnRoot}/{imagePath.TrimStart('/')}";

				// If just an image path & media settings are set up to upload to disk then add the StoreUrl
				else if(mediaSettings.MediaStorage == MediaStorage.Disk)
					imagePath = $"{urlScheme}://{urlHost}/{imagePath.TrimStart('/')}";

				// If just an image path & media settings are set up to upload to the database then do nothing

				// Return the logo
				return imagePath;
			}
		}

		#endregion
	}
}
