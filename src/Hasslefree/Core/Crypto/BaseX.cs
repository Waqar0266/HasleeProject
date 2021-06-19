using System;
using Hasslefree.Core.Helpers.Extensions;

namespace Hasslefree.Core.Crypto
{
	public class BaseX
	{
		/* Properties */
		private string Chars { get; }

		/* CTOR */
		public BaseX()
		{
			Chars = DefaultChars;
		}

		public BaseX(string characters)
		{
			Chars = characters;
		}

		/* Static */
		private static readonly string DefaultChars = "0123456789abcdefghijklmnopqrstuvwxyz";
		private static Random Rand { get; } = new Random();

		/// <summary>
		/// Create a new instance of the BaseX class
		/// </summary>
		/// <param name="chars"></param>
		/// <returns></returns>
		private static BaseX New(string chars = null)
		{
			return chars == null ? new BaseX() : new BaseX(chars);
		}

		/// <summary>
		/// Get a random string
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		private string GetRandomString(int length)
		{
			var result = new char[length];
			var len = Chars.Length;
			for (var i = 0; i < length; i++)
			{
				var n = Rand.Next(len);
				result[i] = Chars[n];
			}
			return new string(result);
		}

		private string GetRandomStringSingleRandom(int length)
		{
			var charLength = Chars.Length;
			var len = (Int64)Math.Pow(charLength, length);
			var value = Rand.NextLong(len);

			var result = "";
			do
			{
				var res = (Int32)(value % charLength);
				result = $"{Chars[res]}{result}";

				value = value / charLength;
			} while (value >= charLength);

			result = $"{Chars[(Int32)value]}{result}";

			return result;
		}


		/// <summary>
		/// Generate a random string
		/// </summary>
		/// <param name="length"></param>
		/// <param name="characters"></param>
		/// <returns></returns>
		public static string GenerateString(int length = 8, string characters = null)
		{
			var baseX = New(characters);

			length = Math.Abs(length);

			return (Int64) Math.Pow(baseX.Chars.Length, length) < 0 ? baseX.GetRandomString(length) : baseX.GetRandomStringSingleRandom(length);
		}
	}
}