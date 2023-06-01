using Business.Application.Application;
using Business.Application.UOW;
using Business.Database;
using Business.Domain;
using Business.Domain.Interfaces.Application;
using Business.Domain.Interfaces.Mongo;
using Business.Domain.Interfaces.Repositories;
using Business.Domain.Interfaces.Services;
using Business.Domain.Interfaces.UOW;
using Business.Domain.Model;
using Business.Domain.Models.Others;
using Business.Repository.Repositories;
using Business.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Business.IoC
{
    public static class ADServerIoC
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            services.AddScoped<IMongoContext, MongoContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserApplication<User, UnitOfWork>, UserApplication<User, UnitOfWork>>();
            services.AddScoped<IUserService<User>, UserService<User>>();
            services.AddScoped<IUserRepository<User>, UserRepository<User>>();

            services.AddScoped<IFileApplication, FileApplication>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileRepository, FileRepository>();

            services.AddScoped<ITokenApplication, TokenApplication>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<ILogRequestApplication<LogRequest, UnitOfWork>, LogRequestApplication<LogRequest, UnitOfWork>>();
            services.AddScoped<ILogRequestService<LogRequest>, LogRequestService<LogRequest>>();
            services.AddScoped<ILogRequestRepository<LogRequest>, LogRequestRepository<LogRequest>>();

            services.AddSingleton<IRedisRepository, RedisRepository>();

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{configuration.GetSection("RedisConnection:Host").Value}:{configuration.GetSection("RedisConnection:Port").Value}"));

            ServiceLocator.Init(services.BuildServiceProvider());
            IRedisRepository redis = ServiceLocator.Resolve<IRedisRepository>();
            redis.Set("uniqueidentifier", configuration.GetSection("Config:uniqueidentifier").Value);
            redis.Set("defaultdisk", configuration.GetSection("Config:defaultdisk").Value);
        }
    }
}
