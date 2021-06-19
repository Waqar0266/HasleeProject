using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hasslefree.Core
{
	/// <summary>
	/// Represents a common helper
	/// </summary>
	public class CommonHelper
	{
		/// <summary>
		/// Add spaces to separate the capitalized words in the string, 
		/// i.e. insert a space before each uppercase letter that is 
		/// either preceded by a lowercase letter or followed by a 
		/// lowercase letter (but not for the first char in string). 
		/// This keeps groups of uppercase letters - e.g. acronyms - together.
		/// </summary>
		/// <param name="pascalCaseString">A string in PascalCase</param>
		/// <returns></returns>
		public static string Wordify(String pascalCaseString)
		{
			Regex r = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z])");
			return r.Replace(pascalCaseString, " ${x}").Replace("_ ", "-");
		}

		/// <summary>
		/// Builds a key value pair list of an Enum type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<KeyValuePair<T, String>> GetEnumKeyValuePair<T>()
		{
			return new List<KeyValuePair<T, String>>(Enum.GetValues(typeof(T)).Cast<T>().Select(e => new KeyValuePair<T, String>(e, Wordify(e.ToString()))).ToList());
		}
	}
}
