namespace Server
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Entry point to ASP.NET core app.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point to program.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create host builder to start ASP.NET core app.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}