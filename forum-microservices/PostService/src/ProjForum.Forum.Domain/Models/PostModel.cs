namespace ProjForum.Forum.Domain.Models;

public class PostModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; } =  true;
    public IEnumerable<string>? TagNames { get; set; }
}