using System;
using System.Collections.Generic;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class SettingsExtensions
	{

		/// <summary>
		/// Updates a settings class with a dictionary
		/// Returns whether any values have been updated
		/// </summary>
		/// <typeparam name="T">Type of ISettings</typeparam>
		/// <param name="settings"></param>
		/// <returns>Boolean whether the value has changed</returns>
		public static Boolean UpdateFromDictionary<T>(this T settings, Dictionary<String, String> dict) where T : Core.Configuration.ISettings
		{
			Type type = typeof(T);
			Boolean changed = false;
			
			// Go through all items in the dictionary
			foreach(var kv in dict)
			{
				// Get the property based on the key value in dictionary
				var property = type.GetProperty(kv.Key);
				// continue if the property isn't found
				if(property == null)
					continue;
				// compare the current value to the new value
				if((string)property.GetValue(settings) != kv.Value)
				{
					// update the value, and set changed to true
					property.SetValue(settings, kv.Value);
					changed = true;
				}
			}
			return changed;
		}
	}
}
