using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Widgets.HassleFree.Services.Infrastructure
{
    public class S3StorageService : ICloudStorageService
    {
        #region Member Variables

        /// <summary>
        /// The client used to connect to S3
        /// </summary>
        private IAmazonS3 S3Client { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;

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

        public S3StorageService(IHttpContextAccessor httpContextAccessor, IAmazonS3 s3Client)
        {
            _httpContextAccessor = httpContextAccessor;
            S3Client = s3Client;
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

        public async Task Process()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_bucket))
                    throw new Exception("No bucket specified");

                await ProcessBucket();

                await ProcessMoves();

                await ProcessCopies();

                await ProcessRemoves();

                await ProcessFolderRemoves();

                await ProcessAdds();
            }
            catch (AmazonServiceException eee)
            {
                var tmp = eee;
            }
            catch (Amazon.Runtime.Internal.HttpErrorResponseException ee)
            {
                var tmp = ee;
            }
            catch (Exception ex)
            {
                var tmp = ex;
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
                using (var response = S3Client.GetObjectAsync(request).Result)
                using (var responseStream = response.ResponseStream)
                    return responseStream.GetBytes();
            }
            catch (AmazonS3Exception e)
            {
                // ReSharper disable once PossibleIntendedRethrow
                throw e;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion

        #region Private Methods

        private async Task ProcessBucket()
        {
            // Create the bucket if it does not exist
            if (!String.IsNullOrWhiteSpace(_bucket))
            {
                var bucket = await GetBucketByName(_bucket);
                if (bucket == null)
                    await CreateBucket(_bucket);
            }
        }

        private async Task ProcessMoves()
        {
            // Remove all keys if there are any
            if (_moveKeys != null && _moveKeys.Any())
            {
                foreach (var key in _moveKeys.Where(a => a.Length == 2))
                    await MoveObject(key[0], key[1]);
            }
        }

        private async Task ProcessCopies()
        {
            // Remove all keys if there are any
            if (_copyKeys != null && _copyKeys.Any())
            {
                foreach (var key in _copyKeys)
                    foreach (var destination in key.Value)
                        await ProcessCopy(key.Key, destination);
            }
        }

        private async Task ProcessRemoves()
        {
            // Remove all keys if there are any
            if (_removeFolderKeys != null && _removeKeys.Any())
            {
                foreach (var key in _removeKeys)
                    await RemoveKey(key);
            }
        }

        private async Task RemoveKey(string key)
        {
            await S3Client.DeleteObjectAsync(new DeleteObjectRequest { BucketName = _bucket, Key = key });
        }

        private async Task ProcessFolderRemoves()
        {
            // Remove all keys if there are any
            if (_removeFolderKeys != null && _removeFolderKeys.Any())
            {
                // Get all of the keys in the folder
                foreach (var key in _removeFolderKeys)
                {
                    var request = new ListObjectsRequest { BucketName = _bucket, Prefix = key };

                    do
                    {
                        // Get the first 1000 keys
                        var response = await S3Client.ListObjectsAsync(request);

                        // If there are more keys to load, flag the request
                        if (response.IsTruncated)
                            request.Marker = response.NextMarker;
                        else
                            request = null;

                        if (response.S3Objects.Any())
                        {
                            await S3Client.DeleteObjectsAsync(new DeleteObjectsRequest
                            {
                                BucketName = _bucket,
                                Objects = response.S3Objects.Select(a => a.Key).Select(a => new KeyVersion { Key = a })
                                    .ToList()
                            });
                        }
                    } while (request != null);
                }
            }
        }

        private async Task ProcessAdds()
        {
            // Remove all keys if there are any
            if (_addObjects != null && _addObjects.Any())
            {
                foreach (var obj in _addObjects)
                    await AddObject(obj);
            }
        }

        private async Task AddObject(StorageObject storageObject)
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

            putObjectRequest.Headers.CacheControl = "public, max-age=3240000";
            putObjectRequest.Headers.ExpiresUtc = storageObject.Expires;
            putObjectRequest.Headers.ContentLength = storageObject.Size;

            // Check whether there is a file location
            if (!String.IsNullOrWhiteSpace(storageObject.FilePath))
                putObjectRequest.FilePath = storageObject.FilePath;
            // If no file location specified, use byte[]
            else
                putObjectRequest.InputStream = new MemoryStream(storageObject.Data);

            // Add content disposition if download only
            if (storageObject.DownloadOnly)
                putObjectRequest.Headers["Content-Disposition"] = $"attachment; filename={storageObject.Name};";

            //Put the storage object
            var result = await S3Client.PutObjectAsync(putObjectRequest);
        }

        private async Task MoveObject(string sourceKey, string destinationkey)
        {
            // Trim any leading '/' characters
            sourceKey = sourceKey?.TrimStart('/');
            destinationkey = destinationkey?.TrimStart('/');

            // Get the information of the source object
            var sourceObject = await S3Client.GetObjectAsync(_bucket, sourceKey);

            // Copy the source image to the new destination
            await S3Client.CopyObjectAsync(new CopyObjectRequest
            {
                SourceKey = sourceObject.Key,
                DestinationKey = destinationkey,
                SourceBucket = sourceObject.BucketName,
                DestinationBucket = _bucket
            });

            // Remove the source file
            RemoveObject(sourceObject.Key);
        }

        private async Task ProcessCopy(string sourceKey, string destinationKey)
        {
            // Trim any leading '/' characters
            sourceKey = sourceKey?.TrimStart('/');
            destinationKey = destinationKey?.TrimStart('/');

            // Get the information of the source object
            //var sourceObject = S3Client.GetObject(_bucket, sourceKey);

            // Copy the source image to the new destination
            await S3Client.CopyObjectAsync(new CopyObjectRequest
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
        private async Task CreateBucket(string bucketName)
        {
            //get a list of all the buckets
            var bucketResponse = await S3Client.ListBucketsAsync();

            //go through all the buckets to check if it exists
            var found = bucketResponse.Buckets.Any(bucket => bucket.BucketName.ToLower().Equals(bucketName.ToLower()));

            //Create the bucket request with the bucket's details if it does not exist
            if (found)
                return;

            var bucketRequest = new PutBucketRequest()
            {
                BucketName = bucketName,
                BucketRegion = new S3Region("EU"),
                BucketRegionName = "EU"
            };
            //Create the bucket
            await S3Client.PutBucketAsync(bucketRequest);
        }

        /// <summary>
        /// Gets a bucket object with the specified name
        /// </summary>
        /// <param name="bucketName">The name of the bucket to find</param>
        /// <returns>Returns a bucket object or null</returns>
        private async Task<S3Bucket> GetBucketByName(string bucketName)
        {
            //Get all bucket objects
            var response = await S3Client.ListBucketsAsync();

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

    public static class StreamExtensions
    {
        /// <summary>
        /// Get the bytes from a stream (max 2GB)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Byte[] GetBytes(this Stream stream)
        {
            Int32 length = (Int32)stream.Length;
            Byte[] bytes = new Byte[length];
            stream.Read(bytes, 0, length);
            return bytes;
        }
    }
}
