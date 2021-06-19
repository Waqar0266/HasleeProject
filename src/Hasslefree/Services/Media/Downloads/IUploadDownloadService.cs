using System;
using System.Collections.Generic;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Web.Models.Media.Downloads;

namespace Hasslefree.Services.Media.Downloads
{
	public interface IUploadDownloadService
	{
		IUploadDownloadService WithPath(String path);
		IUploadDownloadService Add(DownloadModel download);
		IUploadDownloadService Add(IEnumerable<DownloadModel> downloads);

		List<Download> Return();
		List<Download> Save();
	}
}
