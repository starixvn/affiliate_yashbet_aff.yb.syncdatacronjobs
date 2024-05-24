using Aff.YB.DataSync.Service.Host.Extensions;
using Aff.YB.DataSync.Service.Host.Filters;
using Aff.YB.DataSync.Service.Host.Middleware;
using Aff.YB.DataSync.Service.Host.Settings;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;

namespace Aff.YB.DataSync.Service.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigFanexData();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddLogging();
            services.AddUnderMaintenance(Configuration);
            services.AddHttpClient();
            services.AddSiteHealthChecks(Configuration);

            AddHangFireService(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UsePing();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<ErrorHandler>();
            app.UseSiteHealthChecks(Configuration);
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHangFire("/jobs");
            app.UseRecuringJobs(Configuration);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

        }

        private void ConfigFanexData()
        {
            var listConnectionName = Configuration.GetSection("ConnectionStrings").GetChildren().Select(_ => _.Key);
            var connectionStrings = new Dictionary<string, ConnectionConfiguration>(listConnectionName
                .Select(connectionName => new KeyValuePair<string, ConnectionConfiguration>(connectionName,
                    new ConnectionConfiguration(connectionName, Configuration.GetConnectionString(connectionName)))
                ));

            DbSettingProviderManager
                .StartNewSession()
                .Use(connectionStrings)
                .Register("Microsoft.Data.SqlClient", ObjectDbFactoryFunc(), "SpConfig/SpNames.xml", false)
                .Run();

            static CreateObjectDbFunc ObjectDbFactoryFunc()
            {
                return (string settingKey, int? commandTimeout) => new SqlObjectDb(settingKey, commandTimeout);
            }

        }

        private void AddHangFireService(IServiceCollection services)
        {
            var redisConfig = Configuration.GetSection("Redis").Get<RedisSetting>();
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                KeepAlive = 180,
                EndPoints = { { redisConfig.Server, redisConfig.Port } },
                Password = redisConfig.Pass,
                DefaultDatabase = redisConfig.Database,
            });

            services.AddHangfire((serviceProvider, configuration) =>
            {
                configuration.UseRedisStorage(redis,new RedisStorageOptions
                {
                    Prefix = redisConfig.AppName,
                    Db = redisConfig.Database
                }).WithJobExpirationTimeout(TimeSpan.FromHours(2));
                configuration.UseFilter(serviceProvider.GetService<GlobalHangfireJobExceptionFilterAttribute>());
                configuration.UseFilter(serviceProvider.GetService<UMHangfireFilterAttribute>());
            });

            services.AddHangfireServer(options =>
            {
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });

            services.AddCronJobsDI(Configuration);

        }

    }
}
