using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Aff.YB.DataSync.API.Site
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((builderContext, config) =>
               {
                   var env = builderContext.HostingEnvironment;
                   config.SetBasePath(env.ContentRootPath);
                   config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                   config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);
                   config.AddEnvironmentVariables();
               })
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               })
               .UseSerilog();
    }
}
