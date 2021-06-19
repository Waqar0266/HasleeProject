using Hasslefree.Core.Domain.Accounts;
using Hasslefree.Core.Domain.Security;
using Hasslefree.Data;
using Hasslefree.Web.Models.Security.Sessions.Get;
using System;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Security.Sessions
{
	public class GetSessionService : IGetSessionService
	{
		#region Private Properties

		private IDataRepository<Core.Domain.Security.Login> LoginRepo { get; }
		private IDataRepository<Person> PersonRepo { get; }
		private IDataRepository<Session> SessionRepo { get; }

		#endregion

		#region Constructor

		public GetSessionService
		(
			IDataRepository<Core.Domain.Security.Login> loginRepo,
			IDataRepository<Person> personRepo,
			IDataRepository<Session> sessionRepo
		)
		{
			// Repos
			LoginRepo = loginRepo;
			PersonRepo = personRepo;
			SessionRepo = sessionRepo;
		}

		#endregion

		#region IGetSessionsService

		public SessionWarning Warning { get; private set; }

		public SessionGet this[int sessionId, bool includeDates = true]
		{
			get
			{
				if (sessionId <= 0) return SessionNotFound();

				var session = SessionQuery(sessionId);

				if (session == null) return SessionNotFound();

				return new SessionGet
				{
					SessionId = session.SessionId,
					CreatedOn = includeDates ? session.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? session.ModifiedOn : (DateTime?)null,
					Reference = session.Reference,
					ExpiresOn = session.ExpiresOn,
					Location = new SessionLocation
					{
						IpAddress = session.IpAddress,
						Latitude = session.Latitude,
						Longitude = session.Longitude
					},
					User = GetSessionUser(session.LoginId, session.Login)
				};
			}
		}

		#endregion

		#region Private Methods

		private Session SessionQuery(int sessionId)
		{
			var sFuture = SessionRepo.Table.DeferredFirstOrDefault(s => s.SessionId == sessionId).FutureValue();

			var lFuture = (from s in SessionRepo.Table
						   where s.SessionId == sessionId && s.LoginId.HasValue
						   join l in LoginRepo.Table on s.LoginId.Value equals l.LoginId
						   select l).DeferredFirstOrDefault().FutureValue();

			var pFuture = (from s in SessionRepo.Table
						   where s.SessionId == sessionId && s.LoginId.HasValue
						   join l in LoginRepo.Table on s.LoginId.Value equals l.LoginId
						   join p in PersonRepo.Table on l.PersonId equals p.PersonId
						   select p).DeferredFirstOrDefault().FutureValue();

			var session = sFuture.Value;

			if (session == null) return null;

			session.Login = session.Login ?? lFuture.Value;
			if (session.Login != null) session.Login.Person = session.Login.Person ?? pFuture.Value;

			return session;
		}

		private dynamic SessionNotFound()
		{
			Warning = new SessionWarning(SessionWarning.SessionWarningCode.SessionNotFound);
			return null;
		}

		private static SessionUser GetSessionUser(int? loginId, Core.Domain.Security.Login user)
		{
			if (!loginId.HasValue) return null;

			return new SessionUser
			{
				LoginId = loginId.Value,
				PersonId = user?.PersonId,
				FullName = user?.Person?.FirstName + " " + user?.Person?.Surname,
				Email = user?.Email
			};
		}

		#endregion
	}
}
