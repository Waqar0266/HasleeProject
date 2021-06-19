using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Services.Cache;
using System;

namespace Hasslefree.Services.Downloads
{
	[Obsolete("Deprecated. Please do not use services from the 'Hasslefree' Project.")]
	public class DownloadService : IDownloadService
	{
		#region Cache

		private const string KEY_DOWNLOAD_BY_ID = "Hasslefree.Cache.Downloads.Download.ById({0})";

		#endregion

		#region Fields

		private ICacheManager _cacheManager;
		private IDataRepository<Download> _downloadRepository;

		#endregion

		#region Constructor

		public DownloadService
		(
			ICacheManager cacheManager,
			IDataRepository<Download> downloadRepository
		)
		{
			_cacheManager = cacheManager;
			_downloadRepository = downloadRepository;
		}

		#endregion

		#region Download
		
		/// <summary>
		/// Inserts a new download
		/// </summary>
		/// <param name="download">A Download object</param>
		public void Create(Download download)
		{
			// Create the download
			download.CreatedOn = DateTime.Now;
			download.ModifiedOn = DateTime.Now;
			_downloadRepository.Insert(download);
		}

		#endregion
	}
}
