using Serilog;
using Starfish.GrpcService.Server;
using Starfish.GrpcService.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton<IStarfishRateLimiterService, StarfishRateLimiterService>();

// Use Serilog
builder.Host.UseSerilog((builderContext, serviceProvider, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(builderContext.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<FraudCheckerService>();
app.MapGrpcService<RateLimiterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();