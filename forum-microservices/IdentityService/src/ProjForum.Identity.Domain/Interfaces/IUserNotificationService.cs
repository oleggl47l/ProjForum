namespace ProjForum.Identity.Domain.Interfaces;

public interface IUserNotificationService
{
    Task NotifyUserStatusChanged(string userId);
    Task NotifyUserDeleted(string userId);
}