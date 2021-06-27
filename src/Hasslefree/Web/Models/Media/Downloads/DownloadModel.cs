using Newtonsoft.Json;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Web.Models.Common;
using System;

namespace Hasslefree.Web.Models.Media.Downloads
{
	/// <summary>
	/// A model for documents to be uploaded
	/// </summary>
	public class DownloadModel
	{
		public DownloadModel()
		{
			DownloadType = DownloadType.Other;
			MediaStorage = MediaStorage.Cloud;
			Action = CrudAction.None;
		}

		/// <summary>
		/// Unique identifier
		/// </summary>
		public Int32 DownloadId { get; set; }

		/// <summary>
		/// Type of file
		/// </summary>
		public String ContentType { get; set; }

		/// <summary>
		/// File extension
		/// </summary>
		public String Extension { get; set; }

		/// <summary>
		/// File name
		/// </summary>
		public String FileName { get; set; }

		public byte[] Data { get; set; }

		[JsonIgnore]
		public DownloadType DownloadType { get; set; }

		/// <summary>
		/// Enum of file type
		///  - Picture
		///  - Audio
		///  - Video
		///  - Document
		///  - Archive
		///  - Other
		/// </summary>
		public String DownloadTypeEnum
		{
			get => DownloadType.ToString();
			set => DownloadType = (DownloadType)Enum.Parse(typeof(DownloadType), value);
		}

		[JsonIgnore]
		public MediaStorage MediaStorage { get; set; }
		/// <summary>
		/// Enum of how value is stored
		///  - Database
		///  - Cloud
		///  - Disk
		/// </summary>
		public String MediaStorageEnum
		{
			get => MediaStorage.ToString();
			set => MediaStorage = (MediaStorage)Enum.Parse(typeof(MediaStorage), value);
		}

		/// <summary>
		/// File size
		/// </summary>
		public Int64 Size { get; set; }

		/// <summary>
		/// Location of file
		/// </summary>
		public String Key { get; set; }


		[JsonIgnore]
		public CrudAction Action { get; set; }
		/// <summary>
		/// Enum denoting action to take with file
		///  - None
		///  - Update
		///  - Create
		///  - Remove
		/// </summary>
		public String ActionEnum
		{
			get => Action.ToString();
			set => Action = (CrudAction)Enum.Parse(typeof(CrudAction), value);
		}
	}
}
