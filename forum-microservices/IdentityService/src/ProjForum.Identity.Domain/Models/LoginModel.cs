namespace ProjForum.Identity.Domain.Models;

public class LoginModel
{
    public string Id { get; init; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string AccessToken { get; set; }
}