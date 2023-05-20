using Business.Domain.Interfaces.Repositories;
using Business.Domain.Model;
using Business.Repository.Contexts;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;

namespace Business.Repository.Repositories
{
    public class FileRepository : IFileRepository
    {
        public IDisposable SubscriptionNotifications { get; private set; }
        public IDisposable SubscriptionIncompleteUpoads { get; private set; }
        public IDisposable SubscriptionFilesInBucket { get; private set; }
        public IDisposable SubscriptionObjects { get; private set; }
        public List<string> Notifications { get; private set; } = new List<string>();
        public List<string> IncompleteUploads { get; private set; } = new List<string>();
        public List<string> FilesInBucket { get; private set; } = new List<string>();
        public List<string> Objects { get; private set; } = new List<string>();

        private readonly MinioContext _context;

        public FileRepository(IOptions<MinioConnection> settings)
        {
            _context = new MinioContext(settings);
        }

        public async Task<bool> VerifyIfBucketExists(string bucket)
        {
            BucketExistsArgs args = new BucketExistsArgs()
                .WithBucket(bucket);
            bool found = await _context.Minio.BucketExistsAsync(args);
            if (found)
                return found;
            else
                throw new FileNotFoundException();
        }

        public async Task UpdateFile(string bucket, string filePath, string obj, IServerSideEncryption? sse = null)
        {
            byte[] bs = File.ReadAllBytes(filePath);
            using (MemoryStream filestream = new MemoryStream(bs))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                Dictionary<string, string> metaData = new Dictionary<string, string> { { "Metadata", $"{bucket}_{obj}" } };
                PutObjectArgs args = new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(obj)
                    .WithStreamData(filestream)
                    .WithObjectSize(filestream.Length)
                    .WithContentType("application/octet-stream")
                    .WithHeaders(metaData)
                    .WithServerSideEncryption(sse);
                await _context.Minio.PutObjectAsync(args);
            }
        }

        public async Task CopyFile(string fromBucket, string fromObj, string toBucket, string toObj, IServerSideEncryption? sseSrc = null, IServerSideEncryption? sseDest = null)
        {
            CopySourceObjectArgs cpSrcArgs = new CopySourceObjectArgs()
                .WithBucket(fromBucket)
                .WithObject(fromObj)
                .WithServerSideEncryption(sseSrc);
            CopyObjectArgs args = new CopyObjectArgs()
                .WithBucket(toBucket)
                .WithObject(toObj)
                .WithCopyObjectSource(cpSrcArgs)
                .WithServerSideEncryption(sseDest);
            await _context.Minio.CopyObjectAsync(args);
        }

        public async Task CreateBucket(string bucket)
        {
            await _context.Minio.MakeBucketAsync(
                new MakeBucketArgs()
                    .WithBucket(bucket));
        }

        public async Task DeleteBucket(string bucket)
        {
            await _context.Minio.RemoveBucketAsync(
                new RemoveBucketArgs()
                    .WithBucket(bucket));
        }

        public void GetFilesInBucketByPrefix(string bucket, string? prefix, bool? recursive = true, bool? versions = false)
        {
            ListObjectsArgs listArgs = new ListObjectsArgs()
                .WithBucket(bucket)
                .WithPrefix(prefix)
                .WithRecursive((bool)recursive);

            IObservable<Item> observable = _context.Minio.ListObjectsAsync(listArgs);
            SubscriptionFilesInBucket = observable.Subscribe(
                item => FilesInBucket.Add(item.Key),
                ex => throw new Exception(ex.Message));
        }

        public async Task<BucketNotification> GetBucketNotifications(string bucket)
        {
            GetBucketNotificationsArgs args = new GetBucketNotificationsArgs()
                .WithBucket(bucket);
            return await _context.Minio.GetBucketNotificationsAsync(args);
        }

        public async Task<IEnumerable<Bucket>> GetAllBuckets()
        {
            ListAllMyBucketsResult list = await _context.Minio.ListBucketsAsync();
            return list.Buckets;
        }

        public void ListenIncompleteUploads(string bucket, string? prefix, bool? recursive = true)
        {
            ListIncompleteUploadsArgs args = new ListIncompleteUploadsArgs()
                .WithBucket(bucket)
                .WithPrefix(prefix)
                .WithRecursive((bool)recursive);

            IObservable<Upload> observable = _context.Minio.ListIncompleteUploads(args);
            SubscriptionIncompleteUpoads = observable.Subscribe(
                item => IncompleteUploads.Add(item.Key),
                ex => throw new Exception(ex.Message));
        }

        public void ListenBucketNotifications(string bucket, List<EventType> events, string? prefix = "", string? suffix = "", bool? recursive = true)
        {
            events = events ?? new List<EventType> { EventType.ObjectCreatedAll };
            ListenBucketNotificationsArgs args = new ListenBucketNotificationsArgs()
                .WithBucket(bucket)
                .WithPrefix(prefix)
                .WithEvents(events)
                .WithSuffix(suffix);

            IObservable<MinioNotificationRaw> observable = _context.Minio.ListenBucketNotificationsAsync(bucket, events, prefix, suffix);
            SubscriptionNotifications = observable.Subscribe(
                notification => Notifications.Add(notification.json),
                ex => throw new Exception(ex.Message));
        }

        public async Task RemoveAllBucketNotifications(string bucket)
        {
            RemoveAllBucketNotificationsArgs args = new RemoveAllBucketNotificationsArgs()
                .WithBucket(bucket);
            await _context.Minio.RemoveAllBucketNotificationsAsync(args);
        }

        public async Task RemoveIncompleteUpload(string bucket, string obj)
        {
            RemoveIncompleteUploadArgs args = new RemoveIncompleteUploadArgs()
                .WithBucket(bucket)
                .WithObject(obj);
            await _context.Minio.RemoveIncompleteUploadAsync(args);
        }

        public async Task RemoveObject(string bucket, string obj, string? versionId = null)
        {
            RemoveObjectArgs args = new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(obj);

            if (!string.IsNullOrEmpty(versionId))
            {
                args = args.WithVersionId(versionId);
            }

            await _context.Minio.RemoveObjectAsync(args);
        }

        public async Task RemoveObjects(string bucket, List<string> objs)
        {
            RemoveObjectsArgs objArgs = new RemoveObjectsArgs()
                .WithBucket(bucket)
                .WithObjects(objs);

            IObservable<DeleteError> objectsOservable = await _context.Minio.RemoveObjectsAsync(objArgs).ConfigureAwait(false);
            SubscriptionObjects = objectsOservable.Subscribe(
                objDeleteError => Objects.Add(objDeleteError.Key),
                ex => throw new Exception(ex.Message));
        }
    }
}
