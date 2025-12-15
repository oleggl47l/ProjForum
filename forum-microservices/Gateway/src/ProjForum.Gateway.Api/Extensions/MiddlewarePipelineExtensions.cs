using Ocelot.Middleware;

namespace ProjForum.Gateway.Api.Extensions;

public static class MiddlewarePipelineExtensions
{
    public static void UseAppPipeline(this WebApplication app)
    {
        app.UseCors();

        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
        {
            app.UseSwagger();
            app.UseSwaggerForOcelotUI();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    public static async Task RunGatewayAsync(this WebApplication app)
    {
        await app.UseOcelot();
        await app.RunAsync();
    }
}