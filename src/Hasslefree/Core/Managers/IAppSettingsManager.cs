using System;

namespace Hasslefree.Core.Managers
{
	public interface IAppSettingsManager
	{
		String CdnRoot { get; }
	}

	public static class AppSettingsManagerExtensions
	{
		public static String PrependCdnRoot(this IAppSettingsManager appSettings, String path)
		{
			if (String.IsNullOrWhiteSpace(path))
				return path;

			if (path.StartsWith("/"))
				path = path.TrimStart('/');

			return $"{appSettings.CdnRoot}/{path}";
		}
	}
}
