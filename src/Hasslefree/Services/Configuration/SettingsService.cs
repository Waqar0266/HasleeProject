using Hasslefree.Core;
using Hasslefree.Core.Configuration.Annotations;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Z.EntityFramework.Plus;
using static System.String;

namespace Hasslefree.Services.Configuration
{
	public class SettingsService : ISettingsService
	{
		#region Constants

		private const string CacheKey = "Hasslefree.Cache.Settings()";
		private const string CachePattern = "Hasslefree.Cache.Settings";

		#endregion

		#region Dependencies

		private ICacheManager CacheManager { get; }
		private IDataRepository<Setting> SettingsRepo { get; }

		#endregion

		#region Constructor

		public SettingsService
		(
			ICacheManager cacheManager,
			IDataRepository<Setting> repository
		)
		{
			CacheManager = cacheManager;
			SettingsRepo = repository;
		}

		#endregion

		/// <summary>
		/// Get all settings from the repository
		/// </summary>
		/// <returns>A Dictionary of Setting objects</returns>
		private IDictionary<string, Setting> GetAllSettings()
		{
			return CacheManager.Get<IDictionary<string, Setting>>(CacheKey, 10, () =>
			{
				return SettingsRepo.Table.ToDictionary(s => s.Key.ToLower());
			});
		}

		/// <summary>
		/// Get a setting by it key
		/// </summary>
		/// <param name="key">The key to match</param>
		/// <returns>A Setting object or null</returns>
		private Setting GetSettingByKey(string key)
		{
			if (IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

			var k = key.ToLower();
			var settings = GetAllSettings();

			return settings.ContainsKey(k) ? settings[k] : null;
		}

		/// <summary>
		/// Checks if a setting exists
		/// </summary>
		/// <param name="key">The key to match</param>
		/// <returns>A Setting object or null</returns>
		private bool SettingExists(string key) => GetSettingByKey(key) != null;

		/// <summary>
		/// Inserts a new setting in the database
		/// </summary>
		/// <param name="setting">A Setting object to insert</param>
		private void InsertSetting(Setting setting)
		{
			if (setting == null) throw new ArgumentNullException(nameof(setting));

			//Insert into database
			SettingsRepo.Insert(setting);

			//Clear all settings from cache
			CacheManager.RemoveByPattern(CachePattern);
		}

		/// <summary>
		/// Updates an existing setting
		/// </summary>
		/// <param name="setting"></param>
		private void UpdateSetting(Setting setting)
		{
			if (setting == null) throw new ArgumentNullException(nameof(setting));

			if (!SettingsRepo.Table.Any(s => s.SettingId == setting.SettingId))
				throw new SettingNotFoundException();

			//Update the database
			SettingsRepo.Update(setting);

			//Clear all settings from cache
			CacheManager.RemoveByPattern(CachePattern);
		}

		/// <summary>
		/// Loads a setting class from the database
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extraIdentifier"></param>
		/// <returns></returns>
		public virtual T LoadSetting<T>(string extraIdentifier = "") where T : Core.Configuration.ISettings, new()
		{
			//Create the setting object
			var settings = Activator.CreateInstance<T>();

			//Build the settings object
			foreach (var prop in typeof(T).GetProperties())
			{
				if (!prop.CanRead || !prop.CanWrite) continue;

				//The key to the property
				var key = typeof(T).Name + "." + prop.Name;
				if (!IsNullOrEmpty(extraIdentifier)) key += "." + extraIdentifier;

				//Get the setting
				var setting = GetSettingByKey(key);

				if (Attribute.IsDefined(prop, typeof(SettingComplexType)))
				{
					if (setting?.Value == null)
					{
						var constr = prop.PropertyType.GetConstructor(Type.EmptyTypes);
						if (constr != null)
						{
							var instance = constr.Invoke(new object[0]);
							prop.SetValue(settings, instance, null);
						}
						else prop.SetValue(settings, null, null);
					}
					else
					{
						var value = Newtonsoft.Json.JsonConvert.DeserializeObject(setting.Value, prop.PropertyType);
						prop.SetValue(settings, value);
					}
				}
				else if (setting != null)
				{
					//Get the type converter
					var converter = TypeDescriptor.GetConverter(prop.PropertyType);

					//Check if the type can be converted from a string
					if (!converter.CanConvertFrom(typeof(String))) continue;

					//Check if the setting value can be converted into the target type
					if (!converter.IsValid(setting.Value)) continue;

					//Get the value of the property
					var value = converter.ConvertFromInvariantString(setting.Value);

					//Set the property
					prop.SetValue(settings, value);
				}
			}

			//Return the settings object
			return settings;

		}

		/// <summary>
		/// Save a setting to the database
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="settings"></param>
		/// <param name="extraIdentifier"></param>
		public virtual void SaveSetting<T>(T settings, string extraIdentifier = "") where T : Core.Configuration.ISettings, new()
		{
			foreach (var prop in typeof(T).GetProperties())
			{
				if (!prop.CanRead || !prop.CanWrite) continue;

				//The key to the property
				var key = typeof(T).Name + "." + prop.Name;
				if (!IsNullOrEmpty(extraIdentifier)) key += "." + extraIdentifier;

				//Get the value
				dynamic value = prop.GetValue(settings, null);

				//Set the setting
				if (Attribute.IsDefined(prop, typeof(SettingComplexType)))
				{
					if (value == null) SetSetting(key, "");
					else SetComplexSetting(key, value);
				}
				else
				{
					SetSetting(key, (value ?? ""));
				}
			}
		}

		/// <summary>
		/// Deletes all settings from the database
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public virtual void DeleteSetting<T>(string extraIdentifier = "") where T : Core.Configuration.ISettings, new()
		{
			var toDelete = new List<string>();
			foreach (var prop in typeof(T).GetProperties())
			{
				var key = typeof(T).Name + "." + prop.Name;
				if (!IsNullOrEmpty(extraIdentifier)) key += "." + extraIdentifier;
				toDelete.Add(key);
			}

			SettingsRepo.Table.Where(a => toDelete.Contains(a.Key)).Delete();

			// Clear all settings from cache
			CacheManager.RemoveByPattern(CachePattern);
		}

		#region Private Methods

		/// <summary>
		/// Insert ot update a setting
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private void SetSetting<T>(string key, T value)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			//Always store lowercase keys
			key = key.Trim().ToLowerInvariant();

			//Update an existing setting
			if (SettingExists(key))
			{
				var setting = GetSettingByKey(key);
				setting.Value = value.ToString();
				UpdateSetting(setting);
			}
			//Insert a new setting
			else
			{
				InsertSetting(new Setting
				{
					Key = key,
					Value = value.ToString()
				});
			}
		}

		private void SetComplexSetting<T>(string key, T value)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			//Always store lowercase keys
			key = key.Trim().ToLowerInvariant();

			// Serialize the object
			var saveValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);

			//Update an existing setting
			if (SettingExists(key))
			{
				var setting = GetSettingByKey(key);
				setting.Value = saveValue;
				UpdateSetting(setting);
			}
			//Insert a new setting
			else
			{
				InsertSetting(new Setting
				{
					Key = key,
					Value = saveValue
				});
			}
		}

		#endregion
	}
}
