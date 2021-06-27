using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Infrastructure.Storage;
using Hasslefree.Core.Managers;
using Hasslefree.Data;
using Hasslefree.Services.Infrastructure.Storage;
using Hasslefree.Web.Models.Media.Downloads;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Configuration;

namespace Hasslefree.Services.Media.Downloads
{
	public class UploadDownloadService : IUploadDownloadService
	{
		#region Constants

		#endregion

		#region Private Properties

		// Data Repositories
		private IDataRepository<Download> DownloadRepo { get; }

		// Services
		private ICloudStorageService StorageService { get; }

		// Managers
		private IAppSettingsManager AppSettings { get; }

		// Other
		private IDataContext Database { get; }

		#endregion

		#region Fields

		private String _path;
		private List<DownloadModel> _downloads;

		private List<Download> _returnDownloads;

		#endregion

		#region Constructor

		public UploadDownloadService(
				// Data Repositories
				IDataRepository<Download> downloadRepo,
				// Services
				ICloudStorageService storageService,
				// Managers
				IAppSettingsManager appSettings,
				// Other
				IDataContext database
			)
		{
			// Data Repositories
			DownloadRepo = downloadRepo;

			// Services
			StorageService = storageService;

			// Managers
			AppSettings = appSettings;

			// Other
			Database = database;
		}

		#endregion

		#region IUploadService

		public IUploadDownloadService WithPath(String path)
		{
			_path = path;

			if (_path.EndsWith("/"))
				_path = _path.Remove(_path.Length - 1);

			if (_path.StartsWith("/"))
				_path = _path.Remove(0, 1);

			return this;
		}

		public IUploadDownloadService Add(DownloadModel download)
		{
			if (_downloads == null)
				_downloads = new List<DownloadModel>();

			_downloads.Add(download);

			return this;
		}

		public IUploadDownloadService Add(IEnumerable<DownloadModel> downloads)
		{
			if (_downloads == null)
				_downloads = new List<DownloadModel>();

			_downloads.AddRange(downloads);

			return this;
		}

		public List<Download> Return()
		{
			ProcessPictures();

			return _returnDownloads;
		}

		public List<Download> Save()
		{
			ProcessPictures();

			DownloadRepo.Add(_returnDownloads);

			Database.SaveChanges();

			return _returnDownloads;
		}

		#endregion

		#region Private Methods

		private void ProcessPictures()
		{
			_returnDownloads = new List<Download>();

			var bucketName = WebConfigurationManager.AppSettings["BucketName"];

			StorageService.WithBucket(bucketName);

			foreach (var file in _downloads)
			{
				Byte[] data = file.Data;
				if (data == null)
					using (var client = new WebClient())
					{
						var key = file.Key;
						if (!key.StartsWith("https://") && !key.StartsWith("http://"))
							key = $"http:{key}";
						key = key.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
						var uri = new Uri(key);
						data = client.DownloadData(uri);
					}

				var path = $"/{_path}/{file.FileName}";
				if (!path.EndsWith(file.Extension))
					path = $"{path}.{file.Extension}";
				// Add object to be uploaded
				StorageService.UploadObject(new StorageObject()
				{
					Data = data,
					FilePath = "",
					Key = path,
					MimeType = file.ContentType,
					Name = file.FileName,
					Size = data.LongLength
				});

				// Add picture record to return list
				_returnDownloads.Add(new Download
				{
					CreatedOn = DateTime.Now,
					ModifiedOn = DateTime.Now,
					ContentType = file.ContentType,
					DownloadType = DownloadType.Document,
					Extension = file.Extension,
					FileName = file.FileName,
					MediaStorage = file.MediaStorage,
					RelativeFolderPath = AppSettings.PrependCdnRoot(path),
					Size = file.Size
				});
			}

			// Process the storage
			StorageService.Process();
		}

		#endregion
	}
}
