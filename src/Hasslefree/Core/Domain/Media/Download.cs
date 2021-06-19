using System;

namespace Hasslefree.Core.Domain.Media
{
	/// <summary>
	/// A downloadable item, either for purchase or supporting document/file
	/// </summary>
	[Serializable]
	public class Download : BaseEntity
	{
		public Download()
		{
			CreatedOn = DateTime.Now;
			ModifiedOn = DateTime.Now;
		}
		
		public int DownloadId { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ModifiedOn { get; set; }
		public string FileName { get; set; }
		public string Extension { get; set; }
		public byte[] Binary { get; set; }
		public string RelativeFolderPath { get; set; }
		public string ContentType { get; set; }
		public long Size { get; set; }
		public string DownloadTypeEnum { get; set; }
		
		public string MediaStorageEnum { get; set; }
		public MediaStorage MediaStorage
		{
			get => (MediaStorage)Enum.Parse(typeof(MediaStorage), MediaStorageEnum);
			set => MediaStorageEnum = value.ToString();
		}

		public DownloadType DownloadType
		{
			get => (DownloadType)Enum.Parse(typeof(DownloadType), DownloadTypeEnum);
			set => DownloadTypeEnum = value.ToString();
		}
	}
}