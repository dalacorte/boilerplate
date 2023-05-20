using Business.Domain.Interfaces.Repositories;
using Business.Domain.Interfaces.Services;
using Minio.DataModel;

namespace Business.Service.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public Task CopyFile(string fromBucket, string fromObj, string toBucket, string toObj, IServerSideEncryption? sseSrc = null, IServerSideEncryption? sseDest = null)
            => _fileRepository.CopyFile(fromBucket, fromObj, toBucket, toObj, sseSrc, sseDest);

        public Task CreateBucket(string bucket)
            => _fileRepository.CreateBucket(bucket);

        public Task DeleteBucket(string bucket)
            => _fileRepository.DeleteBucket(bucket);

        public Task<IEnumerable<Bucket>> GetAllBuckets()
            => _fileRepository.GetAllBuckets();

        public Task<BucketNotification> GetBucketNotifications(string bucket)
            => _fileRepository.GetBucketNotifications(bucket);

        public void GetFilesInBucketByPrefix(string bucket, string? prefix, bool? recursive = true, bool? versions = false)
            => _fileRepository.GetFilesInBucketByPrefix(bucket, prefix, recursive, versions);

        public void ListenBucketNotifications(string bucket, List<EventType> events, string? prefix = "", string? suffix = "", bool? recursive = true)
            => _fileRepository.ListenBucketNotifications(bucket, events, prefix, suffix, recursive);

        public void ListenIncompleteUploads(string bucket, string? prefix, bool? recursive = true)
            => _fileRepository.ListenIncompleteUploads(bucket, prefix, recursive);

        public Task RemoveAllBucketNotifications(string bucket)
            => _fileRepository.RemoveAllBucketNotifications(bucket);

        public Task RemoveIncompleteUpload(string bucket, string obj)
            => _fileRepository.RemoveIncompleteUpload(bucket, obj);

        public Task RemoveObject(string bucket, string obj, string? versionId = null)
            => _fileRepository.RemoveObject(bucket, obj, versionId);

        public Task RemoveObjects(string bucket, List<string> objs)
            => _fileRepository.RemoveObjects(bucket, objs);

        public Task UpdateFile(string bucket, string filePath, string obj, IServerSideEncryption? sse = null)
            => _fileRepository.UpdateFile(bucket, filePath, obj, sse);

        public Task<bool> VerifyIfBucketExists(string bucket)
            => _fileRepository.VerifyIfBucketExists(bucket);
    }
}
