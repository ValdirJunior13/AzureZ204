using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
var builder = FunctionsApplicationInsightsExtensions.CreateBuilder(args);
builder.ConfigureFunctionsApplication();
builder.Services.Configure<KestrelServerOptions>(OptionsBuilderConfigurationExtensions =>
{
    options.Limits.MaxRequestBufferSize = 1024 * 1024 * 1024;

}
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();


host.Run();