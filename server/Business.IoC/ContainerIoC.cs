using Business.Database;
using Business.Domain;
using Business.Domain.Interfaces.Mongo;
using Business.Domain.Interfaces.Repositories;
using Business.Repository.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Business.IoC
{
    public static class ContainerIoC
    {
        public static IServiceProvider Register(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            ServiceLocator.Init(services.BuildServiceProvider());
            MongoPersistence.Configure();

            ADServerIoC.Register(services, configuration);
            return services.BuildServiceProvider();
        }
    }
}
