using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.HassleFree.Services.Infrastructure
{
    public interface ICloudStorageService
    {
        ICloudStorageService WithBucket(string bucket);

        ICloudStorageService UploadObject(StorageObject storageObject);
        ICloudStorageService UploadObjects(IEnumerable<StorageObject> storageObjects);

        ICloudStorageService RemoveObject(string key);
        ICloudStorageService RemoveObjects(IEnumerable<string> keys);
        ICloudStorageService RemoveFolder(string key);
        ICloudStorageService RemoveFolders(IEnumerable<string> keys);

        ICloudStorageService MoveObject(string[] moveKey);
        ICloudStorageService CopyObject(string fromKey, string toKey);

        Task Process();

        Byte[] ReadObject(string key);
    }
}
