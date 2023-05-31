using Business.Domain.Model;
using Business.Repository.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.IoC
{
    public static class ContainerIoC
    {
        public static IServiceProvider Register(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoConnection>(configuration.GetSection("MongoConnection"));
            services.Configure<RedisConnection>(configuration.GetSection("RedisConnection"));
            services.Configure<MinioConnection>(configuration.GetSection("MinioConnection"));

            MongoPersistence.Configure();

            ADServerIoC.Register(services, configuration);
            return services.BuildServiceProvider();
        }
    }
}
