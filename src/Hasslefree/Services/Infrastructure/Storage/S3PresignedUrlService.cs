using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Hasslefree.Web.Models.Common;
using Hasslefree.Web.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.String;

namespace Hasslefree.Services.Infrastructure.Storage
{
	public class S3PresignedUrlService : IS3PresignedUrlService
	{
		#region Properties

		/// <summary>
		/// The client used to connect to S3
		/// </summary>
		private AmazonS3Client S3Client { get; }

		#endregion

		#region Constructor

		public S3PresignedUrlService(string awsAccessKey, string awsSecretKey)
		{
			S3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.EUWest1);
		}

		#endregion

		#region Fields

		private string _bucket;
		private string _storageRootPath;
		private string _mediaFolderPath;

		#endregion

		public IS3PresignedUrlService WithBucket(string bucket)
		{
			_bucket = bucket.ToLower();

			return this;
		}

		public IS3PresignedUrlService WithStoreRootPath(string path)
		{
			_storageRootPath = path;

			return this;
		}

		public IS3PresignedUrlService WithMediaFolderPath(string path)
		{
			_mediaFolderPath = path;

			return this;
		}

		public IDictionary<string, string> GenerateKeys(IEnumerable<string> keys)
		{
			if (IsNullOrWhiteSpace(_storageRootPath))
				throw new Exception("No store root path specified");

			if (IsNullOrWhiteSpace(_bucket))
				throw new Exception("No bucket specified");

			if (keys == null || !keys.Any())
				throw new Exception("No keys provided");

			return GenerateSignedKeys(keys);
		}

		#region Private Methods

		private IDictionary<string, string> GenerateSignedKeys(IEnumerable<string> keys)
		{
			var signedKeys = new Dictionary<string, string>();

			foreach (var key in keys)
			{
				var url = S3Client.GetPreSignedURL(new GetPreSignedUrlRequest
				{
					BucketName = _bucket,
					Key = $"{_storageRootPath}{_mediaFolderPath}/{key.ToKebabCase(true, "~./".ToArray())}",
					Expires = DateTime.UtcNow.AddDays(1),
					Verb = HttpVerb.PUT
				});

				signedKeys.Add(key, url);
			}

			return signedKeys;
		}

		#endregion
	}
}
