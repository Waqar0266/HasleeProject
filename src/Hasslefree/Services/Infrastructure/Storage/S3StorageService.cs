using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Infrastructure.Storage;

namespace Hasslefree.Services.Infrastructure.Storage
{
	public class S3StorageService : ICloudStorageService
	{
		#region Member Variables

		/// <summary>
		/// The client used to connect to S3
		/// </summary>
		private AmazonS3Client S3Client { get; }

		#endregion

		#region Fields

		private string _bucket;
		private List<string> _removeKeys;
		private List<string> _removeFolderKeys;
		private List<string[]> _moveKeys;
		private Dictionary<String, HashSet<String>> _copyKeys;
		private List<StorageObject> _addObjects;

		#endregion

		#region Constructor

		public S3StorageService(string awsAccessKey, string awsSecretKey)
		{
			//Create the Amazon S3 client
			S3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, new AmazonS3Config { RegionEndpoint = RegionEndpoint.EUWest1 });
		}

		#endregion

		#region ICloudStorageService

		public ICloudStorageService WithBucket(string bucket)
		{
			_bucket = bucket.ToLower();

			return this;
		}

		public ICloudStorageService UploadObject(StorageObject storageObject)
		{
			if (_addObjects == null)
				_addObjects = new List<StorageObject>();

			_addObjects.Add(storageObject);

			return this;
		}

		public ICloudStorageService UploadObjects(IEnumerable<StorageObject> storageObjects)
		{
			if (_addObjects == null)
				_addObjects = new List<StorageObject>();

			_addObjects.AddRange(storageObjects);

			return this;
		}

		/// <summary>
		/// Move a source key to a new destination key
		/// </summary>
		/// <param name="moveKey">[0] - Source Key, [1] - Destination Key</param>
		/// <returns></returns>
		public ICloudStorageService MoveObject(string[] moveKey)
		{
			if (_moveKeys == null)
				_moveKeys = new List<string[]>();

			_moveKeys.Add(moveKey);

			return this;
		}

		public ICloudStorageService CopyObject(string fromKey, string toKey)
		{
			if (_copyKeys == null)
				_copyKeys = new Dictionary<string, HashSet<String>>();

			if (!_copyKeys.ContainsKey(fromKey))
				_copyKeys.Add(fromKey, new HashSet<String>()
				{
					toKey
				});
			else
				_copyKeys[fromKey].Add(toKey);

			return this;
		}

		public ICloudStorageService RemoveObject(string key)
		{
			if (_removeKeys == null)
				_removeKeys = new List<string>();

			_removeKeys.Add(key);

			return this;
		}

		public ICloudStorageService RemoveObjects(IEnumerable<string> keys)
		{
			if (_removeKeys == null)
				_removeKeys = new List<string>();

			_removeKeys.AddRange(keys);

			return this;
		}

		public ICloudStorageService RemoveFolder(string key)
		{
			if (_removeFolderKeys == null)
				_removeFolderKeys = new List<string>();

			_removeFolderKeys.Add(key);

			return this;
		}

		public ICloudStorageService RemoveFolders(IEnumerable<string> keys)
		{
			if (_removeFolderKeys == null)
				_removeFolderKeys = new List<string>();

			_removeFolderKeys.AddRange(keys);

			return this;
		}

		public void Process()
		{
			try
			{
				if (String.IsNullOrWhiteSpace(_bucket))
					throw new Exception("No bucket specified");

				ProcessBucket();

				ProcessMoves();

				ProcessCopies();

				ProcessRemoves();

				ProcessFolderRemoves();

				ProcessAdds();
			}
			catch (Exception ex)
			{
				Core.Logging.Logger.LogError(ex);
				throw;
			}
		}

		/// <summary>
		/// Read an object from S3 and returns a StreamReader
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Byte[] ReadObject(string key)
		{
			try
			{
				var request = new GetObjectRequest
				{
					BucketName = _bucket,
					Key = key
				};
				using (var response = S3Client.GetObject(request))
					using (var responseStream = response.ResponseStream)
						return responseStream.GetBytes();
			}
			catch (AmazonS3Exception e)
			{
				Core.Logging.Logger.LogError(e);
				// ReSharper disable once PossibleIntendedRethrow
				throw e;
			}
			catch (Exception e)
			{
				Core.Logging.Logger.LogError(e);
				throw;
			}
		}

		#endregion

		#region Private Methods

		private void ProcessBucket()
		{
			// Create the bucket if it does not exist
			if (String.IsNullOrWhiteSpace(_bucket)) return;

			var bucket = GetBucketByName(_bucket);
			if (bucket == null) CreateBucket(_bucket);
		}

		private void ProcessMoves()
		{
			// Remove all keys if there are any
			if (!(_moveKeys?.Any() ?? false)) return;

			foreach (var key in _moveKeys.Where(a => a.Length == 2))
				MoveObject(key[0], key[1]);
		}

		private void ProcessCopies()
		{
			// Remove all keys if there are any
			if (!(_copyKeys?.Any() ?? false)) return;

			foreach (var key in _copyKeys)
				foreach(var destination in key.Value)
					ProcessCopy(key.Key, destination);
		}

