using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static System.String;

namespace Hasslefree.Core.Sessions
{
	public static class SessionExtensions
	{

		public static String GenerateChecksum(this ISessionManager session)
		{
			if (session == null)
				return Hash("No Session was found.");

			if (session.Session == null)
				return Hash("No session was found.");

			// Prepare list of values
			var values = new List<String>
			{
				$"{session.Session.SessionId}-{session.Session.Reference}-{session.Session.ExpiresOn}"
			};

			if (session.Login?.Person != null)
				values.Add($"{session.Login.PersonId}-{session.Login.ModifiedOn.Ticks}-{session.Login.Person.GenderEnum}-{session.Login.Person.PersonStatusEnum}");

			if (session.Location != null)
				values.Add($"{session.Location.IP}-{session.Location.Latitude}-{session.Location.Longtitude}");

			var checksumString = Join(":", values);
			return Hash(checksumString);
		}

		private static String Hash(String checkString)
		{
			// Generate hash for new checksum
			Byte[] hash;
			using (var md5 = MD5.Create())
			{
				hash = md5.ComputeHash(Encoding.UTF8.GetBytes(checkString));
			}

			// Compare the checksums
			return hash.Aggregate("", (current, byt) => current + byt.ToString("X"));
		}

	}
}
