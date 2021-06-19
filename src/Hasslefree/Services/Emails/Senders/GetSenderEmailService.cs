using Hasslefree.Core.Domain.Emails;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.Linq;
using Hasslefree.Core;
using Hasslefree.Services.Cache;

namespace Hasslefree.Services.Emails.Senders
{
	public class GetSenderEmailService : IGetSenderEmailService, IInstancePerRequest
	{
		#region Private Properties

		private IReadOnlyRepository<Email> EmailRepo {get;}
		private ICacheManager Cache { get; }

		#endregion

		#region Constructor

		public GetSenderEmailService(
				IReadOnlyRepository<Email>	 emailRepo,
				ICacheManager cache
			)
		{
			EmailRepo = emailRepo;
			Cache = cache;
		}

		#endregion

		#region IGetSenderEmailService

		public Email this[String type]
		{
			get
			{
				return Cache.Get(CacheKeys.Store.Emails.SenderEmail(type), CacheKeys.Time.DefaultTime, () => EmailRepo.Table.FirstOrDefault(a => a.Type == type));
			}
		}

		#endregion
	}
}
