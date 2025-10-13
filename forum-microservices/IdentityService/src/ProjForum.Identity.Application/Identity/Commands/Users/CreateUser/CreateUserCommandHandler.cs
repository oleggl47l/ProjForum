using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjForum.Identity.Application.Identity.Commands.Users.CreateUser;

public class CreateUserCommandHandler(
    UserManager<Domain.Entities.User> userManager,
    RoleManager<Domain.Entities.Role> roleManager) : IRequestHandler<CreateUserCommand, Unit>
{
    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new Domain.Entities.User
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var roles = await GetRolesByNames(request.RoleNames);
        if (roles == null || !roles.Any())
            throw new ArgumentException("One or more roles do not exist.");


        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user: {errors}");
        }

        await AddRolesToUserAsync(user, request.RoleNames);

        return Unit.Value;
    }

    private async Task<List<Domain.Entities.Role>> GetRolesByNames(IEnumerable<string> roleNames)
    {
        var roles = new List<Domain.Entities.Role>();

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                throw new ArgumentException($"Role with name {roleName} does not exist.");

            roles.Add(role);
        }

        return roles;
    }

    private async Task AddRolesToUserAsync(Domain.Entities.User user,
        List<string> roleNames)
    {
        foreach (var roleName in roleNames)
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
                throw new Exception($"Failed to add role {roleName} to user {user.UserName}");
        }
    }
}