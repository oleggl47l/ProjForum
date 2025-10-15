using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Services;

public class UnblockUsersBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<UnblockUsersBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var (users, _) = await userRepository.GetPagedAsync(0, 100, stoppingToken);
                foreach (var user in users.Where(u => !u.Active))
                {
                    // проверка lockout → лучше вынести в UserRepository
                    user.Activate();
                    logger.LogInformation($"User {user.UserName} has been unlocked.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while unlocking users.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}