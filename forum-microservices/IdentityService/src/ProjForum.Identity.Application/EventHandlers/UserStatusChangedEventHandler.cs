using MediatR;
using ProjForum.BuildingBlocks.Domain.Events;
using ProjForum.Identity.Domain.Interfaces;

namespace ProjForum.Identity.Application.EventHandlers;

public class UserStatusChangedEventHandler(IUserNotificationService notificationService)
    : INotificationHandler<UserStatusChangedDomainEvent>
{
    public async Task Handle(UserStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.NotifyUserStatusChanged(notification.UserId, notification.Active,
            notification.ChangedAt);
    }
}