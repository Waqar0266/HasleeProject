using Hasslefree.Core.Configuration;

namespace Hasslefree.Services.Configuration
{
	public interface ISettingsService
	{
		T LoadSetting<T>(string extraIdentifier = "") where T : ISettings, new();
		void SaveSetting<T>(T settings, string extraIdentifier = "") where T : ISettings, new();
		void DeleteSetting<T>(string extraIdentifier = "") where T : ISettings, new();
	}
}
