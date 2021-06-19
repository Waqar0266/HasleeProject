using System;
using System.Text;
using System.Security.Cryptography;

namespace Hasslefree.Core.Crypto
{
	public static class Hash
	{
		/// <summary>
		/// Generates a random 32 byte string to be used as a salt when hashing passwords
		/// </summary>
		/// <returns></returns>
		public static String GetSalt(int length = 32) => BaseX.GenerateString(length);

		/// <summary>
		/// Generates a hashed value from the input and salt values
		/// </summary>
		/// <param name="input"></param>
		/// <param name="salt"></param>
		/// <returns></returns>
		public static String GetHash(String input, String salt = "")
		{
			using (var sha = SHA256.Create())
			{
				var bytes = Encoding.ASCII.GetBytes(String.Concat(salt, input));
				var hash = sha.ComputeHash(bytes);
				return Encoding.ASCII.GetString(hash);
			}
		}

		/// <summary>
		/// Generates a hashed value from the input and salt values and returns it as a base64 encoded string
		/// </summary>
		/// <param name="input"></param>
		/// <param name="salt"></param>
		/// <returns></returns>
		public static string GetHashBase64(String input, String salt = "")
		{
			using (var sha = SHA256.Create())
			{
				var bytes = Encoding.ASCII.GetBytes(String.Concat(salt, input));
				var hash = sha.ComputeHash(bytes);
				return Convert.ToBase64String(hash);
			}
		}
	}
}
