using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var env = hostContext.HostingEnvironment;
                    config.SetBasePath(Path.Combine(env.ContentRootPath, "Configuration"))
                          .AddJsonFile(path: "settings.json", optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
