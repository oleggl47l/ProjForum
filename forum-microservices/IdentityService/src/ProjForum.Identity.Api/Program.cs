using Microsoft.EntityFrameworkCore;
using ProjForum.Identity.Api.Extensions;
using ProjForum.Identity.Application.Extensions;
using ProjForum.Identity.Infrastructure.Persistence;
using ProjForum.Identity.Infrastructure.Persistence.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build()).WriteTo.Console()
    .CreateLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Host.UseSerilog();

    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddApiServices(configuration);
    builder.Services.AddSwagger(configuration);

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    await app.UseIdentitySeedingAsync();

    if (app.Environment.IsDevelopment())
        app.UseSwaggerUi(configuration);

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseExceptionHandler();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}