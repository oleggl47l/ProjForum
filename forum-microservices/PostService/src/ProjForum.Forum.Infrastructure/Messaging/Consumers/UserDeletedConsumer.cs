using MassTransit;
using Microsoft.Extensions.Logging;
using ProjForum.Forum.Domain.Interfaces;
using SharedModels;

namespace ProjForum.Forum.Infrastructure.Messaging.Consumers;

public class UserDeletedConsumer(IPostRepository postRepository, ILogger<UserStatusChangedEventConsumer> logger) 
    : IConsumer<UserDeletedEvent>

{
    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        logger.LogInformation("UserDeletedEventConsumer started.");

        var userDeletedEvent = context.Message;
        var userId = Guid.Parse(userDeletedEvent.UserId);

        logger.LogInformation($"Set author guid to empty fo user-related posts. UserId: {userId}");

        var posts = await postRepository.GetPostsByUserIdAsync(userId);
        foreach (var post in posts)
        {
            post.AuthorId = Guid.Empty;
            await postRepository.Update(post);
        }

        logger.LogInformation($"User posts updated with empty user ID. UserId: {userId}");

    }
}