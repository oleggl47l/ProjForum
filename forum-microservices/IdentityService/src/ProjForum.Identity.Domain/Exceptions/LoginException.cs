namespace ProjForum.Identity.Domain.Exceptions;

public class LoginException() : Exception(MESSAGE)
{
    private const string MESSAGE = "Incorrect username or password.";
}