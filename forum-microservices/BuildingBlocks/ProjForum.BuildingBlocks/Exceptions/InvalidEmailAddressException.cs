using System.Net.Mail;
using ProjForum.BuildingBlocks.Domain;

namespace ProjForum.BuildingBlocks.Exceptions;

public class InvalidEmailAddressException() : DomainException(Messages)
{
    private const string Messages = "Invalid Email Address.";

    public static void Throw(string email)
    {
        if (!MailAddress.TryCreate(email, out _))
        {
            throw new InvalidEmailAddressException();
        }
    }
}