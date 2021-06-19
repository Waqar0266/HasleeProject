using Hasslefree.Core.Domain.Media;
using Hasslefree.Services.Configuration;
using System;
using System.Web.Mvc;
using Hasslefree.Core.Configuration;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Web.Mvc.Helpers
{
	public static class PicturePath
	{
		public static String GetDownloadPath(this Download download)
		{
			if (download == null)
				return "javascript:void(0)";

			ISettingsService settingsService = EngineContext.Current.Resolve<ISettingsService>();

			MediaSettings mediaSettings = settingsService.LoadSetting<MediaSettings>();
			if (mediaSettings == null) mediaSettings = new MediaSettings();

			if (download.MediaStorage == MediaStorage.Database) return "/backoffice/media/download?downloadid=" + download.DownloadId;
			if (download.MediaStorage == MediaStorage.Cloud)
			{
				return download.RelativeFolderPath;
			}
			else
			{
				return mediaSettings.StorageRootPath + download.RelativeFolderPath;
			}
		}
	}
}
