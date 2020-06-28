using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Kutori
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:5000");
                });
    }
}
