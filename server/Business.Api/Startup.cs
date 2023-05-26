using Api.Hubs;
using AutoMapper;
using Business.Api.Filters;
using Business.Api.Middlewares;
using Business.Api.Providers;
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
using Business.Domain.Validators;
using Business.Repository.Persistence;
using Business.Repository.Repositories;
using Business.Service.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;

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
            RegisterPersistence();

            RegisterRateLimit(services);

            RegisterControllers(services);

            RegisterApi(services);

            RegisterAuthentication(services);

            RegisterFluent(services);

            RegisterSwagger(services);

            RegisterMemoryCache(services);

            RegisterSignalR(services);

            RegisterCors(services);

            RegisterOptions(services);

            RegisterConnection(services, Configuration);

            RegisterServices(services);

            RegisterServiceLocator(services);

            services.AddHealthChecks();

            ReddisInnitialPayload();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseMiddleware<RequestRewindMiddleware>(Array.Empty<object>());

            app.UseHealthChecks("/status");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "swagger-ui")),
                RequestPath = new PathString("/swagger-ui")
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "rate-limit")),
                RequestPath = new PathString("/rate-limit")
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        options.InjectStylesheet("/swagger-ui/dark-theme.css");
                    }
                });
            }

            string[] languages = { "pt-BR", "en-US" };
            string defaultLanguage = languages.FirstOrDefault();
            CultureInfo[] supportedCultures = languages.Select(x => new CultureInfo(x)).ToArray();
            RequestLocalizationOptions requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultLanguage),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            requestLocalizationOptions.RequestCultureProviders.Insert(0, new UrlRequestCultureProvider());
            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/notification");
            });
        }

        private static void RegisterOptions(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });
        }

        private static void RegisterConnection(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoConnection>(configuration.GetSection("MongoConnection"));
            services.Configure<RedisConnection>(configuration.GetSection("RedisConnection"));
            services.Configure<MinioConnection>(configuration.GetSection("MinioConnection"));
        }

        private static void RegisterControllers(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(RequestLoggingFilter));
            });
        }

        private static void RegisterApi(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddEndpointsApiExplorer();
        }

        private void RegisterAuthentication(IServiceCollection services)
        {
            IConfigurationSection appSettingsSection = Configuration.GetSection("IdentityConfig");
            services.Configure<IdentityConfig>(appSettingsSection);

            IdentityConfig appSettings = appSettingsSection.Get<IdentityConfig>();
            byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = appSettings.ValidAudience,
                    ValidIssuer = appSettings.ValidIssuer
                };
            });
        }

        private static void RegisterFluent(IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddValidatorsFromAssemblyContaining<UserValidator>();
        }

        private static void RegisterSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ADServer API",
                    Description = "More in: https://dalacorte.gitbook.io/boilerplate/",
                    Contact = new OpenApiContact
                    {
                        Name = "GitHub",
                        Url = new Uri("https://github.com/dalacorte")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },

                });

                options.IncludeXmlComments(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Business.Api.xml"));


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\nExample: \"Bearer {{token}}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                options.DescribeAllParametersInCamelCase();
            });
        }

        private static void RegisterMemoryCache(IServiceCollection services)
        {
            services.AddMemoryCache();
        }

        private static void RegisterCors(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowSetOrigins",
                builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .SetIsOriginAllowed((host) => true)
                           .AllowCredentials();
                }));
        }

        private static void RegisterSignalR(IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 102400000;
            });
        }

        private static void RegisterPersistence()
        {
            MongoPersistence.Configure();
        }

        private void RegisterRateLimit(IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    string currentUrl = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host.Value}/rate-limit/index.html";

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                    {
                        await context.HttpContext.Response.WriteAsync(
                            $"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s). " +
                            $"Read more about our rate limits at {currentUrl}.", cancellationToken: token);
                    }
                    else
                    {
                        await context.HttpContext.Response.WriteAsync(
                            $"Too many requests. Please try again later. " +
                            $"Read more about our rate limits at {currentUrl}.", cancellationToken: token);
                    }
                };

                options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                    PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = Convert.ToBoolean(Configuration.GetSection("RateLimiter:0:AutoRepleni shment").Value),
                                PermitLimit = Convert.ToInt32(Configuration.GetSection("RateLimiter:0:PermitLimit").Value),
                                QueueLimit = Convert.ToInt32(Configuration.GetSection("RateLimiter:0:QueueLimit").Value),
                                QueueProcessingOrder = Enum.TryParse(Configuration.GetSection("RateLimiter:0:QueueProcessingOrder").Value, out QueueProcessingOrder result) ? result : default,
                                Window = TimeSpan.FromSeconds(Convert.ToDouble(Configuration.GetSection("RateLimiter:0:Window").Value))
                            })),
                    PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = Convert.ToBoolean(Configuration.GetSection("RateLimiter:1:AutoReplenishment").Value),
                                PermitLimit = Convert.ToInt32(Configuration.GetSection("RateLimiter:1:PermitLimit").Value),
                                QueueLimit = Convert.ToInt32(Configuration.GetSection("RateLimiter:1:QueueLimit").Value),
                                QueueProcessingOrder = Enum.TryParse(Configuration.GetSection("RateLimiter:1:QueueProcessingOrder").Value, out QueueProcessingOrder result) ? result : default,
                                Window = TimeSpan.FromSeconds(Convert.ToDouble(Configuration.GetSection("RateLimiter:1:Window").Value))
                            })));
            });
        }

        private void ReddisInnitialPayload()
        {
            IRedisRepository redis = ServiceLocator.Resolve<IRedisRepository>();
            redis.Set("uniqueidentifier", Configuration.GetSection("Config:uniqueidentifier").Value);
            redis.Set("defaultdisk", Configuration.GetSection("Config:defaultdisk").Value);
        }

        private void RegisterServiceLocator(IServiceCollection services)
        {
            ServiceLocator.Init(services.BuildServiceProvider());
        }

        private void RegisterServices(IServiceCollection services)
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                Action<IMapperConfigurationExpression> config = _ => { };

                config(cfg);

                IEnumerable<Assembly> assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();

                TypeInfo[] allTypes = assembliesToScan.Where(a => a.GetName().Name != nameof(AutoMapper))
                                                      .SelectMany(a => a.DefinedTypes)
                                                      .ToArray();

                TypeInfo profileTypeInfo = typeof(Profile).GetTypeInfo();
                TypeInfo[] profiles = allTypes.Where(t => profileTypeInfo.IsAssignableFrom(t) && !t.IsAbstract).ToArray();

                foreach (Type profile in profiles.Select(t => t.AsType()))
                {
                    cfg.AddProfile(profile);
                }
            });
            IMapper mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

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

            services.AddScoped<ILogRequestRepository, LogRequestRepository>();

            services.AddSingleton<IRedisRepository, RedisRepository>();
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{Configuration.GetSection("RedisConnection:Host").Value}:{Configuration.GetSection("RedisConnection:Port").Value}"));
        }
    }
}
