using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Configuration;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Infrastructure.Storage;
using Hasslefree.Data;
using Hasslefree.Services.Infrastructure.Storage;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hasslefree.Core.Helpers;
using Hasslefree.Core.Managers;
using Hasslefree.Web.Models.Media.Pictures.Crud;

namespace Hasslefree.Services.Media.Pictures
{
	public class UploadPictureService : IUploadPictureService
	{
		#region Constants

		#endregion

		#region Private Properties

		// Data Repositories
		private IDataRepository<Picture> PictureRepo { get; }

		// Services
		private ICloudStorageService StorageService { get; }

		// Other
		private IDataContext Database { get; }
		private IAppSettingsManager AppSettings { get; }

		#endregion

		#region Fields

		private String _path;
		private List<PictureModel> _pictures;

		private List<Picture> _returnPictures;

		#endregion

		#region Constructor

		public UploadPictureService(
				// Data Repositories
				IDataRepository<Picture> pictureRepo,
				// Services
				ICloudStorageService storageService,
				// Other
				IDataContext database,
				IAppSettingsManager appSettings
			)
		{
			// Data Repositories
			PictureRepo = pictureRepo;

		   // Services
		   StorageService = storageService;

			// Other
			Database = database;
			AppSettings = appSettings;
		}

		#endregion

		#region IUploadService

		public IUploadPictureService WithPath(String path)
		{
			// /products/{product.Sku}/{_mediaSettings.PictureFolderName}
			_path = path;

			if(_path.EndsWith("/"))
				_path = _path.Remove(_path.Length - 1);

			if(!_path.StartsWith("/"))
				_path = "/" + _path;
			
			return this;
		}

		public IUploadPictureService Add(PictureModel picture)
		{
			if(_pictures == null)
				_pictures = new List<PictureModel>();

			_pictures.Add(picture);

			return this;
		}

		public IUploadPictureService Add(IEnumerable<PictureModel> pictures)
		{
			if(_pictures == null)
				_pictures = new List<PictureModel>();

			_pictures.AddRange(pictures);

			return this;
		}

		public List<Picture> Return()
		{
			ProcessPictures();

			_path = String.Empty;
			_pictures = new List<PictureModel>();
			return _returnPictures;
		}

		public List<Picture> Save()
		{
			ProcessPictures();

			var picturePaths = _returnPictures.Select(p => p.Path).ToList();
			var pictures = PictureRepo.Table.Where(p => picturePaths.Contains(p.Path)).ToList();

			foreach (var returnPicture in _returnPictures)
			{
				var picture = pictures.FirstOrDefault(p => p.Path == returnPicture.Path);

				if (picture == null)
				{
					PictureRepo.Add(returnPicture);
					
					Database.SaveChanges();
				}

				var pictureId = PictureRepo.Table.Where(p => p.Path == returnPicture.Path).Select(p => p.PictureId).FirstOrDefault();

				returnPicture.PictureId = pictureId;
			}

			_path = String.Empty;
			_pictures = new List<PictureModel>();
			return _returnPictures;
		}

		#endregion

		#region Private Methods
		
		private void ProcessPictures()
		{
			_returnPictures = new List<Picture>();

			var bucketName = WebConfigurationManager.AppSettings["BucketName"];

			StorageService.WithBucket(bucketName);
			
			foreach(var pic in _pictures)
			{
				Byte[] data;
				if(pic.File != null)
					data = pic.File;
				else if(!String.IsNullOrWhiteSpace(pic.Key))
					using(var client = new WebClient())
					{
						var key = pic.Key;
						if(!key.StartsWith("https://") && !key.StartsWith("http://"))
							key = $"http:{key}";
						key = key.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
						var uri = new Uri(key);
						data = client.DownloadData(uri);
					}
				else
					continue;

				// Make sure path doesn't start with a '/'
				if(_path.StartsWith("/")) _path = _path.Substring(1);

				var pKey = SlugifyUrl($"{_path}/{pic.Name}");

				// Add object to be uploaded
				StorageService.UploadObject(new StorageObject()
				{
					Data = data,
					FilePath = "",
					Key = pKey,
					MimeType = pic.MimeType,
					Name = pic.Name,
					Size = data.LongLength
				});

				// Add picture record to return list
				_returnPictures.Add(new Picture
				{
					CreatedOn = DateTime.Now,
					ModifiedOn = DateTime.Now,
					Name = pic.Name,
					Format = pic.Format,
					MimeType = pic.MimeType,
					AltText = pic.AlternateText,
					Path = AppSettings.PrependCdnRoot(pKey),
					Folder = _path,
					Transforms = pic.Transforms
				});
			}

			// Process the storage
			StorageService.Process();
		}

		public string SlugifyUrl(string url)
		{
			//First to lower case 
			url = url.ToLowerInvariant();

			//Remove all accents
			var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(url);

			url = Encoding.ASCII.GetString(bytes);

			//Replace spaces 
			url = Regex.Replace(url, @"\s", "-", RegexOptions.Compiled);

			//Remove invalid chars 
			url = Regex.Replace(url, @"[^\w\p{Pd}\/\.]", "", RegexOptions.Compiled);

			//Trim dashes from end 
			url = url.Trim('-', '_');

			//Replace double occurences of - or \_ 
			url = Regex.Replace(url, @"([-_]){2,}", "-", RegexOptions.Compiled);

			return url;
		}

		#endregion
	}
}
