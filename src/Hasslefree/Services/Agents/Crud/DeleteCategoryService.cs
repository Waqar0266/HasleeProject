using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Services.Infrastructure.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Configuration;
using Hasslefree.Core.Managers;
using static System.String;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class DeleteAgentService : IDeleteAgentService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Agent> AgentRepo { get; }
		private IDataRepository<Picture> PictureRepo { get; }

		// Services
		private ICloudStorageService CloudStorageService { get; }

		private IAppSettingsManager AppSettings { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private bool _removeImages;
		private bool _removeFiles;

		private readonly HashSet<int> _agentIds = new HashSet<int>();

		#endregion

		#region Constructor

		public DeleteAgentService
		(
			IDataRepository<Agent> agentRepo,
			IDataRepository<Picture> pictureRepo,
			ICloudStorageService cloudStorageService,
			IAppSettingsManager appSettings,
			IDataContext database
		)
		{
			// Repos
			AgentRepo = agentRepo;
			PictureRepo = pictureRepo;

			// Services
			CloudStorageService = cloudStorageService;

			AppSettings = appSettings;

			// Other
			Database = database;
		}

		#endregion

		#region IDeleteAgentService

		public bool HasWarnings
		{
			get
			{
				Warnings.Clear();
				return !IsValid();
			}
		}

		public List<AgentWarning> Warnings { get; } = new List<AgentWarning>();

		public IDeleteAgentService this[int agentId]
		{
			get
			{
				if (agentId <= 0) return this;

				if (_agentIds.Contains(agentId)) return this;

				_agentIds.Add(agentId);

				return this;
			}
		}

		public IDeleteAgentService this[List<int> agentIds]
		{
			get
			{
				if (!agentIds?.Any() ?? true) return this;

				foreach (var agentId in agentIds)
				{
					if (_agentIds.Contains(agentId)) continue;

					_agentIds.Add(agentId);
				}

				return this;
			}
		}

		public IDeleteAgentService RemoveImages(bool removeFiles = false)
		{
			_removeImages = true;
			_removeFiles = removeFiles;

			return this;
		}

		public bool Remove(bool saveChanges = true)
		{
			var categories = GetCategories();

			if (HasWarnings) return Clear(false);

			if (!categories.Any()) return Clear(true);

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				// Remove the categories
				AgentRepo.Remove(categories);

				// Remove the SEO records
				if (!RemoveSeoRecords(categories.Select(c => c.SeoId).Distinct().ToList())) return Clear(false);

				// Remove the Sitemap records
				if (!RemoveSitemapRecords(categories.Select(c => c.AgentId).Distinct().ToList())) return Clear(false);

				// Remove the pictures
				if (_removeImages) RemoveAgentPictures(categories.Where(c => c.PictureId.HasValue).Select(c => c.PictureId.Value).ToList());

				// Return if the changes mustn't be saved
				if (!saveChanges) return Clear(true);

				// Save the changes to the database
				Database.SaveChanges();

				scope.Complete();
			}

			return Clear(true);
		}

		#endregion

		#region Private Methods

		private List<Agent> GetCategories()
		{
			return (from c in AgentRepo.Table
					where _agentIds.Contains(c.AgentId)
					from sc in AgentRepo.Table
					where sc.Path.Equals(c.Path)
						  || sc.Path.StartsWith(c.Path + "/")
						  || sc.ParentAgentId.HasValue
						  && _agentIds.Contains(sc.ParentAgentId.Value)
					select sc).ToList();
		}

		private bool IsValid()
		{
			if (!_agentIds.Any())
				Warnings.Add(new AgentWarning(AgentWarningCode.CategoriesNotFound));

			return !Warnings.Any();
		}

		private void RemoveAgentPictures(ICollection<int> pictureIds)
		{
			var (pictures, paths) = GetAgentPictures(pictureIds);

			if (!pictures.Any()) return;

			PictureRepo.Remove(pictures);

			if (!_removeFiles || !(paths?.Any() ?? false)) return;

			foreach (var path in paths)
			{
				var pictureKey = path.Replace(AppSettings.CdnRoot, "").TrimEnd('/');

				if (IsNullOrWhiteSpace(pictureKey)) continue;

				// Remove the picture from cloud storage
				CloudStorageService
					.WithBucket(WebConfigurationManager.AppSettings["BucketName"])
					.RemoveObject(pictureKey)
					.Process();
			}
		}

		private (List<Picture> pictures, List<string> paths) GetAgentPictures(ICollection<int> pictureIds)
		{
			var pictures = PictureRepo.Table.Where(p => pictureIds.Contains(p.PictureId)).ToList();

			return !_removeFiles ? (pictures, null) : (pictures, pictures.Select(p => p.Path).ToList());
		}

		private bool RemoveSeoRecords(List<int> seoIds)
		{
			if (!seoIds?.Any() ?? true) return true;

			return false;
		}

		private bool RemoveSitemapRecords(List<int> agentIds)
		{
			return false;
		}

		private bool Clear(bool success)
		{
			_removeFiles = _removeImages = false;
			_agentIds.Clear();

			return success;
		}

		#endregion
	}
}