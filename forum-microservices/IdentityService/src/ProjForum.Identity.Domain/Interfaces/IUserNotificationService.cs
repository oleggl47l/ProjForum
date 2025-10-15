namespace ProjForum.Identity.Domain.Interfaces;

public interface IUserNotificationService
{
    Task NotifyUserStatusChanged(Guid userId, bool active, DateTime changedAt);
    Task NotifyUserDeleted(Guid userId, DateTime deletedAt);
}