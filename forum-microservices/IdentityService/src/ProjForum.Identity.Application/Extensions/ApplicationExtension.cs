using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjForum.Identity.Application.Behaviours;
using ProjForum.Identity.Application.Services;
using ProjForum.Identity.Domain.Interfaces;
using ProjForum.Identity.Infrastructure.Messaging;

namespace ProjForum.Identity.Application.Extensions;

public static class ApplicationExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.Lifetime = ServiceLifetime.Scoped;
            configuration.RegisterServicesFromAssembly(typeof(ApplicationExtension).GetTypeInfo().Assembly);
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddHostedService<UnblockUsersBackgroundService>();

        services.AddScoped<IUserNotificationService, UserNotificationService>();
    }
}