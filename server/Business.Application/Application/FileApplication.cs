using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Services;
using Minio.DataModel;

namespace Business.Application.Application
{
    public class FileApplication : IFileApplication
    {
        private readonly IFileService _fileService;

        public FileApplication(IFileService fileService)
        {
            _fileService = fileService;
        }

        public Task CopyFile(string fromBucket, string fromObj, string toBucket, string toObj, IServerSideEncryption? sseSrc = null, IServerSideEncryption? sseDest = null)
            => _fileService.CopyFile(fromBucket, fromObj, toBucket, toObj, sseSrc, sseDest);

        public Task CreateBucket(string bucket)
            => _fileService.CreateBucket(bucket);

        public Task DeleteBucket(string bucket)
            => _fileService.DeleteBucket(bucket);

        public Task<IEnumerable<Bucket>> GetAllBuckets()
            => _fileService.GetAllBuckets();

        public Task<BucketNotification> GetBucketNotifications(string bucket)
            => _fileService.GetBucketNotifications(bucket);

        public void GetFilesInBucketByPrefix(string bucket, string? prefix, bool? recursive = true, bool? versions = false)
            => _fileService.GetFilesInBucketByPrefix(bucket, prefix, recursive, versions);

        public void ListenBucketNotifications(string bucket, List<EventType> events, string? prefix = "", string? suffix = "", bool? recursive = true)
            => _fileService.ListenBucketNotifications(bucket, events, prefix, suffix, recursive);

        public void ListenIncompleteUploads(string bucket, string? prefix, bool? recursive = true)
            => _fileService.ListenIncompleteUploads(bucket, prefix, recursive);

        public Task RemoveAllBucketNotifications(string bucket)
            => _fileService.RemoveAllBucketNotifications(bucket);

        public Task RemoveIncompleteUpload(string bucket, string obj)
            => _fileService.RemoveIncompleteUpload(bucket, obj);

        public Task RemoveObject(string bucket, string obj, string? versionId = null)
            => _fileService.RemoveObject(bucket, obj, versionId);

        public Task RemoveObjects(string bucket, List<string> objs)
            => _fileService.RemoveObjects(bucket, objs);

        public Task UpdateFile(string bucket, string filePath, string obj, IServerSideEncryption? sse = null)
            => _fileService.UpdateFile(bucket, filePath, obj, sse);

        public Task<bool> VerifyIfBucketExists(string bucket)
            => _fileService.VerifyIfBucketExists(bucket);
    }
}
