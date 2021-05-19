using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.HassleFree.Models.Documents;
using Nop.Plugin.Widgets.HassleFree.Services.Infrastructure;
using Nop.Services.Customers;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.HassleFree.Controllers.Documents
{
    public class DocumentUploadController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private ICloudStorageService _storageService { get; }
        private ICustomerService _customerService { get; }

        #endregion

        #region Ctor

        public DocumentUploadController(IWorkContext workContext,
                                        ICloudStorageService storageService,
                                        ICustomerService customerService)
        {
            _workContext = workContext;
            _storageService = storageService;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(customer))
                return Challenge();

            var files = Request.Form.Files;

            var result = new { files = new List<UploadFilesResult>() };

            _storageService.WithBucket("hasslefree-storage");

            foreach (var file in files)
            {
                var uniqueId = Guid.NewGuid().ToString();
                var key = SlugifyUrl($"uploads/{uniqueId}/{file.FileName}");

                var fileBytes = await GetByteArrayFromFileAsync(file);
                //upload to S3
                _storageService.UploadObject(new StorageObject()
                {
                    Data = fileBytes,
                    FilePath = "",
                    Key = key,
                    MimeType = file.ContentType,
                    Name = file.FileName,
                    Size = fileBytes.Length
                });

                result.files.Add(new UploadFilesResult()
                {
                    deleteType = "GET",
                    deleteUrl = "/documents/delete/id",
                    name = file.FileName,
                    size = file.Length,
                    thumbnailUrl = GetThumbnail(file.FileName),
                    type = file.ContentType,
                    url = key
                });
            }

            await _storageService.Process();

            return Json(result);
        }

        #endregion

        #region Private Methods

        private async Task<byte[]> GetByteArrayFromFileAsync(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                await file.CopyToAsync(target);
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
            var extension = Path.GetExtension(fileName);

            if (extension is "jpg" or "jpeg")
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

            return $"/Plugins/Widgets.HassleFree/Images/{result}.png";
        }

        #endregion
    }
}
