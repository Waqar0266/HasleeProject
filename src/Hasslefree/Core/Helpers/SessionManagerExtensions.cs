using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Sessions;
using System;
using System.Linq;
using System.Text;

namespace Hasslefree.Core.Helpers
{
	public static class SessionManagerExtensions
	{
		public static String Hash(this ISessionManager session) => session == null ? Guid.NewGuid().ToString() : SessionHashFromPersonAndAccount(session.Login);

		public static String SessionHashFromPersonAndAccount(Login login)
		{
			var newCheckSum = $"{login.PersonId}:{login.Email}:{login.Person.PersonGuid}:{login.CreatedOn}";
			newCheckSum += $"-[{DateTime.UtcNow.RoundUp(TimeSpan.FromMinutes(2)).Ticks}]-";

			Byte[] hash;
			using (var md5 = System.Security.Cryptography.MD5.Create())
			{
				hash = md5.ComputeHash(Encoding.UTF8.GetBytes(newCheckSum));
			}

			return hash.Aggregate("", (current, byt) => current + byt.ToString("X"));
		}

	}
}
