using System.ComponentModel.DataAnnotations;
using ProjForum.Identity.Domain.Models;
using MediatR;

namespace ProjForum.Identity.Application.Identity.Queries;

public class LoginQuery : IRequest<LoginModel>
{
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
}