		private void ProcessRemoves()
		{
			// Remove all keys if there are any
			if (!(_removeKeys?.Any() ?? false)) return;

			foreach (var key in _removeKeys)
				RemoveKey(key);
		}

		private void RemoveKey(string key)
			=> S3Client.DeleteObject(new DeleteObjectRequest
			{
				BucketName = _bucket,
				Key = key
			});

		private void ProcessFolderRemoves()
		{
			// Remove all keys if there are any
			if (!(_removeFolderKeys?.Any() ?? false))
				return;

			// Get all of the keys in the folder
			foreach (var key in _removeFolderKeys)
			{
				var request = new ListObjectsRequest
				{
					BucketName = _bucket,
					Prefix = key
				};

				do
				{
					// Get the first 1000 keys
					var response = S3Client.ListObjects(request);

					// If there are more keys to load, flag the request
					if (response.IsTruncated)
						request.Marker = response.NextMarker;
					else
						request = null;

					if (response.S3Objects.Any())
						S3Client.DeleteObjects(new DeleteObjectsRequest
						{
							BucketName = _bucket,
							Objects = response.S3Objects.Select(a => a.Key).Select(a => new KeyVersion
							{
								Key = a
							}).ToList()
						});
				} while (request != null);
			}
		}

		private void ProcessAdds()
		{
			// Remove all keys if there are any
			if (!(_addObjects?.Any() ?? false)) return;

			foreach (var obj in _addObjects)
				AddObject(obj);
		}

		private void AddObject(StorageObject storageObject)
		{
			storageObject.Key = storageObject.Key.TrimStart('/');

			var putObjectRequest = new PutObjectRequest
			{
				AutoCloseStream = true,
				AutoResetStreamPosition = true,
				ContentType = storageObject.MimeType,
				Key = storageObject.Key,
				BucketName = _bucket,
				CannedACL = S3CannedACL.PublicRead
			};

			// Check whether there is a file location
			if (!String.IsNullOrWhiteSpace(storageObject.FilePath))
				putObjectRequest.FilePath = storageObject.FilePath;
			// If no file location specified, use byte[]
			else
				putObjectRequest.InputStream = new MemoryStream(storageObject.Data);

			// Add content disposition if download only
			if (storageObject.DownloadOnly) putObjectRequest.Headers["Content-Disposition"] = $"attachment; filename={storageObject.Name};";

			//Put the storage object
			S3Client.PutObject(putObjectRequest);
		}

		private void MoveObject(string sourceKey, string destinationkey)
		{
			// Trim any leading '/' characters
			sourceKey = sourceKey?.TrimStart('/');
			destinationkey = destinationkey?.TrimStart('/');

			// Get the information of the source object
			var sourceObject = S3Client.GetObject(_bucket, sourceKey);

			// Copy the source image to the new destination
			S3Client.CopyObject(new CopyObjectRequest
			{
				SourceKey = sourceObject.Key,
				DestinationKey = destinationkey,
				SourceBucket = sourceObject.BucketName,
				DestinationBucket = _bucket
			});

			// Remove the source file
			RemoveObject(sourceObject.Key);
		}

		private void ProcessCopy(string sourceKey, string destinationKey)
		{
			// Trim any leading '/' characters
			sourceKey = sourceKey?.TrimStart('/');
			destinationKey = destinationKey?.TrimStart('/');

			// Get the information of the source object
			//var sourceObject = S3Client.GetObject(_bucket, sourceKey);

			// Copy the source image to the new destination
			S3Client.CopyObject(new CopyObjectRequest
			{
				SourceKey = sourceKey,
				DestinationKey = destinationKey,
				SourceBucket = _bucket,
				DestinationBucket = _bucket
			});
		}

		/// <summary>
		/// Creates a new Bucket
		/// </summary>
		/// <param name="bucketName">The name of the bucket to create</param>
		private void CreateBucket(string bucketName)
		{
			//get a list of all the buckets
			var bucketResponse = S3Client.ListBuckets();

			//go through all the buckets to check if it exists
			var found = bucketResponse.Buckets.Any(bucket => bucket.BucketName.ToLower().Equals(bucketName.ToLower()));

			//Create the bucket request with the bucket's details if it does not exist
			if (found) return;

			var bucketRequest = new PutBucketRequest()
			{
				BucketName = bucketName,
				BucketRegion = new S3Region("EU"),
				BucketRegionName = "EU"
			};
			//Create the bucket
			S3Client.PutBucket(bucketRequest);
		}

		/// <summary>
		/// Gets a bucket object with the specified name
		/// </summary>
		/// <param name="bucketName">The name of the bucket to find</param>
		/// <returns>Returns a bucket object or null</returns>
		private S3Bucket GetBucketByName(string bucketName)
		{
			//Get all bucket objects
			var response = S3Client.ListBuckets();

			//Check for null values
			if (response != null && response.Buckets != null)
			{
				//Loop through all returned buckets
				foreach (var s3Bucket in response.Buckets)
				{
					//Check if bucket name matches bucketName
					if (s3Bucket.BucketName == bucketName)
					{
						//Return the bucket object
						return s3Bucket;
					}
				}
			}

			//Return null if no matching bucket was found
			return null;
		}

		#endregion
	}
}
