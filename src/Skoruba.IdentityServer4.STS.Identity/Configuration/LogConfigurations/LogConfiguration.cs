using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using Serilog.Sinks.RabbitMQ;
using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration.LogConfigurations
{
    [ExcludeFromCodeCoverage]
    public static class LogConfiguration
    {
        public static void AddSerilogLabsoftApplicationLog(
            IServiceCollection services,
            IConfiguration configuration,
            string environment)
        {
            var jsonFormatter = new JsonFormatter(closingDelimiter: null, renderMessage: true, formatProvider: null);
            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationName", "Labsoft.IdentityCenter.Sts")
                .Enrich.WithProperty("Environment", environment)
                .MinimumLevel.Error()
                .WriteTo.RabbitMQ(
                    hostname: configuration["ApplicationLog_HostName"],
                    port: int.Parse(configuration["ApplicationLog_Port"]),
                    vHost: configuration["ApplicationLog_vHost"],
                    username: configuration["ApplicationLog_UserName"],
                    password: configuration["ApplicationLog_Password"],
                    exchange: configuration["ApplicationLog_Exchange"],
                    routeKey: configuration["ApplicationLog_RoutingKey"],
                    exchangeType: "direct",
                    deliveryMode: RabbitMQDeliveryMode.NonDurable,
                    formatter: jsonFormatter)
                .WriteTo.Console()
                .CreateLogger();

            services.AddLogging(lb => lb.AddSerilog(logger));
        }
    }
}
