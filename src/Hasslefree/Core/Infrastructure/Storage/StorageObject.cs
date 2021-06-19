using System;

namespace Hasslefree.Core.Infrastructure.Storage
{
	public class StorageObject
	{
		public StorageObject()
		{
			CreatedOnUtc = DateTime.UtcNow;
			ModifiedOnUtc = DateTime.UtcNow;
			DownloadOnly = false;
		}

		#region Value Properties

		public string Key { get; set; } //relative path
		public string FilePath { get; set; }
		public string Name { get; set; }
		public byte[] Data { get; set; }
		public DateTime CreatedOnUtc { get; set; }
		public DateTime ModifiedOnUtc { get; set; }
		public long Size { get; set; }
		public string MimeType { get; set; }
		public bool DownloadOnly { get; set; }

		#endregion
	}
}
