﻿namespace Hasslefree.Web.Models.FileUploader
{
	public class UploadFilesResult
    {
        public string name { get; set; }
        public long size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
		public int downloadId { get; set; }
	}
}
