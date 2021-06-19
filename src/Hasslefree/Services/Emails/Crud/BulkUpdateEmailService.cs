using Hasslefree.Core;
using Hasslefree.Core.Domain.Emails;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Emails.Crud
{
	public class BulkUpdateEmailService : IBulkUpdateEmailService, IInstancePerRequest
	{
		private IDataRepository<Email> EmailRepo { get; }
		private IDataContext Database { get; }

		private HashSet<Int32> _ids;
		private List<Email> _emails;
		private Dictionary<Int32, EmailSettings> _updates = new Dictionary<Int32, EmailSettings>();

		public BulkUpdateEmailService(
			IDataRepository<Email> emailRepo,
			IDataContext database
		)
		{
			EmailRepo = emailRepo;
			Database = database;
		}


		public IBulkUpdateEmailService WithEmailSetting(int id, string from, string subject, string recipient, bool send)
		{
			_updates.Add(id, new EmailSettings
			{
				From = from,
				Subject = subject,
				Recipient = recipient,
				Send = send
			});

			return this;
		}

		public bool Update(bool saveChanges = true)
		{
			try
			{
				_ids = _updates.Keys.ToHashSet();

				GetEmails();

				if (!_emails.Any()) return Clean(true);

				UpdateEmails();

				if (saveChanges)
					Database.SaveChanges();

				return Clean(true);
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}

			return Clean(false);
		}

		private bool Clean(bool success)
		{
			_ids.Clear();
			_emails.Clear();
			_updates.Clear();

			return success;
		}

		private void GetEmails()
		{
			_emails = EmailRepo.Table.Where(e => _ids.Contains(e.EmailId)).ToList();
		}

		private void UpdateEmails()
		{
			foreach (var email in _emails)
			{
				if (!_updates.ContainsKey(email.EmailId)) continue;

				var update = _updates[email.EmailId];
				var changes = false;

				if (!String.IsNullOrWhiteSpace(update.From) && (!email.From?.Equals(update.From, StringComparison.CurrentCultureIgnoreCase) ?? true))
				{
					email.From = update.From;
					changes = true;
				}

				if (!String.IsNullOrWhiteSpace(update.Subject) && (!email.Subject?.Equals(update.Subject, StringComparison.CurrentCultureIgnoreCase) ?? true))
				{
					email.Subject = update.Subject;
					changes = true;
				}

				if (!String.IsNullOrWhiteSpace(update.Recipient) && (!email.Recipient?.Equals(update.Recipient, StringComparison.CurrentCultureIgnoreCase) ?? true))
				{
					email.Recipient = update.Recipient;
					changes = true;
				}

				if (email.Send != update.Send)
				{
					email.Send = update.Send;
					changes = true;
				}

				if (changes) email.ModifiedOn = DateTime.Now;
			}
		}

		private class EmailSettings
		{
			public String From { get; set; }
			public String Subject { get; set; }
			public String Recipient { get; set; }
			public Boolean Send { get; set; }
		}
	}
}
