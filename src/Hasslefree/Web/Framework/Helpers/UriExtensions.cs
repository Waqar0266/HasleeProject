using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hasslefree.Web.Framework.Helpers
{
	public static class UriExtensions
	{
		public static string ReplaceQueryStringByKey(this Uri uri, string key, object value)
		{
			// this gets all the query string key value pairs as a collection
			var newQueryString = HttpUtility.ParseQueryString(uri.Query);

			//replaces or adds the key
			newQueryString.Set(key, Convert.ToString(value));

			// this gets the page path from root without QueryString
			string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

			return newQueryString.Count > 0
				? $"{pagePathWithoutQueryString}?{newQueryString}"
				: pagePathWithoutQueryString;
		}

		public static string RemoveQueryStringValueByKey(this Uri uri, string key, object value, List<string> availableBrands)
		{

			// this gets all the query string key value pairs as a collection
			var newQueryString = HttpUtility.ParseQueryString(uri.Query);

			//adds the key
			var values = newQueryString.GetValues(key)?[0].Split('_').ToList() ?? new List<string>();
			values.Remove(Convert.ToString(value));
			//if (availableBrands.Count == 0) values.Clear();
			//else
			foreach (var brand in availableBrands)
				values.Remove(brand);

			if (values.Count == 0) newQueryString.Remove(key);
			else newQueryString.Set(key, String.Join("_", values));

			// this gets the page path from root without QueryString
			string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

			return (newQueryString.Count > 0
				? $"{pagePathWithoutQueryString}?{newQueryString}"
				: pagePathWithoutQueryString);
		}
	}
}
