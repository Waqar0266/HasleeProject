using System;
using Hasslefree.Core.Domain.Media;

namespace Hasslefree.Services.Downloads
{
	[Obsolete("Deprecated. Please do not use services from the 'Hasslefree' Project.")]
	public interface IDownloadService
	{
		void Create(Download download);
	}
}
