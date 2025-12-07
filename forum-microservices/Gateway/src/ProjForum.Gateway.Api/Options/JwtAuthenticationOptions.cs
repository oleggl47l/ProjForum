namespace ProjForum.Gateway.Api.Options;

public class JwtAuthenticationOptions
{
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}