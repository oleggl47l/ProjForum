using System.ComponentModel.DataAnnotations;
using MediatR;
using ProjForum.Identity.Domain.Models;

namespace ProjForum.Identity.Application.Identity.Queries.Auth.Login;

public class LoginQuery : IRequest<LoginModel>
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
}