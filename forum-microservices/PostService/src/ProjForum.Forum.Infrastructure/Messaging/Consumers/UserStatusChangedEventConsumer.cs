using MassTransit;
using Microsoft.Extensions.Logging;
using ProjForum.Forum.Domain.Interfaces;
using SharedModels;

namespace ProjForum.Forum.Infrastructure.Messaging.Consumers;

public class UserStatusChangedEventConsumer(IPostRepository postRepository, ILogger<UserStatusChangedEvent> logger) : IConsumer<UserStatusChangedEvent>
{
    public async Task Consume(ConsumeContext<UserStatusChangedEvent> context)
    {
        logger.LogInformation("UserStatusChangedEventConsumer started.");
        
        var userStatusChangedEvent = context.Message;

        var userId = Guid.Parse(userStatusChangedEvent.UserId);

        if (!userStatusChangedEvent.Active)
        {
            logger.LogInformation("User inactive. DePublish posts.");

            var posts = await postRepository.GetPostsByUserIdAsync(userId);
            foreach (var post in posts)
            {
                post.IsPublished = false;
                await postRepository.Update(post);
            }
        }
        else
        {
            logger.LogInformation("User active. Re-publish posts if applicable.");

            
            var posts = await postRepository.GetPostsByUserIdAsync(userId);
            foreach (var post in posts)
            {
                if (!post.IsPublished)
                {
                    post.IsPublished = true;
                    await postRepository.Update(post);
                }
            }
        }
    }
}
