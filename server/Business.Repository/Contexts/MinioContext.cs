using Business.Domain.Model;
using Microsoft.Extensions.Options;
using Minio;

namespace Business.Repository.Contexts
{
    public class MinioContext
    {
        public MinioClient Minio;

        public MinioContext(IOptions<MinioConnection> settings)
        {
            Minio = new MinioClient()
                            .WithEndpoint(settings.Value.Endpoint)
                            .WithCredentials(settings.Value.AccessKey, settings.Value.SecretKey)
                            .Build();
        }
    }
}