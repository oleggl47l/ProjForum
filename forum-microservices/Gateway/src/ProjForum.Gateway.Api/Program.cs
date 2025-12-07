using ProjForum.Gateway.Api.Extensions;
using ProjForum.Gateway.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false,
    reloadOnChange: true);

builder.Host.AddSerilogLogging(builder.Configuration);

builder.Services.ConfigureOptions(builder.Configuration);

builder.Services
    .AddCorsConfiguration()
    .AddJwtAuth(builder.Configuration)
    .AddDocumentation(builder.Configuration)
    .AddGateway(builder.Configuration);

var app = builder.Build();

app.UseAppPipeline();

await app.RunGatewayAsync();