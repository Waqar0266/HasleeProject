using Hasslefree.Core;
using Hasslefree.Core.Domain.Emails;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Web.Models.Emails;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hasslefree.Services.Emails.Crud
{
	public class ListEmailService : IListEmailService, IInstancePerRequest
	{
		#region Constants

		private const String EmailsPrefix = "Hasslefree.Services.Emails.Definitions";

		#endregion

		private readonly Dictionary<string, string> Titles = new Dictionary<string, string>()
		{
			{ "Customers.Register", "Registration" },
			{ "Customers.RegisterApproval", "Registration Approval" },
			{ "Customers.RegisterDeclined", "Registration Declined" },
			{ "Customers.RegisterApproved", "Registration Approved" },
			{ "Customers.RegisterOtp", "Registration Otp" },
			{ "Customers.ForgotPassword", "Registration Forgot Password" },
			{ "Customers.Invoice", "Invoice" },
			{ "Customers.EftReference", "EFT Reference" },
			{ "Customers.Enquiry", "Enquiry" },
			{ "Customers.Voucher", "Voucher" },
			{ "Customers.CollectionInfo", "Collection Info" },
			{ "Customers.CollectionReady", "Collection Ready" },
		};

		private IReadOnlyRepository<Email> EmailRepo { get; }

		private String _type;
		private Int32 _storeId;

		private IQueryable<Email> _emails;

		public ListEmailService(
		IReadOnlyRepository<Email> emailRepo
		)
		{
			EmailRepo = emailRepo;
		}

		public IListEmailService StartsWithType(string type)
		{
			_type = type;
			return this;
		}

		public List<EmailSettingsModel> List()
		{
			if (_type == null) return new List<EmailSettingsModel>();

			GetEmails();

			var emails = _emails.Any() ? _emails.Select(e => new EmailSettingsModel()
			{
				Id = e.EmailId,
				From = e.From,
				Subject = e.Subject,
				Recipient = e.Recipient,
				Send = e.Send,
				Type = e.Type
			}).ToList() : new List<EmailSettingsModel>();

			emails.ForEach(e =>
			{
				e.Title = GetTitle(e.Type);
				e.Type = e.Type.Replace(".", "-");
			});

			return emails;
		}

		private void GetEmails()
		{

			_emails = EmailRepo.Table.Where(a => a.Type.StartsWith(_type));
		}

		private string GetTitle(string type)
		{
			var iCastType = typeof(IEmailDefinition);

			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => p != null &&
							(p.FullName?.StartsWith(EmailsPrefix) ?? false) &&
							!p.IsInterface &&
							!p.IsAbstract &&
							iCastType.IsAssignableFrom(p))
				.Select(a => a).ToList();

			var fullType = types.FirstOrDefault(a => a.FullName == $"{EmailsPrefix}.{type}");

			if (fullType == null)
				return null;

			var instance = (IEmailDefinition)Activator.CreateInstance(fullType);

			return instance?.Title;
		}
	}
}
