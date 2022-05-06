using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HRD
{
    public class Program
    {
        /// <summary>
        /// The entry point of the program
        /// </summary>
        /// <param name="args">Process passed arguments</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Builds the webservice, and specified to use the Startup class for configuration
        /// </summary>
        /// <param name="args">Process passed arguments</param>
        /// <returns>The builder object used for building the webserver</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
