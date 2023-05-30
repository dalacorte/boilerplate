using Business.Background.Tasks;
using Business.Domain;
using Business.Domain.Interfaces.Repositories;
using Business.Domain.Interfaces.Services;
using Business.Domain.Model;
using Business.Repository.Repositories;
using Business.Service.Services;
using Hangfire;
using Hangfire.Logging;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;

namespace AD.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Business.Domain.Model.MongoConnection>(Configuration.GetSection("MongoConnection"));

            services.Configure<RedisConnection>(Configuration.GetSection("RedisConnection"));

            services.AddHangfire(config =>
            {
                MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder(Configuration.GetSection("MongoConnection:ConnectionString").Value)
                {
                    DatabaseName = "hangfire",
                    AuthenticationSource = "admin",
                    AuthenticationMechanism = "SCRAM-SHA-256"
                };
                MongoClient mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

                MongoStorageOptions storageOptions = new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                    InvisibilityTimeout = TimeSpan.FromMinutes(5),
                    CheckConnection = true
                };

                config.UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, storageOptions)
                      .UseColouredConsoleLogProvider(LogLevel.Info);
            });
            services.AddHangfireServer(options =>
            {
                options.Queues = new[] { "default" };
            });

            services.AddSingleton<IRedisRepository, RedisRepository>();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{Configuration.GetSection("RedisConnection:Host").Value}:{Configuration.GetSection("RedisConnection:Port").Value}"));

            ServiceLocator.Init(services.BuildServiceProvider());
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHangfireDashboard();

            using (IStorageConnection connection = JobStorage.Current.GetConnection())
            {
                foreach (RecurringJobDto recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            RecurringJob.AddOrUpdate<RedisConsumerTask>(x => x.RedisConsumer(), Cron.Minutely);
            RecurringJob.AddOrUpdate<RandomImageTask>(x => x.GetImage(), Cron.Minutely);
            RecurringJob.AddOrUpdate<CorruptFileTask>(x => x.CorruptFile(), Cron.Daily);
        }
    }
}
