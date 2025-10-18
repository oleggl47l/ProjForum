namespace ProjForum.Identity.Domain.Exceptions;

public class UserBlockedException(string userName, DateTimeOffset lockoutEnd)
    : Exception(string.Format(MessageFormat, userName, lockoutEnd.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")))
{
    private const string MessageFormat = "User {0} blocked till {1}.";
}