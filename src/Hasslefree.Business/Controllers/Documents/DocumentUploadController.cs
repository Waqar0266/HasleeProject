using Hasslefree.Core.Domain.Media;
using Hasslefree.Core.Infrastructure.Storage;
using Hasslefree.Data;
using Hasslefree.Services.Infrastructure.Storage;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Models.FileUploader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.Documents
{
	public class DocumentUploadController : BaseController
	{

		#region Fields

		private ICloudStorageService StorageService { get; }
		private IDataRepository<Download> DownloadRepo { get; }

		#endregion

		public DocumentUploadController(ICloudStorageService storageService, IDataRepository<Download> downloadRepo)
		{
			StorageService = storageService;

			DownloadRepo = downloadRepo;
		}

		[HttpPost]
		public ActionResult Index(HttpPostedFileBase[] files)
		{
			var result = new { files = new List<UploadFilesResult>() };

			StorageService.WithBucket("hasslefree-storage");

			foreach (HttpPostedFileBase file in files)
			{
				var uniqueId = Guid.NewGuid().ToString();
				var key = SlugifyUrl($"uploads/{uniqueId}/{file.FileName}");

				var fileBytes = GetByteArrayFromFile(file);
				//upload to S3
				StorageService.UploadObject(new StorageObject()
				{
					Data = fileBytes,
					FilePath = "",
					Key = key,
					MimeType = file.ContentType,
					Name = file.FileName,
					Size = fileBytes.Length
				});

				var download = new Download()
				{
					ContentType = file.ContentType,
					DownloadType = DownloadType.Document,
					Extension = Path.GetExtension(file.FileName).Replace(".", ""),
					FileName = file.FileName,
					MediaStorage = MediaStorage.Cloud,
					RelativeFolderPath = key,
					Size = file.ContentLength
				};

				DownloadRepo.Insert(download);

				result.files.Add(new UploadFilesResult()
				{
					deleteType = "GET",
					deleteUrl = $"/documents/delete/{download.DownloadId}",
					name = file.FileName,
					size = file.ContentLength,
					thumbnailUrl = GetThumbnail(file.FileName),
					type = file.ContentType,
					url = key,
					downloadId = download.DownloadId
				});
			}

			StorageService.Process();

			return Json(result);
		}

		[HttpGet]
		[Route("documents/delete/{id}")]
		public ActionResult Delete(int id)
		{
			var download = DownloadRepo.Table.FirstOrDefault(d => d.DownloadId == id);
			var key = download.RelativeFolderPath;
			StorageService.WithBucket("hasslefree-storage").RemoveObject(key).Process();
			DownloadRepo.Delete(download);

			return Json(new { }, JsonRequestBehavior.AllowGet);
		}

		#region Private Methods

		private byte[] GetByteArrayFromFile(HttpPostedFileBase file)
		{
			using (var target = new MemoryStream())
			{
				file.InputStream.CopyTo(target);
				return target.ToArray();
			}
		}

		private string SlugifyUrl(string url)
		{
			//First to lower case 
			url = url.ToLowerInvariant();

			//Remove all accents
			var bytes = Encoding.UTF8.GetBytes(url);

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

		private string GetThumbnail(string fileName)
		{
			var result = "unknown";
			var extension = Path.GetExtension(fileName).Replace(".", "");

			if (extension == "jpg" || extension == "jpeg")
				result = "jpg";
			if (extension == "doc")
				result = "doc";
			if (extension == "docx")
				result = "docx";
			if (extension == "mp3")
				result = "mp3";
			if (extension == "pdf")
				result = "pdf";
			if (extension == "ppt")
				result = "ppt";
			if (extension == "pptx")
				result = "pptx";
			if (extension == "txt")
				result = "txt";
			if (extension == "xls")
				result = "xls";
			if (extension == "xlsx")
				result = "xlsx";
			if (extension == "zip")
				result = "zip";

			return $"/Images/{result}.png";
		}

		#endregion
	}
}