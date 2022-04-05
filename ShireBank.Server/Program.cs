using Microsoft.EntityFrameworkCore;
using ShireBank.Server.Database;
using ShireBank.Server.Database.Handlers;
using ShireBank.Server.Database.Queries;
using ShireBank.Server.Database.Queries.Interfaces;
using ShireBank.Server.Interceptors;
using ShireBank.Server.Services;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Additional configuration is required to successfully run gRPC on macOS.
    // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

    // Add services to the container.
    builder.Services
        .AddGrpc()
        .AddServiceOptions<CustomerService>(options =>
        {
            options.Interceptors.Add<InspectionInterceptor>();
        });

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    builder.Services.AddDbContext<DataContext>(x => x.UseSqlite("Data Source=ShireBank.db"));

    builder.Services.AddScoped<IAccountQueries, AccountQueries>();
    builder.Services.AddSingleton<IResilientDbHandler, ResilientDbHandler>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapGrpcService<CustomerService>();
    app.MapGrpcService<InspectorService>();

    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.Run();

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}