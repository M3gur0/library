using Microsoft.Extensions.Configuration;
using Serilog;

namespace M3gur0.Library.WebAPI.Extensions
{
    public static class LoggerExtensions
    {
        public static ILogger InitializeLogger(this IConfiguration configuration, string appName)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("ApplicationContext", appName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger = logger;

            return logger;
        }
    }
}
