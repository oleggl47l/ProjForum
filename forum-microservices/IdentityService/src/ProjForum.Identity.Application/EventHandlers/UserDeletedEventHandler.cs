using MediatR;
using ProjForum.BuildingBlocks.Domain.Events;
using ProjForum.Identity.Domain.Interfaces;

namespace ProjForum.Identity.Application.EventHandlers;

public class UserDeletedEventHandler(IUserNotificationService notificationService)
    : INotificationHandler<UserDeletedDomainEvent>
{
    public async Task Handle(UserDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.NotifyUserDeleted(notification.UserId, notification.DeletedAt);
    }
}