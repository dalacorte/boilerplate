using Minio.DataModel;

namespace Business.Domain.Model
{
    public class FileDTO
    {
        public string? Bucket { get; set; }
        public string? FilePath { get; set; }
        public string? Obj { get; set; }
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public string? VersionId { get; set; }
        public string? FromBucket { get; set; }
        public string? FromObj { get; set; }
        public string? ToBucket { get; set; }
        public string? ToObj { get; set; }
        public bool? Recursive { get; set; }
        public bool? Versions { get; set; }
        public IServerSideEncryption? SSE { get; set; }
        public IServerSideEncryption? SSESRC { get; set; }
        public IServerSideEncryption? SSEDEST { get; set; }
        public List<string>? Objs { get; set; }
        public List<EventType>? Events { get; set; }
    }
}
