using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjForum.Identity.Domain.Entities;
using ProjForum.Identity.Domain.Interfaces;

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
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserNotificationService>();

                var users = userManager.Users.Where(u => !u.Active).ToList();

                foreach (var user in users)
                {
                    var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);

                    if (lockoutEnd <= DateTime.UtcNow)
                    {
                        user.Active = true;
                        await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow);
                        await userManager.UpdateAsync(user);
                        logger.LogInformation($"User {user.UserName} has been unlocked.");
                        await userService.NotifyUserStatusChanged(user.Id);
                    }
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