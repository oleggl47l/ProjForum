namespace ProjForum.Forum.Domain.Models;

public class CommentModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Content { get; set; } = string.Empty;
}