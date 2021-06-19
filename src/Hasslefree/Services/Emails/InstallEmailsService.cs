using Hasslefree.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hasslefree.Core.Domain.Emails;
using Hasslefree.Data;
using Hasslefree.Services.Emails.Definitions;

namespace Hasslefree.Services.Emails
{
	public class InstallEmailsService : IInstallEmailsService, IInstancePerRequest
	{
		#region Constants

		private const String TemplateEmailsPrefix = "Hasslefree.Services.Emails.Templates";
		private const String EmailsPrefix = "Hasslefree.Services.Emails.Definitions";

		#endregion

		#region Private Properties

		private IDataRepository<Email> EmailRepo { get; }
		private IDataContext Database { get; }

		#endregion

		#region Constructor

		public InstallEmailsService(
				IDataRepository<Email> emailRepo,
				IDataContext database
			)
		{
			EmailRepo = emailRepo;
			Database = database;
		}

		#endregion

		#region IInstallEmailsService
		public void Install()
		{
			try
			{
				// Get all email types
				var types = GetEmailTypes();

				// Check that all email are in DB
				var dbTypes = GetDbEmails();
				var strTypes = types.Select(a => GetTypeName(a.FullName)).ToList();
				var missingTypes = types.Where(b => strTypes.Except(dbTypes).ToHashSet().Contains(GetTypeName(b.FullName))).ToHashSet();

				// Add emails that are not in DB
				InsertEmails(missingTypes);
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
			}
		}

		#endregion

		#region Private Methods

		private List<Type> GetEmailTypes()
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

			return types
						.Where(b => b != null && !String.IsNullOrWhiteSpace(b.FullName))
						.ToList();
		}

		private String GetTypeName(String fullName) => fullName?.Replace($"{EmailsPrefix}.", "");

		private List<String> GetDbEmails()
		{
			return EmailRepo.Table.Select(b => b.Type).ToList();
		}

		private void InsertEmails(HashSet<Type> types)
		{
			var installDefinitions = GetInstallDefinitions(types);

			var templates = GetTemplates(types);

			var save = false;
			foreach (var kv in installDefinitions)
			{
				var name = kv.Key;

				if (!templates.ContainsKey(name))
					continue;
					
				var definition = kv.Value;

				EmailRepo.Add(new Email()
				{
					CreatedOn = DateTime.Now,
					ModifiedOn = DateTime.Now,
					Type = name,
					Send = definition.Send,
					SendType = SendType.Default,
					From = definition.From,
					Subject = definition.Subject,
					Url = definition.Url,
					Template = templates[name]
				});
				save = true;
			}

			if (save)
				Database.SaveChanges();
		}

		private Dictionary<String, InstallDefinition> GetInstallDefinitions(HashSet<Type> types)
		{
			return (from type in types
					where type != null
					let instance = (IEmailDefinition)Activator.CreateInstance(type)
					select new
					{
						Name = GetTypeName(type.FullName),
						Definition = instance.Install
					}).ToDictionary(a => a.Name, b => b.Definition);
		}

		private Dictionary<String, String> GetTemplates(HashSet<Type> types)
		{
			return (from t in types
					where t != null
					let name = GetTypeName(t.FullName)
					select new
					{
						Name = name,
						Template = GetTemplate(name)
					}).ToDictionary(a => a.Name, b => b.Template);
		}

		private String GetTemplate(String type)
		{
			var location = $"{TemplateEmailsPrefix}.{type}.html";

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(location))
			{
				if (stream == null)
					return String.Empty;

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		#endregion


	}
}
