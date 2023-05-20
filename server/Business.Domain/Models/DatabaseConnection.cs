namespace Business.Domain.Model
{
    public class MongoConnection
    {
        public string ConnectionString { get; set; }
        public string Hangfire { get; set; }
        public string Database { get; set; }
    }

    public class RedisConnection
    {
        public string Host { get; set; }
        public string Port { get; set; }
    }

    public class MinioConnection
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
