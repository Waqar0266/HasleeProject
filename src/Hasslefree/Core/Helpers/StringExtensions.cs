using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using Hasslefree.Core.Crypto;

namespace Hasslefree.Core.Helpers
{
	public static class StringExtensions
	{
		/// <summary>
		/// Slugify a string by removing all accent characters, 
		/// removing leading and trailing spaces, and replacing spaces with hyphens.
		/// </summary>
		/// <param name="domain">The string to slugify</param>
		/// <param name="maxLength">The max length before it gets trimmed</param>
		/// <returns></returns>
		public static string Slugify(this string domain, int maxLength = int.MaxValue)
		{
			// Remove Accent
			var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(domain);
			domain = Encoding.ASCII.GetString(bytes);

			domain = domain.ToLower();

			// Strip invalid characters  
			domain = Regex.Replace(domain, @"[^a-zA-Z0-9\s-]", "");

			// Convert multiple spaces into singlke spaces
			domain = Regex.Replace(domain, @"\s+", " ").Trim();

			// Cut & Trim 
			domain = domain.Substring(0, domain.Length <= maxLength ? domain.Length : maxLength).Trim();
			if (domain.EndsWith("-")) domain = domain.Substring(0, domain.Length - 1);
			domain = Regex.Replace(domain, @"\s", "-"); // Hyphens   

			return domain;
		}

		public static bool EnumTryParse<TEnum>(this string value, out TEnum @enum) where TEnum : struct, Enum
			=> Enum.TryParse(value, false, out @enum);

		public static string AppendIfNotNull(this string str, string appendText, string seperator = ", ")
			=> str += !string.IsNullOrWhiteSpace(appendText) ? seperator + appendText : null;

		/// <summary>
		/// Converts a string to "Title Case".
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToTitleCase(this String str)
		{
			var culture = Thread.CurrentThread.CurrentCulture;
			var info = culture.TextInfo;
			return info.ToTitleCase(str);
		}

		public static String ToLength(this String str, Int32 length)
		{
			if (String.IsNullOrWhiteSpace(str))
				return str;

			return str.Length > length ? str.Substring(0, length) : str;
		}

		public static String ToUniqueFileName(this String str)
		{
			if (!str.Contains("."))
				return str;

			var fileName = str.Substring(0, str.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase));
			var extension = str.Substring(str.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase) + 1);
			var random = BaseX.GenerateString(4, "abcdefghijklmnopqrstuvwxyz1234567890");

			return $"{fileName}_{random}.{extension}";
		}

		public static String ToSnakeCase(this String str)
		{
			if (String.IsNullOrWhiteSpace(str))
				return str;

			var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
			var matches = pattern.Matches(str);
			return String.Join("_", from Match m in matches select m.Value).ToLower();
		}
	}
}
