using MediatR;
using ProjForum.BuildingBlocks.Domain.Interfaces;
using ProjForum.Identity.Application.DTOs;
using ProjForum.Identity.Application.DTOs.User;
using ProjForum.Identity.Domain.Identities;
using ProjForum.Identity.Domain.Interfaces.Repositories;

namespace ProjForum.Identity.Application.Identity.Commands.Users.CreateUser;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, CreateUserResultDto>
{
    public async Task<CreateUserResultDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
            throw new InvalidOperationException("Email already in use");

        if (await userRepository.GetByUserNameAsync(request.UserName, cancellationToken) is not null)
            throw new InvalidOperationException("Username already in use");

        foreach (var role in request.Roles)
        {
            var existingRole = await roleRepository.GetByNameAsync(role, cancellationToken);
            if (existingRole is null)
                throw new KeyNotFoundException($"Role '{role}' not found");
        }

        var user = User.Create(request.UserName, request.Email);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await userRepository.AddAsync(user, cancellationToken);

            foreach (var role in request.Roles)
                await userRepository.AddToRoleAsync(user, role, cancellationToken);
        }, cancellationToken);

        var roles = await userRepository.GetRolesAsync(user, cancellationToken);
        var dto = new UserDto(user.Id, user.UserName, user.Email, user.Active, user.AccessFailedCount, roles);

        return new CreateUserResultDto(new OperationResultDto(true, "User created successfully"), dto);
    }
}