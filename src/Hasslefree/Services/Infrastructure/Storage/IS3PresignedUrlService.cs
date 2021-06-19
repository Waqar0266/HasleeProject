using System.Collections.Generic;

namespace Hasslefree.Services.Infrastructure.Storage
{
	public interface IS3PresignedUrlService
	{
		IS3PresignedUrlService WithBucket(string bucket);
		IS3PresignedUrlService WithStoreRootPath(string path);
		IS3PresignedUrlService WithMediaFolderPath(string path);
		IDictionary<string, string> GenerateKeys(IEnumerable<string> keys);
	}
}
