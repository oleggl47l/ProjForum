using MassTransit;
using ProjForum.Identity.Domain.Interfaces;

namespace ProjForum.Identity.Infrastructure.Messaging;

public class UserNotificationService(IPublishEndpoint publishEndpoint) : IUserNotificationService
{
    public async Task NotifyUserStatusChanged(Guid userId, bool active, DateTime changedAt)
    {
        await publishEndpoint.Publish(new
        {
            UserId = userId,
            Active = active,
            ChangedAt = changedAt
        });
    }

    public async Task NotifyUserDeleted(Guid userId, DateTime deletedAt)
    {
        await publishEndpoint.Publish(new
        {
            UserId = userId,
            DeletedAt = deletedAt
        });
    }
}